using System;
using UnityEngine;

namespace InGameDebugger {
	public class Updater : MonoBehaviour {
		public Action Action;

		void Update() {
			Action?.Invoke();
		}
	}
}
