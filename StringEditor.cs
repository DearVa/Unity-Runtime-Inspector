using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;

namespace InGameDebugger {

	public class StringEditor : MonoBehaviour {
		public Func<string> get;
		public Action<string> set;

		private GameObject input;
		private bool editing;

		void Start() {
			if (GetComponent<SceneViewerFlag>() == null) {
				gameObject.AddComponent<SceneViewerFlag>();
			}

			input = new GameObject("FloatInput");
			input.AddComponent<SceneViewerFlag>();
			input.transform.SetParent(transform);

			var inputText = new GameObject("FloatInputText");
			inputText.AddComponent<SceneViewerFlag>();
			inputText.transform.SetParent(input.transform);
			var inputTextT = inputText.AddComponent<Text>();
			inputTextT.font = ViewerCreater.font;
			inputTextT.fontSize = ViewerCreater.fontSize;
			inputTextT.supportRichText = false;
			inputTextT.color = Color.black;
			inputTextT.alignment = TextAnchor.MiddleCenter;
			var inputTextR = inputText.GetComponent<RectTransform>();
			inputTextR.anchoredPosition = Vector2.zero;
			inputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size * 0.8f);
			inputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);

			input.AddComponent<Image>();
			var inputR = input.GetComponent<RectTransform>();
			inputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size * 0.8f);
			inputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			inputR.anchoredPosition = Vector2.zero;
			var inputI = input.AddComponent<InputField>();
			inputI.textComponent = inputTextT;
			var down = new EventTrigger.Entry() {
				eventID = EventTriggerType.PointerDown
			};
			down.callback.AddListener((data) => {
				editing = true;
			});
			input.AddComponent<EventTrigger>().triggers.Add(down);
			inputI.onEndEdit.AddListener(new UnityAction<string>((str) => {
				editing = false;
				set(str);
			}));
		}

		void Update() {
			if (!editing) {
				input.GetComponent<InputField>().text = get();
			}
		}
	}
}