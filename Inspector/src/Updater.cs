using System;
using UnityEngine;

namespace RuntimeInspector {
	internal class Updater : MonoBehaviour {
		public Action action;

		void Update() {
			action?.Invoke();
		}
	}
}
