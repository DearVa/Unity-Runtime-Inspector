using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;
using System.Reflection;

namespace InGameDebugger {
	public class FloatEditor : MonoBehaviour {
		public Func<float> get;
		public Action<float> set;
		public float minimum;
		public float maximum;

		private GameObject input;
		private float begin;
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
			inputTextT.font = ViewerCreator.font;
			inputTextT.fontSize = ViewerCreator.FontSize;
			inputTextT.supportRichText = false;
			inputTextT.color = Color.black;
			inputTextT.alignment = TextAnchor.MiddleCenter;
			var inputTextR = inputText.GetComponent<RectTransform>();
			inputTextR.anchoredPosition = Vector2.zero;
			inputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreator.Size * 0.8f);
			inputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);

			input.AddComponent<Image>();
			var inputR = input.GetComponent<RectTransform>();
			inputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreator.Size * 0.8f);
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
				var value = get() + (data.currentInputModule.input.mousePosition.x - begin) / 10;
				if (value < minimum) {
					value = minimum;
				} else if (value > maximum) {
					value = maximum;
				}
				begin = data.currentInputModule.input.mousePosition.x;
				set(value);
				input.GetComponent<InputField>().text = get().ToString();
			});
			input.GetComponent<EventTrigger>().triggers.Add(drag);
			inputI.onEndEdit.AddListener(str => {
				editing = false;
				var value = float.Parse(str);
				if (value < minimum) {
					value = minimum;
				} else if (value > maximum) {
					value = maximum;
				}
				set(value);
			});
		}

		void Update() {
			if (!editing) {
				input.GetComponent<InputField>().text = get().ToString();
			}
		}
	}
}