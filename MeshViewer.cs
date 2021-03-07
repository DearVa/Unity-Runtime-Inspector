using UnityEngine;

namespace InGameDebugger {
	public class MeshViewer : MonoBehaviour {
		public Transform target;
		public MeshFilter MeshFilter;
		public MeshRenderer MeshRenderer;

		private Camera cam;

		void Start() {
			cam = GetComponent<Camera>();
			cam.depth = 100;
			cam.clearFlags = CameraClearFlags.Color;
			cam.backgroundColor = Color.black;
			cam.cullingMask = 1 << 31;
		}

		void Update() {
			if (Input.GetMouseButton(0)) {
				target.transform.Rotate(transform.right, Input.GetAxis("Mouse Y") * 3f, Space.World);
				target.transform.Rotate(transform.up, -Input.GetAxis("Mouse X") * 3f, Space.World);
			}
			var scroll = Input.GetAxis("Mouse ScrollWheel");
			if (scroll < 0) {
				if (cam.fieldOfView <= 100)
					cam.fieldOfView += 2;
			} else if (scroll > 0) {
				if (cam.fieldOfView > 40)
					cam.fieldOfView -= 2;
			}
		}

		public void ViewMesh(Mesh mesh, Material material) {
			MeshFilter.mesh = mesh;
			MeshRenderer.material = material;
			gameObject.SetActive(true);
		}
	}
}
