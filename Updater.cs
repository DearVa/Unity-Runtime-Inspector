using System;
using UnityEngine;

public class Updater : MonoBehaviour {
	public Action Action;

	void Update() {
		Action?.Invoke();
	}
}
