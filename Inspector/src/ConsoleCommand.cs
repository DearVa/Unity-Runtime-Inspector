using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeInspector {
	internal class ConsoleCommand {
		public static readonly Dictionary<string, ConsoleCommandWrapper> Commands = new Dictionary<string, ConsoleCommandWrapper>();

		private static readonly Dictionary<KeyCode, List<Updater>> Bindings = new Dictionary<KeyCode, List<Updater>>();

		public static void Initialize() {
			Register("#bind", BindCommand, 2);
			Register("#unbind", UnbindCommand, 1);
			Register("#listobj", ListObjCommand);
			Register("#highlight", HighlightCommand, 1);
			Register("#inspect", InspectCommand);
		}

		public static void Register(string command, Func<string[], object> action, int argc = 0) {
			Commands.Add(command, new ConsoleCommandWrapper(action, argc));
		}

		private static object BindCommand(string[] args) {
			var key = ParseKeyCode(args[0]);
			var method = Console.Instance.Compile(args[1]);
			var updater = Console.Instance.gameObject.AddComponent<Updater>();
			updater.action = () => {
				if (Input.GetKeyDown(key)) {
					object refObj = null;
					method.Invoke(ref refObj);
				}
			};
			if (Bindings.ContainsKey(key)) {
				Bindings[key].Add(updater);
			} else {
				Bindings.Add(key, new List<Updater>() { updater });
			}
			return null;
		}

		private static object UnbindCommand(string[] args) {
			var key = ParseKeyCode(args[0]);
			if (Bindings.ContainsKey(key)) {
				foreach (var updater in Bindings[key]) {
					Object.Destroy(updater);
				}
				Bindings.Remove(key);
			} else {
				throw new Exception("No such a binding.");
			}
			return null;
		}

		private static KeyCode ParseKeyCode(string str) {
			if (str.StartsWith("KeyCode.")) {
				str = str.Substring(8);
			}
			var key = Enum.Parse(typeof(KeyCode), str);
			if (key == null) {
				throw new ArgumentException($"KeyCode: {str} is Invalid.");
			}
			return (KeyCode)key;
		}

		private static object ListObjCommand(string[] args) {
			return null;
		}

		private static object HighlightCommand(string[] args) {
			return null;
		}

		private static object InspectCommand(string[] args) {
			return null;
		}
	}

	internal class ConsoleCommandWrapper {
		private readonly int argc;
		private readonly Func<string[], object> action;

		public ConsoleCommandWrapper(Func<string[], object> action, int argc) {
			this.action = action;
			this.argc = argc;
		}

		public object Invoke(string[] args) {
			if (args == null) {
				if (argc != 0) {
					throw new ArgumentException($"{argc} args expect, none given.");
				}
				return action.Invoke(null);
			}
			if (args.Length != argc) {
				throw new ArgumentException($"{argc} args expect, {args.Length} given.");
			}
			for (var i = 0; i < args.Length; i++) {
				args[i] = args[i].Trim();
			}
			return action.Invoke(args);
		}
	}
}
