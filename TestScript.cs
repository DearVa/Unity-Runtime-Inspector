using UnityEngine;
using System;

namespace RuntimeInspector {

	public class TestScript : MonoBehaviour {
		public Func<string> get;
		public Action<string> set;

		private GameObject input;
		private bool editing;

		void Start() {
			transform.gameObject.layer = 0;
			ChangeChildrenLayer(transform, 0);
		}

		void ChangeChildrenLayer(Transform tf, int layer) {
			for (int i = 0; i < tf.childCount; i++) {
				var child = tf.GetChild(i);
				child.gameObject.layer = layer;
				ChangeChildrenLayer(child, layer);
			}
		}
	}
}