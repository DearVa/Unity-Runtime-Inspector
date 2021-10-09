using UnityEngine;

namespace RuntimeInspector {
	internal class SceneViewerFlag : MonoBehaviour {
		private void Awake() {
			DontDestroyOnLoad(gameObject);
		}
	}
}