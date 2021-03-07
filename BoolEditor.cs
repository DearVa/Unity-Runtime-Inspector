using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace InGameDebugger {
	public class BoolEditor : MonoBehaviour {
		public Func<bool> get;
		public Action<bool> set;

		private Toggle checkT;
		private GameObject checkText;

		void Start() {
			if (GetComponent<SceneViewerFlag>() == null) {
				gameObject.AddComponent<SceneViewerFlag>();
			}

			var check = new GameObject("CheckBox");
			check.AddComponent<SceneViewerFlag>();
			check.transform.SetParent(transform);

			checkText = new GameObject("CheckText");
			checkText.AddComponent<SceneViewerFlag>();
			checkText.transform.SetParent(check.transform);
			var checkTextT = checkText.AddComponent<Text>();
			checkTextT.font = ViewerCreater.font;
			checkTextT.fontSize = ViewerCreater.fontSize;
			checkTextT.supportRichText = false;
			checkTextT.color = Color.black;
			checkTextT.alignment = TextAnchor.MiddleCenter;
			checkTextT.text = "√";
			var checkTextR = checkText.GetComponent<RectTransform>();
			checkTextR.anchoredPosition = Vector2.zero;
			checkTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size * 0.8f);
			checkTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ViewerCreater.size * 0.8f);

			check.AddComponent<Image>();
			var inputR = check.GetComponent<RectTransform>();
			inputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size * 0.8f);
			inputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ViewerCreater.size * 0.8f);
			inputR.anchoredPosition = Vector2.zero;

			checkT = check.AddComponent<Toggle>();
			Update();
			checkT.onValueChanged.AddListener(new UnityAction<bool>((b) => {
				set(b);
			}));
		}

		void Update() {
			bool isOn = get();
			checkT.isOn = isOn;
			checkText.SetActive(isOn);
		}
	}
}