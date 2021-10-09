using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Injector {
	internal static class Program {
		private static readonly string[] FileList = { "Mono.CSharp.dll", "ICSharpCode.Decompiler.dll" };
		private static readonly string[] DependencyList = { "Humanizer.dll", "System.Collections.Immutable.dll", "System.Reflection.Metadata.dll" };

		private static void Main() {
			var startupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var assemblyPath = Path.Combine(startupPath, "RuntimeInspector.dll");
			if (!File.Exists(assemblyPath)) {
				throw new FileNotFoundException(assemblyPath);
			}
			foreach (var name in FileList.Where(name => !File.Exists(Path.Combine(startupPath, name)))) {
				throw new FileNotFoundException(name);
			}
			var missingDependencyList = new List<string>();
			foreach (var name in DependencyList) {
				try {
					Assembly.Load(new AssemblyName(name));
				} catch {
					missingDependencyList.Add(name);
				}
			}
			foreach (var name in missingDependencyList.Where(name => !File.Exists(Path.Combine(startupPath, name)))) {
				throw new FileNotFoundException(name);
			}
			Console.WriteLine("Please input process name or pid: ");
			var input = Console.ReadLine();
			Process process = null;
			if (int.TryParse(input, out var pid)) {
				process = Process.GetProcessById(pid);
			}
			if (process == null) {
				var processes = Process.GetProcessesByName(input);
				if (processes.Length == 0) {
					throw new Exception("Cannot find process: " + input);
				}
				if (processes.Length > 1) {
					Console.WriteLine("These processes has same name, please select one:");
					for (var i = 0; i < processes.Length; i++) {
						Console.WriteLine($"[{i + 1}] PID: {processes[i].Id}");
					}
					var index = int.Parse(Console.ReadLine() ?? "1");
					process = processes[index - 1];
				} else {
					process = processes[0];
				}
			}
			var gamePath = GetProcessPath(process.Id);
			Console.WriteLine("Game path: " + gamePath);
			var gameName = Path.GetFileNameWithoutExtension(gamePath);
			var injectPath = Path.Combine(Path.GetDirectoryName(gamePath) ?? string.Empty, gameName + "_Data", "Managed");
			if (!Directory.Exists(injectPath)) {
				throw new Exception("Selected process is not a mono Unity Game.");
			}

			foreach (var name in FileList.Concat(missingDependencyList)) {
				var dstPath = Path.Combine(injectPath, name);
				if (File.Exists(dstPath)) {
					continue;
				}
				File.Copy(Path.Combine(startupPath, name), dstPath);
			}

			Console.WriteLine("Assemblies copy finished.");
			var injector = new SharpMonoInjector.Injector(process.Id);
			var ptr = injector.Inject(File.ReadAllBytes(assemblyPath), "RuntimeInspector", "ViewerCreator", "Create");
			var color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Inject successfully! Press any key to Eject.");
			Console.ForegroundColor = color;
			Console.ReadKey();
			injector.Eject(ptr, "RuntimeInspector", "ViewerCreator", "Create");
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, ref int lpdwSize);
		private const int QueryLimitedInformation = 0x00001000;

		public static string GetProcessPath(int pid) {
			var size = 1024;
			var sb = new StringBuilder(size);
			var handle = OpenProcess(QueryLimitedInformation, false, pid);
			if (handle == IntPtr.Zero) {
				return null;
			}
			var flag = QueryFullProcessImageName(handle, 0, sb, ref size);
			CloseHandle(handle);
			return flag ? sb.ToString() : null;
		}
	}
}
