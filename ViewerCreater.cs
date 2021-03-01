using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace InGameDebugger {
	public class ViewerCreater : MonoBehaviour {
		public static ViewerCreater Instance;
		public GameObject viewerCanvas;
		public Font font;

		private static GameObject creater;
		private float timeScale;
		private bool paused;

		public static void Create() {
			try {
				if (creater == null) {
					creater = new GameObject("[SceneViewer]");
					creater.transform.SetAsFirstSibling();
					creater.AddComponent<SceneViewerFlag>();
					Instance = creater.AddComponent<ViewerCreater>();
				}
			} catch (Exception e) {
				Utils.MessageBoxError(e.ToString(), "Error in Create");
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
				var panelR = panel.GetComponent<RectTransform>();
				panelR.anchorMin = new Vector2(0, 1);
				panelR.anchorMax = new Vector2(0, 1);
				panelR.pivot = new Vector2(0, 1);
				panelR.anchoredPosition = new Vector2(0, -100);
				panelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				panelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - 100);
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
				closeBtnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
				closeText.AddComponent<SceneViewerFlag>();
				closeText.transform.SetParent(closeBtn.transform);
				var closeTextT = closeText.AddComponent<Text>();
				closeBtn.AddComponent<Button>().onClick.AddListener(new UnityAction(() => {
					panel.SetActive(!panel.activeSelf);
					closeTextT.text = panel.activeSelf ? "隐藏浏览器" : "显示浏览器";
				}));

				closeTextT.text = "显示浏览器";
				closeTextT.font = font;
				closeTextT.fontSize = 50;
				closeTextT.color = Color.black;
				closeTextT.alignment = TextAnchor.MiddleCenter;
				closeText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				closeText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);

				var pauseBtn = new GameObject("PauseBtn");
				var pauseText = new GameObject("PauseText");
				pauseBtn.AddComponent<SceneViewerFlag>();
				pauseBtn.transform.SetParent(viewerCanvas.transform);
				pauseBtn.AddComponent<Image>();
				var pauseBtnR = pauseBtn.GetComponent<RectTransform>();
				pauseBtnR.anchorMin = new Vector2(0, 1);
				pauseBtnR.anchorMax = new Vector2(0, 1);
				pauseBtnR.pivot = new Vector2(0, 1);
				pauseBtnR.anchoredPosition = new Vector2(350, 0);
				pauseBtnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
				pauseText.AddComponent<SceneViewerFlag>();
				pauseText.transform.SetParent(pauseBtn.transform);
				var pauseTextT = pauseText.AddComponent<Text>();

				pauseTextT.text = "暂停";
				pauseTextT.font = font;
				pauseTextT.fontSize = 50;
				pauseTextT.color = Color.black;
				pauseTextT.alignment = TextAnchor.MiddleCenter;
				var psuseTextR = pauseText.GetComponent<RectTransform>();
				psuseTextR.anchoredPosition = Vector2.zero;
				psuseTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);

				var moreBtn = Instantiate(pauseBtn, viewerCanvas.transform);
				moreBtn.name = "MoreBtn";
				moreBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(600, 0);
				moreBtn.GetComponentInChildren<Text>().text = "控制台";

				var console = new GameObject("Console");
				console.transform.SetParent(viewerCanvas.transform);
				console.AddComponent<InGameConsole>().size = 80f;
				console.SetActive(false);

				pauseBtn.AddComponent<Button>().onClick.AddListener(new UnityAction(() => {
					paused = !paused;
					if (paused) {
						timeScale = Time.timeScale;
						Time.timeScale = 0;
					} else {
						Time.timeScale = timeScale;
					}
					pauseTextT.text = paused ? "播放" : "暂停";
				}));
				moreBtn.AddComponent<Button>().onClick.AddListener(new UnityAction(() => {
					console.SetActive(!console.activeSelf);
					moreBtn.GetComponentInChildren<Text>().text = console.activeSelf ? "关闭" : "控制台";
				}));
			} catch (Exception e) {
				Utils.MessageBoxError(e.ToString(), "Error in ViewerCreater Start");
			}
		}
	}
}
