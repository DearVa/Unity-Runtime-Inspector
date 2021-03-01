//using FullSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace InGameDebugger {
	//public class SceneExporter : MonoBehaviour {
	//	private List<MGameObject> gameObjs = new List<MGameObject>();

	//	public void OnGUI() {
	//		if (GUI.Button(new Rect(0, 0, 200, 100), "Save")) {
	//			Save();
	//		}
	//	}

	//	private void Save() {
	//		var objs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
	//		foreach (var obj in objs) {
	//			if (obj.transform.parent != null) {
	//				continue;
	//			}
	//			gameObjs.Add(GetGameObj(obj));
	//		}

	//		if (!Directory.Exists("/storage/emulated/0/UnityScenes/")) {
	//			Directory.CreateDirectory("/storage/emulated/0/UnityScenes/");
	//		}

	//		int num = 0;
	//		string fileName;
	//		do {
	//			fileName = $"/storage/emulated/0/UnityScenes/{num++}.dat";
	//		} while (File.Exists(fileName));

	//		using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create))) {
	//			fsSerializer serializer = new fsSerializer();
	//			serializer.TrySerialize(gameObjs.GetType(), gameObjs, out fsData data).AssertSuccessWithoutWarnings();
	//			sw.Write(fsJsonPrinter.CompressedJson(data));
	//		}

	//		ShowAndroidToastMessage($"文件已保存：{fileName}");
	//	}

	//	private void ShowAndroidToastMessage(string message) {
	//		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	//		AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	//		if (unityActivity != null) {
	//			AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
	//			unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
	//				AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
	//				toastObject.Call("show");
	//			}));
	//		}
	//	}

	//	private MGameObject GetGameObj(GameObject obj) {
	//		var gameObj = new MGameObject(obj.name) {
	//			comps = GetComps(obj)
	//		};
	//		foreach (Transform child in obj.transform) {
	//			if (gameObj.childern == null) {
	//				gameObj.childern = new List<MGameObject>();
	//			}
	//			var childObj = GetGameObj(child.gameObject);
	//			gameObj.childern.Add(childObj);
	//		}
	//		return gameObj;
	//	}

	//	private List<MComponent> GetComps(GameObject obj) {
	//		var comps = new List<MComponent>();
	//		var coms = obj.GetComponents<Component>();
	//		foreach (var com in coms) {
	//			if (com is SceneExporter) {
	//				continue;
	//			}
	//			var comp = new MComponent(com.GetType());
	//			comps.Add(comp);
	//			var props = com.GetType().GetProperties();
	//			foreach (var prop in props) {
	//				try {
	//					comp.props.Add(prop.Name, Serialize(com, prop));
	//				} catch { }
	//			}
	//		}
	//		return comps;
	//	}

	//	private string Serialize(Component com, PropertyInfo prop) {
	//		fsSerializer fs = new fsSerializer();
	//		var obj = prop.GetValue(com, null);
	//		fs.TrySerialize(obj.GetType(), obj, out fsData data).AssertSuccessWithoutWarnings();
	//		return fsJsonPrinter.CompressedJson(data);
	//	}
	//}

	[Serializable]
	public class MComponent {
		public Type type;
		public Dictionary<string, string> props = new Dictionary<string, string>();

		public MComponent(Type type) {
			this.type = type;
		}
	}

	[Serializable]
	public class MGameObject {
		public string name;
		public List<MGameObject> childern = new List<MGameObject>();
		public List<MComponent> comps = new List<MComponent>();

		public MGameObject(string name) {
			this.name = name;
		}
	}
}