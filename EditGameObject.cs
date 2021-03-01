using System.Collections.Generic;
using UnityEngine;

namespace InGameDebugger {
	public class EditGameObject {
		public GameObject gameObject;
		public List<EditGameObject> children = new List<EditGameObject>();
		public EditGameObject father;
		public int level;
		public bool open;
		public bool highlight;

		public EditGameObject(GameObject gameObject, int level, EditGameObject father) {
			this.gameObject = gameObject;
			this.level = level;
			this.father = father;
		}

		public void SwitchOpen() {
			open = !open;
		}

		public override bool Equals(object obj) {
			if (obj is EditGameObject @object) {
				return gameObject.Equals(@object.gameObject);
			}
			return false;
		}

		public override int GetHashCode() {
			return gameObject.GetHashCode();
		}
	}
}