using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace InGameDebugger {
	public static class Utils {
		[DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
		public static extern int MessageBox(IntPtr handle, string message, string title, uint type);

		public static void MessageBoxError(string message, string title) {
			MessageBox(IntPtr.Zero, message, title, 0x00000010);
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
