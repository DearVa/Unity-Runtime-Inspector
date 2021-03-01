using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;
using Mono.CSharp;

namespace InGameDebugger {
	public class InGameConsole : MonoBehaviour {
		public float size;

		private InputField codeInputI;
		private Text outputT;
		private Evaluator evaluator;

		void Start() {
			try {
				if (GetComponent<SceneViewerFlag>() == null) {
					gameObject.AddComponent<SceneViewerFlag>();
				}

				if (GetComponent<RectTransform>() == null) {
					gameObject.AddComponent<RectTransform>();
				}

				gameObject.AddComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 0.3f);

				var font = ViewerCreater.font;

				var thisR = GetComponent<RectTransform>();
				thisR.anchorMin = new Vector2(0.5f, 0);
				thisR.anchorMax = new Vector2(0.5f, 0);
				thisR.pivot = new Vector2(0.5f, 0);
				thisR.anchoredPosition = Vector2.zero;
				thisR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				thisR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - 100);

				var codeInput = new GameObject("CodeInput");
				codeInput.AddComponent<SceneViewerFlag>();
				codeInput.transform.SetParent(transform);
				codeInput.AddComponent<Image>().color = Color.white;
				var codeInputR = codeInput.GetComponent<RectTransform>();
				codeInputR.anchorMin = Vector2.zero;
				codeInputR.anchorMax = Vector2.right;
				codeInputR.pivot = new Vector2(0.5f, 0);
				codeInputR.anchoredPosition = Vector2.zero;
				codeInputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				codeInputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 0.6f);
				var codeInputText = new GameObject("Text");
				codeInputText.transform.SetParent(codeInput.transform);
				var codeInputTextT = codeInputText.AddComponent<Text>();
				codeInputTextT.font = font;
				codeInputTextT.fontSize = 30;
				codeInputTextT.color = Color.black;
				var codeInputTextR = codeInputText.GetComponent<RectTransform>();
				codeInputTextR.anchorMin = Vector2.zero;
				codeInputTextR.anchorMax = Vector2.one;
				codeInputTextR.pivot = new Vector2(0.5f, 0.5f);
				codeInputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width - 20);
				codeInputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 0.6f - 20);
				codeInputTextR.anchoredPosition = Vector2.zero;

				var output = Instantiate(codeInput, transform);
				output.name = "Output";
				outputT = output.GetComponentInChildren<Text>();
				outputT.text = "Output";
				var outputR = output.GetComponent<RectTransform>();
				outputR.anchorMin = Vector2.up;
				outputR.anchorMax = Vector2.one;
				outputR.pivot = new Vector2(0.5f, 1);
				outputR.anchoredPosition = new Vector2(0, -10);
				outputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - size * 0.6f - 120);

				codeInputTextT.supportRichText = false;
				codeInputI = codeInput.AddComponent<InputField>();
				codeInputI.textComponent = codeInputTextT;

				var writer = new StringWriter();
				var printer = new StreamReportPrinter(writer);
				var compilerContext = new CompilerContext(new CompilerSettings(), printer);
				evaluator = new Evaluator(compilerContext);
				evaluator.ReferenceAssembly(Assembly.Load("UnityEngine.CoreModule"));
				evaluator.ReferenceAssembly(Assembly.Load("UnityEngine.UI"));
				evaluator.Run("using System;using UnityEngine;using UnityEngine.UI;using UnityEngine.Events;using System.Reflection;using System.Collections;using System.Collections.Generic;");
				codeInputI.onEndEdit.AddListener(new UnityAction<string>((str) => {
					if (str == "") {
						return;
					}
					try {
						var method = evaluator.Compile(str);
						writer.Flush();
						var builder = writer.GetStringBuilder();
						var s = builder.ToString();
						builder.Clear();
						if (printer.ErrorsCount > 0) {
							outputT.text = $"<color=#ff0000ff>{s}</color>";
							return;
						}
						object result = null;
						method.Invoke(ref result);
						if (result == null) {
							result = "Done";
						}
						outputT.text = $"{str}\n<color=#00aa00ff>{result}</color>";
					} catch (Exception e) {
						outputT.text = $"<color=#ff0000ff>{e.Message}</color>";
					}
				}));
			} catch (Exception e) {
				Utils.MessageBoxError(e.ToString(), "Error in InGameConsole Start");
			}
		}
	}
}