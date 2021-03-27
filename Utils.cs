using System;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

namespace InGameDebugger {
	public static class Utils {
		[DllImport("user32.dll")]
		public static extern int MessageBox(IntPtr hwnd, string text, string caption, uint type);

		public static void LogError(string message, string title) {
			var fn = $"{Application.persistentDataPath}/{DateTime.Now:yyyy-MM-dd}.txt";
			if (File.Exists(fn)) {
				var oldLog = File.ReadAllText(fn);
				File.WriteAllText(fn, $"{oldLog}\n\n{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff}\n{message}\n");
			} else {
				File.Create(fn);
				File.WriteAllText(fn, $"{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff}\n\n{message}\n");
			}
			MessageBox(IntPtr.Zero, $"Saved At: {fn}", title, 0x00000030);
		}
	}
}
