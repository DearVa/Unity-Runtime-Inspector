using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace InGameDebugger {
	public class EnumEditor : MonoBehaviour {
		public float size;
		public int fontSize;
		public Type enumType;
		public Func<int> get;
		public Action<int> set;

		private Dropdown dropD;
		private readonly List<int> enums = new List<int>();

		void Start() {
			if (GetComponent<SceneViewerFlag>() == null) {
				gameObject.AddComponent<SceneViewerFlag>();
			}

			var font = Font.CreateDynamicFontFromOSFont("Arial", 50);

			var drop = new GameObject("DropDown");
			drop.AddComponent<SceneViewerFlag>();
			drop.transform.SetParent(transform);
			drop.AddComponent<Image>();
			dropD = drop.AddComponent<Dropdown>();

			var dropLabel = new GameObject("DropLabel");
			dropLabel.AddComponent<SceneViewerFlag>();
			dropLabel.transform.SetParent(drop.transform);
			var dropLabelT = dropLabel.AddComponent<Text>();
			dropLabelT.font = font;
			dropLabelT.fontSize = fontSize;
			dropLabelT.supportRichText = false;
			dropLabelT.color = Color.black;
			dropLabelT.alignment = TextAnchor.MiddleCenter;
			dropD.captionText = dropLabelT;
			var dropLabelR = dropLabel.GetComponent<RectTransform>();
			dropLabelR.anchoredPosition = Vector2.zero;
			dropLabelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 0.8f);
			dropLabelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			
			var template = new GameObject("Template");
			template.AddComponent<SceneViewerFlag>();
			template.transform.SetParent(drop.transform);
			template.AddComponent<Image>();
			var templateS = dropLabel.AddComponent<ScrollRect>();
			templateS.horizontal = false;
			var templateR = template.GetComponent<RectTransform>();
			templateR.anchorMin = Vector2.zero;
			templateR.anchorMax = new Vector2(1, 0);
			templateR.pivot = new Vector2(0.5f, 1);
			templateR.anchoredPosition = Vector2.zero;
			templateR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
			dropD.template = templateR;

			var values = Enum.GetValues(enumType);
			Array.Sort(values);
			int? last = null;
			for (int i = 0; i < values.Length; i++) {
				if ((int)values.GetValue(i) != last) {
					last = (int)values.GetValue(i);
					enums.Add((int)values.GetValue(i));
					dropD.options.Add(new Dropdown.OptionData(values.GetValue(i).ToString()));
				}
			}

			var viewPort = new GameObject("Viewport");
			viewPort.AddComponent<SceneViewerFlag>();
			viewPort.transform.SetParent(template.transform);
			viewPort.AddComponent<Image>();
			viewPort.AddComponent<Mask>();
			var viewPortR = viewPort.GetComponent<RectTransform>();
			viewPortR.anchorMin = Vector2.zero;
			viewPortR.anchorMax = Vector2.one;
			viewPortR.pivot = new Vector2(0, 1);
			viewPortR.anchoredPosition = Vector2.zero;
			viewPortR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropD.options.Count * size * 0.8f);
			viewPortR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
			templateS.viewport = viewPortR;

			var content = new GameObject("Content");
			content.AddComponent<SceneViewerFlag>();
			content.transform.SetParent(viewPort.transform);
			var contentR = content.AddComponent<RectTransform>();
			contentR.anchorMin = new Vector2(0, 1);
			contentR.anchorMax = Vector2.one;
			contentR.pivot = new Vector2(0.5f, 1);
			contentR.anchoredPosition = Vector2.zero;
			contentR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 0.8f);
			contentR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			templateS.content = contentR;

			var item = new GameObject("Item");
			item.AddComponent<SceneViewerFlag>();
			item.transform.SetParent(content.transform);
			item.AddComponent<Toggle>();
			var itemR = item.GetComponent<RectTransform>();
			itemR.anchorMin = new Vector2(0, 0.5f);
			itemR.anchorMax = new Vector2(1, 0.5f);
			itemR.anchoredPosition = Vector2.zero;
			itemR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 0.8f);
			itemR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);

			var itemLabel = new GameObject("ItemLabel");
			itemLabel.AddComponent<SceneViewerFlag>();
			itemLabel.transform.SetParent(item.transform);
			var itemLabelT = itemLabel.AddComponent<Text>();
			itemLabelT.font = font;
			itemLabelT.fontSize = fontSize;
			itemLabelT.supportRichText = false;
			itemLabelT.color = Color.black;
			itemLabelT.alignment = TextAnchor.MiddleCenter;
			dropD.itemText = itemLabelT;
			var itemLabelR = itemLabel.GetComponent<RectTransform>();
			itemLabelR.anchorMin = Vector2.zero;
			itemLabelR.anchorMax = Vector2.one;
			itemLabelR.anchoredPosition = Vector2.zero;
			itemLabelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 0.8f);
			itemLabelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);

			dropLabelR.anchoredPosition = Vector2.zero;
			dropLabelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);

			var dropR = drop.GetComponent<RectTransform>();
			dropR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 0.8f);
			dropR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			dropR.anchoredPosition = Vector2.zero;

			template.SetActive(false);

			Update();

			dropD.onValueChanged.AddListener(new UnityAction<int>((index) => {
				set(enums[index]);
			}));
		}

		private void Update() {
			int i = get();
			dropD.value = enums.IndexOf(i);
		}
	}
}