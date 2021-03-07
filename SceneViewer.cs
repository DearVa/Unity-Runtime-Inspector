using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;

namespace InGameDebugger {
	public class SceneViewer : MonoBehaviour {
		private readonly List<EditGameObject> editGameObjects = new List<EditGameObject>();
		private GameObject[] editLines;
		private GameObject inspectorObj;

		private GameObject topPanel;
		private GameObject refreshBtn;

		private GameObject scrollViewH;
		private GameObject viewportH;
		private GameObject contentH;

		private GameObject scrollViewI;
		private GameObject viewportI;
		private GameObject contentI;
		private GameObject backBtn;

		private MeshViewer meshViewer;
		private AudioSource mAudioSource;

		private bool find;
		private float offset;
		private GameObject comBtn;

		private void Start() {
			try {
				topPanel = new GameObject("TopPanel");
				topPanel.AddComponent<SceneViewerFlag>();
				topPanel.transform.SetParent(transform);
				topPanel.AddComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
				var topPanelR = topPanel.GetComponent<RectTransform>();
				topPanelR.anchorMin = new Vector2(0, 1);
				topPanelR.anchorMax = Vector2.one;
				topPanelR.pivot = new Vector2(0, 1);
				topPanelR.anchoredPosition = new Vector2(0, ViewerCreater.topBtnHeight);
				topPanelR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);

				refreshBtn = new GameObject("RefreshBtn");
				refreshBtn.AddComponent<SceneViewerFlag>();
				refreshBtn.transform.SetParent(topPanel.transform);
				refreshBtn.AddComponent<Image>();
				refreshBtn.AddComponent<Button>();
				var refreshBtnR = refreshBtn.GetComponent<RectTransform>();
				refreshBtnR.anchorMin = Vector2.one;
				refreshBtnR.anchorMax = Vector2.one;
				refreshBtnR.pivot = Vector2.one;
				refreshBtnR.anchoredPosition = Vector2.zero;
				refreshBtnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ViewerCreater.topBtnWidth);

				var refreshText = new GameObject("RefreshText");
				refreshText.AddComponent<SceneViewerFlag>();
				refreshText.transform.SetParent(refreshBtn.transform);
				var refreshTextT = refreshText.AddComponent<Text>();
				refreshTextT.text = "Refresh";
				refreshTextT.font = ViewerCreater.font;
				refreshTextT.fontSize = ViewerCreater.topBtnFontSize;
				refreshTextT.color = Color.black;
				refreshTextT.alignment = TextAnchor.MiddleCenter;
				var refreshTextR = refreshText.GetComponent<RectTransform>();
				refreshTextR.anchoredPosition = Vector2.zero;
				refreshTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ViewerCreater.topBtnWidth);

				var findBtn = Instantiate(refreshBtn, topPanel.transform);
				findBtn.name = "FindBtn";
				findBtn.GetComponentInChildren<Text>().text = "Capture";
				findBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-ViewerCreater.topBtnWidth * 1.1f, 0);
				findBtn.GetComponent<Button>().onClick.AddListener(new UnityAction(() => {
					scrollViewH.SetActive(false);
					find = true;
				}));
				refreshBtn.GetComponent<Button>().onClick.AddListener(new UnityAction(RefreshGameObjects));

				scrollViewH = new GameObject("ScrollViewH");
				scrollViewH.AddComponent<SceneViewerFlag>();
				scrollViewH.transform.SetParent(transform);
				var scrollViewHR = scrollViewH.AddComponent<RectTransform>();
				scrollViewHR.anchorMin = Vector2.zero;
				scrollViewHR.anchorMax = Vector3.one;
				scrollViewHR.pivot = new Vector2(0.5f, 0);
				scrollViewHR.anchoredPosition = Vector2.zero;
				scrollViewHR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				scrollViewHR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - 100);
				var scrollViewHS = scrollViewH.AddComponent<ScrollRect>();
				scrollViewHS.onValueChanged.AddListener(_ => RefreshView());
				scrollViewHS.movementType = ScrollRect.MovementType.Clamped;
				scrollViewHS.decelerationRate = 0.5f;

				viewportH = new GameObject("ViewportH");
				viewportH.AddComponent<SceneViewerFlag>();
				viewportH.transform.SetParent(scrollViewH.transform);
				viewportH.AddComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
				var viewportHR = viewportH.GetComponent<RectTransform>();
				viewportHR.anchorMin = Vector2.zero;
				viewportHR.anchorMax = Vector2.one;
				viewportHR.pivot = new Vector2(0, 1);
				viewportHR.anchoredPosition = Vector2.zero;
				viewportHR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				viewportHR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - 100);
				viewportH.AddComponent<Mask>();

				contentH = new GameObject("ContentH");
				contentH.AddComponent<SceneViewerFlag>();
				contentH.transform.SetParent(viewportH.transform);
				var contentHR = contentH.AddComponent<RectTransform>();
				contentHR.anchorMin = new Vector2(0, 1);
				contentHR.anchorMax = Vector2.one;
				contentHR.pivot = new Vector2(0, 1);
				contentHR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				scrollViewHS.viewport = viewportH.GetComponent<RectTransform>();
				scrollViewHS.content = contentH.GetComponent<RectTransform>();

				scrollViewI = new GameObject("ScrollViewI");
				scrollViewI.AddComponent<SceneViewerFlag>();
				scrollViewI.transform.SetParent(transform);
				var scrollViewIR = scrollViewI.AddComponent<RectTransform>();
				scrollViewIR.anchorMin = Vector2.zero;
				scrollViewIR.anchorMax = new Vector2(1, 0);
				scrollViewIR.pivot = new Vector2(0.5f, 0);
				scrollViewIR.anchoredPosition = Vector2.zero;
				scrollViewIR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				scrollViewIR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - 100);
				var scrollViewIS = scrollViewI.AddComponent<ScrollRect>();
				scrollViewIS.movementType = ScrollRect.MovementType.Clamped;
				scrollViewIS.decelerationRate = 0.5f;

				scrollViewI.SetActive(false);

				viewportI = new GameObject("ViewportI");
				viewportI.AddComponent<SceneViewerFlag>();
				viewportI.transform.SetParent(scrollViewI.transform);
				viewportI.AddComponent<Image>();
				viewportI.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
				var viewportIR = viewportI.GetComponent<RectTransform>();
				viewportIR.anchorMin = Vector2.zero;
				viewportIR.anchorMax = Vector2.one;
				viewportIR.pivot = new Vector2(0, 1);
				viewportIR.anchoredPosition = Vector2.zero;
				viewportIR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				viewportIR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - 100);
				viewportI.AddComponent<Mask>();

				contentI = new GameObject("ContentI");
				contentI.AddComponent<SceneViewerFlag>();
				contentI.transform.SetParent(viewportI.transform);
				var contentIR = contentI.AddComponent<RectTransform>();
				contentIR.anchorMin = new Vector2(0, 1);
				contentIR.anchorMax = Vector2.one;
				contentIR.pivot = new Vector2(0, 1);
				contentIR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				scrollViewIS.viewport = viewportI.GetComponent<RectTransform>();
				scrollViewIS.content = contentI.GetComponent<RectTransform>();

				backBtn = new GameObject("BackBtn");
				backBtn.AddComponent<SceneViewerFlag>();
				backBtn.transform.SetParent(scrollViewI.transform);
				backBtn.AddComponent<Image>();
				backBtn.AddComponent<Button>();
				var backBtnR = backBtn.GetComponent<RectTransform>();
				backBtnR.anchorMin = Vector2.one;
				backBtnR.anchorMax = Vector2.one;
				backBtnR.pivot = Vector2.one;
				backBtnR.anchoredPosition = new Vector2(0, 100);
				backBtnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 310);

				var backText = new GameObject("BackText");
				backText.AddComponent<SceneViewerFlag>();
				backText.transform.SetParent(backBtn.transform);
				var backTextT = backText.AddComponent<Text>();
				backTextT.text = "Back";
				backTextT.font = ViewerCreater.font;
				backTextT.fontSize = ViewerCreater.topBtnFontSize;
				backTextT.color = Color.black;
				backTextT.alignment = TextAnchor.MiddleCenter;
				backText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				backText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 310);
				backBtn.GetComponent<Button>().onClick.AddListener(() => {
					scrollViewI.SetActive(false);
					scrollViewH.SetActive(true);
				});

				var meshViewerObj = new GameObject("MeshViewer");
				meshViewerObj.transform.parent = transform;
				meshViewerObj.transform.position = new Vector3(1000, 1000, 990);
				meshViewerObj.AddComponent<Camera>();
				meshViewer = meshViewerObj.AddComponent<MeshViewer>();
				var light = new GameObject("Light");
				light.transform.parent = meshViewerObj.transform;
				light.AddComponent<Light>().type = LightType.Directional;
				var meshObj = new GameObject("Mesh");
				meshObj.transform.parent = meshViewerObj.transform;
				meshObj.transform.position = new Vector3(1000, 1000, 1000);
				meshObj.layer = 31;
				meshViewer.target = meshObj.transform;
				meshViewer.MeshFilter = meshObj.AddComponent<MeshFilter>();
				meshViewer.MeshRenderer = meshObj.AddComponent<MeshRenderer>();
				meshViewerObj.SetActive(false);

				mAudioSource = gameObject.AddComponent<AudioSource>();
				mAudioSource.playOnAwake = false;
				mAudioSource.loop = true;

				InitHierarchy();
				RefreshGameObjects();
				RefreshView();
			} catch (Exception e) {
				Utils.LogError(e.ToString(), "Error in SceneViewer Start");
			}
}

		private void RefreshView() {
			float y = 0;
			int num = 0, j = 0;
			float maxWidth = Screen.width;
			for (int i = 0; i < editGameObjects.Count; i++) {
				var edit = editGameObjects[i];
				if (IsLineShow(edit)) {
					if (y + ViewerCreater.size >= contentH.GetComponent<RectTransform>().anchoredPosition.y) {
						if (j < editLines.Length) {
							num++;
							editLines[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(edit.level * ViewerCreater.size, -y);
							var editline = editLines[j].GetComponent<EditLine>();
							editline.Set(edit);
							float width = editline.nameText.GetComponent<RectTransform>().rect.width + (edit.level + 1.5f) * ViewerCreater.size;
							if (width > maxWidth) {
								maxWidth = width;
							}
							if (!editLines[j].activeSelf) {
								editLines[j].SetActive(true);
							}
							j++;
						} else {
							break;
						}
					}
					y += ViewerCreater.size;
				}
			}
			for (; num < editLines.Length; num++) {
				editLines[num].SetActive(false);
			}
			foreach (var edit in editLines) {
				if (edit.activeSelf) {
					var editline = edit.GetComponent<EditLine>();
					editline.funcBtn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth - editline.editGameObject.level * ViewerCreater.size);
				}
			}
			contentH.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
			contentH.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
		}

		private bool IsLineShow(EditGameObject editline) {
			while (editline.father != null) {
				editline = editline.father;
				if (!editline.open) {
					return false;
				}
			}
			return true;
		}

		GameObject AddPropText(string propTextName) {
			var propText = new GameObject(comBtn.name);
			propText.transform.SetParent(comBtn.transform);
			propText.AddComponent<SceneViewerFlag>();
			var propTextT = propText.AddComponent<Text>();
			propTextT.text = $"{propTextName}:";
			propTextT.font = ViewerCreater.font;
			propTextT.fontSize = ViewerCreater.fontSize;
			propTextT.color = Color.black;
			propTextT.alignment = TextAnchor.MiddleLeft;
			var propTextR = propText.GetComponent<RectTransform>();
			propTextR.anchorMin = new Vector2(0, 1);
			propTextR.anchorMax = new Vector2(0, 1);
			propTextR.pivot = new Vector2(0, 1);
			propTextR.anchoredPosition = new Vector2(ViewerCreater.size * 0.6f, offset);
			propTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width - ViewerCreater.size * 0.6f);
			propTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size * 0.8f);
			return propText;
		}

		GameObject AddGroupText(string groupTextName) {
			var g = AddPropText(groupTextName);
			g.GetComponent<Text>().fontStyle = FontStyle.Bold;
			g.GetComponent<RectTransform>().anchoredPosition = new Vector2(ViewerCreater.size * 0.3f, offset);
			offset -= ViewerCreater.size;
			return g;
		}

		void AddVector3Editor(GameObject parent, Func<Vector3> get, Action<Vector3> set) {
			var vector3Editor = new GameObject($"{parent.name} Editor");
			vector3Editor.transform.SetParent(parent.transform);
			var v3R = vector3Editor.AddComponent<RectTransform>();
			v3R.anchorMin = Vector2.one;
			v3R.anchorMax = Vector2.one;
			v3R.pivot = Vector2.one;
			v3R.anchoredPosition = new Vector2(-20, 0);
			v3R.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			v3R.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size * 3);
			var v3V = vector3Editor.AddComponent<Vector3Editor>();
			v3V.get = get;
			v3V.set = set;
			offset -= ViewerCreater.size * 3.2f;
		}

		void AddBoolEditor(GameObject parent, Func<bool> get, Action<bool> set) {
			var boolEditor = new GameObject($"{parent.name} Editor");
			boolEditor.transform.SetParent(parent.transform);
			var v3R = boolEditor.AddComponent<RectTransform>();
			v3R.anchorMin = Vector2.one;
			v3R.anchorMax = Vector2.one;
			v3R.pivot = Vector2.one;
			v3R.anchoredPosition = new Vector2(-20, 0);
			v3R.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ViewerCreater.size * 0.8f);
			v3R.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size * 0.8f);
			var v3B = boolEditor.AddComponent<BoolEditor>();
			v3B.get = get;
			v3B.set = set;
			offset -= ViewerCreater.size;
		}

		void AddEnumEditor(GameObject parent, Type enumType, Func<int> get, Action<int> set) {
			var enumEditor = new GameObject($"{parent.name} Editor");
			enumEditor.transform.SetParent(parent.transform);
			var enumR = enumEditor.AddComponent<RectTransform>();
			enumR.anchorMin = Vector2.one;
			enumR.anchorMax = Vector2.one;
			enumR.pivot = Vector2.one;
			enumR.anchoredPosition = new Vector2(-20, 0);
			enumR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			enumR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
			var enumE = enumEditor.AddComponent<EnumEditor>();
			enumE.enumType = enumType;
			enumE.get = get;
			enumE.set = set;
			offset -= ViewerCreater.size * 1.2f;
		}

		void AddFloatEditor(GameObject parent, Func<float> get, Action<float> set, float min, float max) {
			var floatEditor = new GameObject($"{parent.name} Editor");
			floatEditor.transform.SetParent(parent.transform);
			var floatR = floatEditor.AddComponent<RectTransform>();
			floatR.anchorMin = Vector2.one;
			floatR.anchorMax = Vector2.one;
			floatR.pivot = Vector2.one;
			floatR.anchoredPosition = new Vector2(-20, 0);
			floatR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			floatR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
			var floatV = floatEditor.AddComponent<FloatEditor>();
			floatV.get = get;
			floatV.set = set;
			floatV.minimum = min;
			floatV.maximum = max;
			offset -= ViewerCreater.size * 1.2f;
		}

		void AddFloatEditor(GameObject parent, Func<float> get, Action<float> set) {
			AddFloatEditor(parent, get, set, float.NegativeInfinity, float.PositiveInfinity);
		}

		void AddIntEditor(GameObject parent, Func<int> get, Action<int> set, float min, float max) {
			var intEditor = new GameObject($"{parent.name} Editor");
			intEditor.transform.SetParent(parent.transform);
			var intR = intEditor.AddComponent<RectTransform>();
			intR.anchorMin = Vector2.one;
			intR.anchorMax = Vector2.one;
			intR.pivot = Vector2.one;
			intR.anchoredPosition = new Vector2(-20, 0);
			intR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			intR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
			var intV = intEditor.AddComponent<IntEditor>();
			intV.get = get;
			intV.set = set;
			intV.minimum = min;
			intV.maximum = max;
			offset -= ViewerCreater.size * 1.2f;
		}

		void AddIntEditor(GameObject parent, Func<int> get, Action<int> set) {
			AddIntEditor(parent, get, set, float.NegativeInfinity, float.PositiveInfinity);
		}

		void AddStringEditor(GameObject parent, Func<string> get, Action<string> set) {
			var stringEditor = new GameObject($"{parent.name} Editor");
			stringEditor.transform.SetParent(parent.transform);
			var stringR = stringEditor.AddComponent<RectTransform>();
			stringR.anchorMin = Vector2.one;
			stringR.anchorMax = Vector2.one;
			stringR.pivot = Vector2.one;
			stringR.anchoredPosition = new Vector2(-20, 0);
			stringR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			stringR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
			var stringV = stringEditor.AddComponent<StringEditor>();
			stringV.get = get;
			stringV.set = set;
			offset -= ViewerCreater.size * 1.2f;
		}

		void AddButton(GameObject parent, Func<string> get, Action onClick) {
			var button = new GameObject($"{parent.name} Button");
			button.transform.SetParent(parent.transform);
			var buttonR = button.AddComponent<RectTransform>();
			buttonR.anchorMin = Vector2.one;
			buttonR.anchorMax = Vector2.one;
			buttonR.pivot = Vector2.one;
			buttonR.anchoredPosition = new Vector2(-20, 0);
			buttonR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			buttonR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
			button.AddComponent<Image>();
			var buttonT = new GameObject("text");
			buttonT.transform.SetParent(button.transform);
			var buttonTR = buttonT.AddComponent<RectTransform>();
			buttonTR.anchorMin = Vector2.zero;
			buttonTR.anchorMax = Vector2.one;
			buttonTR.anchoredPosition = Vector2.zero;
			buttonTR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
			buttonTR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
			var buttonTT = buttonT.AddComponent<Text>();
			buttonTT.color = Color.black;
			buttonTT.alignment = TextAnchor.MiddleCenter;
			buttonTT.font = ViewerCreater.font;
			buttonTT.fontSize = ViewerCreater.fontSize;
			buttonT.AddComponent<Updater>().Action = () => buttonTT.text = get();
			var buttonB = button.AddComponent<Button>();
			buttonB.onClick.AddListener(() => onClick?.Invoke());
			offset -= ViewerCreater.size * 1.2f;
		}

		void ViewMesh(MeshFilter meshFilter) {
			var activeH = viewportH.activeSelf;
			var activeI = viewportI.activeSelf;
			viewportH.SetActive(false);
			viewportI.SetActive(false);
			meshViewer.ViewMesh(meshFilter.sharedMesh, inspectorObj.GetComponent<MeshRenderer>().sharedMaterial);
			backBtn.SetActive(true);
			var button = backBtn.GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => {
				meshViewer.gameObject.SetActive(false);
				viewportH.SetActive(activeH);
				viewportI.SetActive(activeI);
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => {
					scrollViewI.SetActive(false);
					scrollViewH.SetActive(true);
				});
			});
		}

		public void LoadInspector() {
			if (inspectorObj == null) {
				return;
			}
			foreach (Transform child in contentI.transform) {
				Destroy(child.gameObject);
			}
			float y = 0;
			var coms = inspectorObj.GetComponents<Component>();
			offset = -ViewerCreater.size;
			foreach (var com in coms) {
				offset = -ViewerCreater.size;
				comBtn = new GameObject(com.GetType().Name);
				comBtn.transform.SetParent(contentI.transform);
				comBtn.AddComponent<SceneViewerFlag>();
				comBtn.AddComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 0.4f);
				comBtn.AddComponent<Button>();
				var comBtnR = comBtn.GetComponent<RectTransform>();
				comBtnR.anchorMin = new Vector2(0, 1);
				comBtnR.anchorMax = new Vector2(0, 1);
				comBtnR.pivot = new Vector2(0, 1);
				comBtnR.anchoredPosition = new Vector2(0, y);
				comBtnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				comBtnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
				var comText = new GameObject("Text");
				comText.transform.SetParent(comBtn.transform);
				var comTextT = comText.AddComponent<Text>();
				comTextT.text = com.GetType().Name;
				comTextT.font = ViewerCreater.font;
				comTextT.fontStyle = FontStyle.Bold;
				comTextT.fontSize = ViewerCreater.fontSize;
				comTextT.color = Color.black;
				comTextT.alignment = TextAnchor.MiddleLeft;
				var comTextR = comText.GetComponent<RectTransform>();
				comTextR.anchoredPosition = new Vector2(ViewerCreater.size * 0.3f, 0);
				comTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				comTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
				y -= ViewerCreater.size;

				if (com is Transform transform) {
					AddVector3Editor(AddPropText("Position"), () => transform.position, v3 => transform.position = v3);
					AddVector3Editor(AddPropText("Rotation"), () => transform.rotation.eulerAngles, v3 => transform.rotation = Quaternion.Euler(v3));
					AddVector3Editor(AddPropText("Scale"), () => transform.localScale, v3 => transform.localScale = v3);
				} else if (com is MeshFilter meshFilter) {
					AddButton(AddPropText("Mesh"), () => meshFilter.sharedMesh == null ? "None" : meshFilter.sharedMesh.name, () => ViewMesh(meshFilter));
				} else if (com is MeshRenderer meshRenderer) {
					AddBoolEditor(AddPropText("enabled"), () => meshRenderer.enabled, b => meshRenderer.enabled = b);
					AddGroupText("Materials");
					var mats = meshRenderer.sharedMaterials;
					AddButton(AddPropText("Size"), () => mats.Length.ToString(), null);
					for (int i = 0; i < mats.Length; i++) {
						var mat = mats[i];
						AddButton(AddPropText($"Element {i}"), () => mat.name, null);
					}
					AddGroupText("Lighting");
					AddEnumEditor(AddPropText("Cast Shadows"), typeof(ShadowCastingMode), () => (int)meshRenderer.shadowCastingMode, i => meshRenderer.shadowCastingMode = (ShadowCastingMode)i);
					AddBoolEditor(AddPropText("Receive Shadows"), () => meshRenderer.receiveShadows, b => meshRenderer.receiveShadows = b);
					AddGroupText("Probes");
					AddEnumEditor(AddPropText("Light Probes"), typeof(LightProbeUsage), () => (int)meshRenderer.lightProbeUsage, i => meshRenderer.lightProbeUsage = (LightProbeUsage)i);
					AddEnumEditor(AddPropText("Reflection Probes"), typeof(ReflectionProbeUsage), () => (int)meshRenderer.reflectionProbeUsage, i => meshRenderer.reflectionProbeUsage = (ReflectionProbeUsage)i);
					AddButton(AddPropText("Anchor Override"), () => meshRenderer.probeAnchor == null ? "None" : meshRenderer.probeAnchor.name, null);
					AddGroupText("Addtional Settings");
					AddEnumEditor(AddPropText("Motion Vectors"), typeof(MotionVectorGenerationMode), () => (int)meshRenderer.motionVectorGenerationMode, i => meshRenderer.motionVectorGenerationMode = (MotionVectorGenerationMode)i);
					AddBoolEditor(AddPropText("Dynamic Occlusion"), () => meshRenderer.allowOcclusionWhenDynamic, b => meshRenderer.allowOcclusionWhenDynamic = b);
				} else if (com is BoxCollider boxCollider) {
					AddBoolEditor(AddPropText("enabled"), () => boxCollider.enabled, b => boxCollider.enabled = b);
					AddBoolEditor(AddPropText("Is Trigger"), () => boxCollider.isTrigger, b => boxCollider.isTrigger = b);
					AddButton(AddPropText("Material"), () => boxCollider.material == null ? "None" : boxCollider.material.name, null);
					AddVector3Editor(AddPropText("Center"), () => boxCollider.center, v3 => boxCollider.center = v3);
					AddVector3Editor(AddPropText("Size"), () => boxCollider.size, v3 => boxCollider.size = v3);
				} else if (com is CapsuleCollider capsuleCollider) {
					AddBoolEditor(AddPropText("enabled"), () => capsuleCollider.enabled, b => capsuleCollider.enabled = b);
					AddBoolEditor(AddPropText("Is Trigger"), () => capsuleCollider.isTrigger, b => capsuleCollider.isTrigger = b);
					AddButton(AddPropText("Material"), () => capsuleCollider.material == null ? "None" : capsuleCollider.material.name, null);
					AddVector3Editor(AddPropText("Center"), () => capsuleCollider.center, v3 => capsuleCollider.center = v3);
					AddFloatEditor(AddPropText("Radius"), () => capsuleCollider.radius, f => capsuleCollider.radius = f);
				} else if (com is AudioSource audioSource) {
					AddBoolEditor(AddPropText("enabled"), () => audioSource.enabled, b => audioSource.enabled = b);
					AddButton(AddPropText("AudioClip"), () => audioSource.clip == null ? "None" : audioSource.clip.name, () => {
						if (audioSource.clip == null) return;
						if (mAudioSource.clip != audioSource.clip) {
							mAudioSource.clip = audioSource.clip;
							mAudioSource.Play();
						} else if (mAudioSource.isPlaying) mAudioSource.Stop();
						else mAudioSource.Play();
					});
					AddButton(AddPropText("Output"), () => audioSource.outputAudioMixerGroup == null ? "None" : audioSource.outputAudioMixerGroup.name, null);
					AddBoolEditor(AddPropText("Mute"), () => audioSource.mute, b => audioSource.mute = b);
					AddBoolEditor(AddPropText("ByPass Effects"), () => audioSource.bypassEffects, b => audioSource.bypassEffects = b);
					AddBoolEditor(AddPropText("Bypass Listener Effects"), () => audioSource.bypassListenerEffects, b => audioSource.bypassListenerEffects = b);
					AddBoolEditor(AddPropText("Bypass Reverb Zones"), () => audioSource.bypassReverbZones, b => audioSource.bypassReverbZones = b);
					AddBoolEditor(AddPropText("Play On Awake"), () => audioSource.playOnAwake, b => audioSource.playOnAwake = b);
					AddBoolEditor(AddPropText("Loop"), () => audioSource.loop, b => audioSource.loop = b);
					AddIntEditor(AddPropText("Priority"), () => audioSource.priority, i => audioSource.priority = i, 0, 256);
					AddFloatEditor(AddPropText("Volume"), () => audioSource.volume, f => audioSource.volume = f, 0, 1);
					AddFloatEditor(AddPropText("Pitch"), () => audioSource.pitch, f => audioSource.pitch = f, -3, 3);
					AddFloatEditor(AddPropText("Stereo Pan"), () => audioSource.panStereo, f => audioSource.panStereo = f, -1, 1);
					AddFloatEditor(AddPropText("Spatial Blend"), () => audioSource.spatialBlend, f => audioSource.spatialBlend = f, 0, 1);
					AddFloatEditor(AddPropText("Reverb Zone Mix"), () => audioSource.reverbZoneMix, f => audioSource.reverbZoneMix = f, 0, 1.1f);
					AddGroupText("3D Sound Settings");
					AddFloatEditor(AddPropText("Droppler Level"), () => audioSource.dopplerLevel, f => audioSource.dopplerLevel = f, 0, 5);
					AddFloatEditor(AddPropText("Spread"), () => audioSource.spread, f => audioSource.spread = f, 0, 360);
					AddEnumEditor(AddPropText("Volume Rolloff"), typeof(AudioRolloffMode), () => (int)audioSource.rolloffMode, i => audioSource.rolloffMode = (AudioRolloffMode)i);
					AddFloatEditor(AddPropText("Min Distance"), () => audioSource.minDistance, f => audioSource.minDistance = f, 0, float.PositiveInfinity);
					AddFloatEditor(AddPropText("Max Distance"), () => audioSource.maxDistance, f => audioSource.maxDistance = f, 0, float.PositiveInfinity);
				} else {
					var type = com.GetType();
					var props = type.GetProperties();

					foreach (var prop in props) {
						try {
							var name = Regex.Replace(prop.Name, "([a-z])([A-Z])", "$1 $2");
							name = name.Substring(0, 1).ToUpper() + name.Substring(1);
							var propText = AddPropText(name);
							if (prop.PropertyType == typeof(Vector3)) {
								AddVector3Editor(propText, () => (Vector3)prop.GetValue(com), v3 => prop.SetValue(com, v3));
							} else if (prop.PropertyType == typeof(bool)) {
								AddBoolEditor(propText, () => (bool)prop.GetValue(com), b => prop.SetValue(com, b));
							} else if (prop.PropertyType == typeof(float)) {
								AddFloatEditor(propText, () => (float)prop.GetValue(com), f => prop.SetValue(com, f));
							} else if (prop.PropertyType == typeof(int)) {
								AddIntEditor(propText, () => (int)prop.GetValue(com), i => prop.SetValue(com, i));
							} else if (prop.PropertyType == typeof(string)) {
								AddStringEditor(propText, () => (string)prop.GetValue(com), s => prop.SetValue(com, s));
							} else if (prop.PropertyType.IsEnum) {
								AddEnumEditor(propText, prop.PropertyType, () => (int)prop.GetValue(com), i => prop.SetValue(com, i));
							} else if (prop.PropertyType.IsSubclassOf(typeof(UnityEvent))) {
								var ue = prop.GetValue(com) as UnityEvent;
								for (int i = 0; i < ue.GetPersistentEventCount(); i++) {
									int ii = i;
									AddButton(propText, () => $"{ue.GetPersistentTarget(ii)} : {ue.GetPersistentMethodName(ii)}", null);
								}
							} else {
								AddButton(propText, () => { 
									try { 
										return prop.GetValue(com).ToString(); 
									} catch { }
									return "Error";
								}, null);
							}
						} catch { }
					}
				}
				y += offset;
			}

			var contentIR = contentI.GetComponent<RectTransform>();
			contentIR.anchoredPosition = Vector2.zero;
			contentIR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
			contentIR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -y);
			scrollViewH.SetActive(false);
			scrollViewI.SetActive(true);
		}

		public void RefreshGameObjects() {
			contentH.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1);
			foreach (var editLine in editLines) {
				if (editLine != null) {
					editLine.SetActive(false);
				}
			}
			editGameObjects.Clear();
			string sceneName = SceneManager.GetActiveScene().name;
			foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>()) {
				if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave) {
					continue;
				}
				if (sceneName != go.gameObject.scene.name) {
					continue;
				}
				if (go.GetComponent<SceneViewerFlag>() == null && go.transform.parent == null) {
					CreateEditGameObject(go, 0, null);
				}
			}
			foreach (var obj in FindObjectsOfType<GameObject>()) {
				if (obj.GetComponent<SceneViewerFlag>() == null && obj.transform.parent == null) {
					CreateEditGameObject(obj, 0, null);
				}
			}
			contentH.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
			contentH.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size * editGameObjects.Count);
		}

		public void InitHierarchy() {
			editLines = new GameObject[Mathf.CeilToInt(Screen.height / ViewerCreater.size)];
			for (int i = 0; i < editLines.Length; i++) {
				var btn = new GameObject($"Btn{i}");
				btn.AddComponent<SceneViewerFlag>();
				btn.AddComponent<Image>();
				btn.AddComponent<Button>().onClick.AddListener(new UnityAction(() => {
					btn.GetComponent<EditLine>().SwitchOpen();
					RefreshView();
				}));
				var funBtn = new GameObject("FuncBtn");
				funBtn.AddComponent<SceneViewerFlag>();
				funBtn.AddComponent<Image>();
				funBtn.AddComponent<Button>().onClick.AddListener(new UnityAction(() => {
					inspectorObj = btn.GetComponent<EditLine>().editGameObject.gameObject;
					LoadInspector();
				}));
				var label = new GameObject("Label");
				label.AddComponent<SceneViewerFlag>();
				label.transform.SetParent(btn.transform);
				label.AddComponent<Text>();
				label.GetComponent<Text>().font = ViewerCreater.font;
				label.GetComponent<Text>().fontSize = (int)(ViewerCreater.size * 0.7f);
				label.GetComponent<Text>().color = Color.black;
				label.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
				label.GetComponent<Text>().text = "＋";
				label.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ViewerCreater.size);
				label.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
				label.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				var text = new GameObject("Text");
				text.AddComponent<SceneViewerFlag>();
				text.transform.SetParent(btn.transform);
				text.AddComponent<Text>();
				text.GetComponent<Text>().font = ViewerCreater.font;
				text.GetComponent<Text>().fontSize = (int)(ViewerCreater.size * 0.5f);
				text.GetComponent<Text>().color = Color.black;
				text.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
				text.GetComponent<Text>().text = gameObject.name;
				text.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				btn.transform.SetParent(contentH.transform);
				btn.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
				btn.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
				btn.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
				btn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ViewerCreater.size);
				btn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
				funBtn.transform.SetParent(btn.transform);
				funBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
				funBtn.GetComponent<RectTransform>().anchorMin = text.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
				funBtn.GetComponent<RectTransform>().anchorMax = text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
				funBtn.GetComponent<RectTransform>().pivot = text.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
				text.GetComponent<RectTransform>().anchoredPosition = new Vector2(ViewerCreater.size * 0.3f, 0);
				funBtn.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
				funBtn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreater.size);
				btn.AddComponent<EditLine>().Init(funBtn.GetComponent<Button>(), text.GetComponent<Text>(), label.GetComponent<Text>());
				editLines[i] = btn;
			}
		}

		private void Update() {
			if (find && Input.GetMouseButtonDown(0)) {
				find = false;
				foreach (var edit in editGameObjects) {
					edit.highlight = false;
				}
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out RaycastHit hit)) {
					foreach (var edit in editGameObjects) {
						if (edit.gameObject == hit.transform.gameObject) {
							var e = edit;
							do {
								e.open = true;
								e.highlight = true;
								e = e.father;
							} while (e != null);
							break;
						}
					}
				}
				scrollViewH.SetActive(true);
				RefreshView();
			}
		}

		public EditGameObject CreateEditGameObject(GameObject gameObject, int level, EditGameObject father) {
			var editGameObject = new EditGameObject(gameObject, level, father);
			int index = editGameObjects.IndexOf(editGameObject);
			if (index != -1) {
				editGameObjects[index] = editGameObject;
			} else {
				editGameObjects.Add(editGameObject);
			}
			foreach (Transform child in gameObject.transform) {
				editGameObject.children.Add(CreateEditGameObject(child.gameObject, level + 1, editGameObject));
			}
			return editGameObject;
		}
	}
}