using System;
using UnityEngine;
using System.IO;

namespace InGameDebugger {
	public static class Utils {
		public static void LogError(string message, string title) {
			var fn = $"{Application.persistentDataPath}/{DateTime.Now:yyyy-MM-dd}.txt";
			if (File.Exists(fn)) {
				var oldLog = File.ReadAllText(fn);
				File.WriteAllText(fn, $"{oldLog}\n\n{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff}\n{message}\n");
			} else {
				File.Create(fn);
				File.WriteAllText(fn, $"{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff}\n\n{message}\n");
			}
			ShowAndroidToastMessage($"{title}\nSaved At: {fn}");
		}

		public static void ShowAndroidToastMessage(string message) {
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			if (unityActivity != null) {
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
					AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
					toastObject.Call("show");
				}));
			}
		}
	}
}
