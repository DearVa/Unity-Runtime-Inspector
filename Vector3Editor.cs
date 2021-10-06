using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;
using System.Reflection;

namespace InGameDebugger {

	public class Vector3Editor : MonoBehaviour {
		public Func<Vector3> get;
		public Action<Vector3> set;

		private InputField xInputI, yInputI, zInputI;
		private bool xEditing, yEditing, zEditing;
		private float xBegin, yBegin, zBegin;
		private Vector3 vector3;

		void Start() {
			if (GetComponent<SceneViewerFlag>() == null) {
				gameObject.AddComponent<SceneViewerFlag>();
			}

			var xInput = new GameObject("XInput");
			xInput.AddComponent<SceneViewerFlag>();
			xInput.transform.SetParent(transform);

			var inputText = new GameObject("Text");
			inputText.AddComponent<SceneViewerFlag>();
			inputText.transform.SetParent(xInput.transform);
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

			xInput.AddComponent<Image>();
			var inputR = xInput.GetComponent<RectTransform>();
			inputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreator.Size * 0.8f);
			inputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			inputR.anchoredPosition = new Vector2(0, ViewerCreator.Size);
			xInputI = xInput.AddComponent<InputField>();
			xInputI.textComponent = inputTextT;
			xInputI.keyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;

			var xDown = new EventTrigger.Entry() {
				eventID = EventTriggerType.PointerDown
			};
			xDown.callback.AddListener((data) => {
				xEditing = true;
				xBegin = data.currentInputModule.input.mousePosition.x;
			});
			xInput.AddComponent<EventTrigger>().triggers.Add(xDown);
			var xDrag = new EventTrigger.Entry() {
				eventID = EventTriggerType.Drag
			};
			xDrag.callback.AddListener((data) => {
				vector3 = new Vector3(vector3.x + (data.currentInputModule.input.mousePosition.x - xBegin) / 10, vector3.y, vector3.z);
				set(vector3);
				xBegin = data.currentInputModule.input.mousePosition.x;
				xInput.GetComponent<InputField>().text = vector3.x.ToString();
			});
			xInput.GetComponent<EventTrigger>().triggers.Add(xDrag);
			xInputI.onEndEdit.AddListener(str => {
				xEditing = false;
				vector3 = new Vector3(float.Parse(str), vector3.y, vector3.z);
				set(vector3);
			});

			var yInput = Instantiate(xInput, transform);
			yInput.name = "YInput";
			yInput.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			var yDown = new EventTrigger.Entry() {
				eventID = EventTriggerType.PointerDown
			};
			yDown.callback.AddListener((data) => {
				yEditing = true;
				yBegin = data.currentInputModule.input.mousePosition.x;
			});
			yInput.GetComponent<EventTrigger>().triggers.Add(yDown);
			var yDrag = new EventTrigger.Entry() {
				eventID = EventTriggerType.Drag
			};
			yDrag.callback.AddListener(data => {
				vector3 = new Vector3(vector3.x, vector3.y + (data.currentInputModule.input.mousePosition.x - yBegin) / 10, vector3.z);
				set(vector3);
				yBegin = data.currentInputModule.input.mousePosition.x;
				yInputI.text = vector3.y.ToString();
			});
			yInput.GetComponent<EventTrigger>().triggers.Add(yDrag);
			yInputI = yInput.GetComponent<InputField>();
			yInputI.onEndEdit.AddListener(str => {
				yEditing = false;
				vector3 = new Vector3(vector3.x, float.Parse(str), vector3.z);
				set(vector3);
			});

			var zInput = Instantiate(xInput, transform);
			zInput.name = "ZInput";
			zInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -ViewerCreator.Size);
			var zDown = new EventTrigger.Entry() {
				eventID = EventTriggerType.PointerDown
			};
			zDown.callback.AddListener((data) => {
				zEditing = true;
				zBegin = data.currentInputModule.input.mousePosition.x;
			});
			zInput.GetComponent<EventTrigger>().triggers.Add(zDown);
			var zDrag = new EventTrigger.Entry() {
				eventID = EventTriggerType.Drag
			};
			zDrag.callback.AddListener((data) => {
				vector3 = new Vector3(vector3.x, vector3.y, vector3.z + (data.currentInputModule.input.mousePosition.x - zBegin) / 10);
				set(vector3);
				zBegin = data.currentInputModule.input.mousePosition.x;
				zInputI.text = vector3.z.ToString();
			});
			zInput.GetComponent<EventTrigger>().triggers.Add(zDrag);
			zInputI = zInput.GetComponent<InputField>();
			zInputI.onEndEdit.AddListener(str => {
				zEditing = false;
				vector3 = new Vector3(vector3.x, vector3.y, float.Parse(str));
				set(vector3);
			});
		}

		void Update() {
			try {
				vector3 = get();
				if (!xEditing) {
					xInputI.text = vector3.x.ToString();
				}
				if (!yEditing) {
					yInputI.text = vector3.y.ToString();
				}
				if (!zEditing) {
					zInputI.text = vector3.z.ToString();
				}
			} catch {
				xInputI.text = "Error";
				yInputI.text = "Error";
				zInputI.text = "Error";
			}
		}
	}
}