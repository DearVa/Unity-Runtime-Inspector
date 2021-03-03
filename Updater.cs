using System;
using UnityEngine;

namespace InGameDebugger {
	public class Updater : MonoBehaviour {
		public delegate void action();
		public action Action;

		void Update() {
			Action?.Invoke();
		}
	}
}
