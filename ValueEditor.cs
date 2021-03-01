using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;
using System.Reflection;

namespace InGameDebugger {

	public class ValueEditor : MonoBehaviour {
		public float size;
		public int fontSize;
		public Component component;
		public PropertyInfo propInfo;

		private GameObject input;
		private float begin;
		private bool editing;

		void Start() {
			if (GetComponent<SceneViewerFlag>() == null) {
				gameObject.AddComponent<SceneViewerFlag>();
			}

			input = new GameObject("ValueInput");
			input.AddComponent<SceneViewerFlag>();
			input.transform.SetParent(transform);

			var inputText = new GameObject("ValueInputText");
			inputText.AddComponent<SceneViewerFlag>();
			inputText.transform.SetParent(input.transform);
			var inputTextT = inputText.AddComponent<Text>();
			inputTextT.font = Font.CreateDynamicFontFromOSFont("Arial", 50);
			inputTextT.fontSize = fontSize;
			inputTextT.supportRichText = false;
			inputTextT.color = Color.black;
			inputTextT.alignment = TextAnchor.MiddleCenter;
			var inputTextR = inputText.GetComponent<RectTransform>();
			inputTextR.anchoredPosition = Vector2.zero;
			inputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 0.8f);
			inputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);

			input.AddComponent<Image>();
			var inputR = input.GetComponent<RectTransform>();
			inputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 0.8f);
			inputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			inputR.anchoredPosition = Vector2.zero;
			var inputI = input.AddComponent<InputField>();
			inputI.textComponent = inputTextT;
			var down = new EventTrigger.Entry() {
				eventID = EventTriggerType.PointerDown
			};
			down.callback.AddListener((data) => {
				editing = true;
				begin = data.currentInputModule.input.mousePosition.x;
			});
			input.AddComponent<EventTrigger>().triggers.Add(down);
			var drag = new EventTrigger.Entry() {
				eventID = EventTriggerType.Drag
			};
			drag.callback.AddListener((data) => {
				try {
					var value = float.Parse(propInfo.GetValue(component, null).ToString()) + (data.currentInputModule.input.mousePosition.x - begin) / 10;
					begin = data.currentInputModule.input.mousePosition.x;
					propInfo.SetValue(component, value, null);
					input.GetComponent<InputField>().text = propInfo.GetValue(component, null).ToString();
				} catch { }
			});
			input.GetComponent<EventTrigger>().triggers.Add(drag);
			inputI.onEndEdit.AddListener(new UnityAction<string>((str) => {
				editing = false;
				try {
					var type = propInfo.PropertyType;
					propInfo.SetValue(component, Convert.ChangeType(str, type), null);
				} catch { }
			}));
		}

		void Update() {
			if (!editing) {
				try {
					input.GetComponent<InputField>().text = propInfo.GetValue(component, null).ToString();
				} catch {
					input.GetComponent<InputField>().text = "Error";
				}
			}
		}
	}
}