using UnityEngine;
namespace InGameDebugger {
	public class SceneViewerFlag : MonoBehaviour {
		private void Awake() {
			DontDestroyOnLoad(gameObject);
		}
	}
}