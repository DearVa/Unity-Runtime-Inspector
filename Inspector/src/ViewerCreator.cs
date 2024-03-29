﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RuntimeInspector {
	internal class ViewerCreator : MonoBehaviour {
		public static Font font;

		public const float Size = 40f, TopBtnWidth = 150f, TopBtnHeight = 50f;
		public const int FontSize = 25, TopBtnFontSize = 30;

		private static GameObject creator;
		private GameObject viewerCanvas;
		private float timeScale;
		private bool paused;

		public static void Create() {
			try {
				if (creator == null) {
					creator = new GameObject("[SceneViewer]");
					creator.transform.SetAsFirstSibling();
					creator.AddComponent<SceneViewerFlag>();
					creator.AddComponent<ViewerCreator>();
				}
			} catch (Exception e) {
				Utils.LogError(e.ToString(), "Create Error");
			}
		}

		private void Start() {
			try {
				font = Font.CreateDynamicFontFromOSFont("Arial", 50);

				var eventSystem = new GameObject("EventSystem");
				eventSystem.transform.SetParent(transform);
				eventSystem.AddComponent<SceneViewerFlag>();
				eventSystem.AddComponent<EventSystem>();
				eventSystem.AddComponent<StandaloneInputModule>();

				viewerCanvas = new GameObject("SceneViewerCanvas");
				viewerCanvas.transform.SetParent(transform);
				viewerCanvas.AddComponent<SceneViewerFlag>();
				var viewerCanvasC = viewerCanvas.AddComponent<Canvas>();
				viewerCanvasC.renderMode = RenderMode.ScreenSpaceOverlay;
				viewerCanvasC.sortingOrder = 29999;
				viewerCanvas.AddComponent<CanvasScaler>();
				viewerCanvas.AddComponent<GraphicRaycaster>();

				var panel = new GameObject("Panel");
				panel.AddComponent<SceneViewerFlag>();
				panel.transform.SetParent(viewerCanvas.transform);
				var panelR = panel.AddComponent<RectTransform>();
				panelR.anchorMin = new Vector2(0, 0);
				panelR.anchorMax = new Vector2(1, 1);
				panelR.pivot = new Vector2(0, 1);
				panelR.anchoredPosition = new Vector2(0, -TopBtnHeight);
				panelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				panelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - TopBtnHeight);
				panel.SetActive(false);
				panel.AddComponent<SceneViewer>();

				var closeBtn = new GameObject("CloseBtn");
				var closeText = new GameObject("CloseText");
				closeBtn.AddComponent<SceneViewerFlag>();
				closeBtn.transform.SetParent(viewerCanvas.transform);
				closeBtn.AddComponent<Image>();
				var closeBtnR = closeBtn.GetComponent<RectTransform>();
				closeBtnR.anchorMin = new Vector2(0, 1);
				closeBtnR.anchorMax = new Vector2(0, 1);
				closeBtnR.pivot = new Vector2(0, 1);
				closeBtnR.anchoredPosition = Vector2.zero;
				closeBtnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TopBtnWidth);
				closeText.AddComponent<SceneViewerFlag>();
				closeText.transform.SetParent(closeBtn.transform);
				var closeTextT = closeText.AddComponent<Text>();
				closeBtn.AddComponent<Button>().onClick.AddListener(() => {
					panel.SetActive(!panel.activeSelf);
					closeTextT.text = panel.activeSelf ? "Hide" : "Show";
				});

				closeTextT.text = "Show";
				closeTextT.font = font;
				closeTextT.fontSize = TopBtnFontSize;
				closeTextT.color = Color.black;
				closeTextT.alignment = TextAnchor.MiddleCenter;
				closeText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				closeText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TopBtnWidth);

				var pauseBtn = new GameObject("PauseBtn");
				var pauseText = new GameObject("PauseText");
				pauseBtn.AddComponent<SceneViewerFlag>();
				pauseBtn.transform.SetParent(viewerCanvas.transform);
				pauseBtn.AddComponent<Image>();
				var pauseBtnR = pauseBtn.GetComponent<RectTransform>();
				pauseBtnR.anchorMin = new Vector2(0, 1);
				pauseBtnR.anchorMax = new Vector2(0, 1);
				pauseBtnR.pivot = new Vector2(0, 1);
				pauseBtnR.anchoredPosition = new Vector2(TopBtnWidth * 1.1f, 0);
				pauseBtnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TopBtnWidth);
				pauseText.AddComponent<SceneViewerFlag>();
				pauseText.transform.SetParent(pauseBtn.transform);
				var pauseTextT = pauseText.AddComponent<Text>();

				pauseTextT.text = "Pause";
				pauseTextT.font = font;
				pauseTextT.fontSize = TopBtnFontSize;
				pauseTextT.color = Color.black;
				pauseTextT.alignment = TextAnchor.MiddleCenter;
				var pauseTextR = pauseText.GetComponent<RectTransform>();
				pauseTextR.anchoredPosition = Vector2.zero;
				pauseTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TopBtnWidth);

				var consoleBtn = Instantiate(pauseBtn, viewerCanvas.transform);
				consoleBtn.name = "ConsoleBtn";
				consoleBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(TopBtnWidth * 2.2f, 0);
				consoleBtn.GetComponentInChildren<Text>().text = "Console";

				var console = new GameObject("Console");
				console.transform.SetParent(viewerCanvas.transform);
				console.AddComponent<Console>();
				console.SetActive(false);

				pauseBtn.AddComponent<Button>().onClick.AddListener(() => {
					paused = !paused;
					if (paused) {
						timeScale = Time.timeScale;
						Time.timeScale = 0;
					} else {
						Time.timeScale = timeScale;
					}
					pauseTextT.text = paused ? "Resume" : "Pause";
				});
				consoleBtn.AddComponent<Button>().onClick.AddListener(() => {
					console.SetActive(!console.activeSelf);
					consoleBtn.GetComponentInChildren<Text>().text = console.activeSelf ? "Close" : "Console";
				});
			} catch (Exception e) {
				Utils.LogError(e.ToString(), "Error in ViewerCreator Start");
			}
		}
	}
}
