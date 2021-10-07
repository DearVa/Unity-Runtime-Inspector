using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using Mono.CSharp;

namespace InGameDebugger {
	public class InGameConsole : MonoBehaviour {
		public static InGameConsole Instance;

		private InputField codeInputI;
		private Text outputT;
		private Evaluator evaluator;
		private RectTransform outputTR, viewportR;
		private readonly LinkedList<string> outputList = new LinkedList<string>();
		private readonly StringBuilder buffer = new StringBuilder();
		private readonly LinkedList<string> commandList = new LinkedList<string>();

		private StringWriter writer;
		private StreamReportPrinter printer;
		private int startIndex, endIndex, caretPosition;

		private LinkedListNode<string> NowCommand {
			set {
				nowCommand = value;
				codeInputI.text = nowCommand == null ? string.Empty : nowCommand.Value;
			}
		}

		private LinkedListNode<string> nowCommand;

		private void Update() {
			if (Input.GetKeyDown(KeyCode.Return)) {
				var str = codeInputI.text;
				if (str == string.Empty) {
					return;
				}
				try {
					var method = evaluator.Compile(str);
					writer.Flush();
					var builder = writer.GetStringBuilder();
					if (printer.ErrorsCount > 0) {
						WriteLine($"{str}\n<color=#ff0000ff>{builder}</color>");
						return;
					}
					builder.Clear();
					object result = "Done";
					method.Invoke(ref result);
					commandList.AddLast(str);
					if (commandList.Count >= 128) {
						commandList.RemoveFirst();
					}
					PlayerPrefs.SetString("InGameConsoleCommandHistory", string.Join("\n", commandList));
					WriteLine($"{str}\n<color=#00aa00ff>{result}</color>");
				} catch (Exception e) {
					WriteLine($"{str}\n<color=#ff0000ff>{e.Message}</color>");
				}
			}
			if (codeInputI.isFocused && commandList.Count > 0) {
				if (Input.GetKeyDown(KeyCode.UpArrow)) {
					NowCommand = nowCommand == null ? commandList.Last : nowCommand.Previous;
				} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
					NowCommand = nowCommand == null ? commandList.First : nowCommand.Next;
				}
			}
		}

		private void Awake() {
			try {
				Instance = this;

				if (GetComponent<SceneViewerFlag>() == null) {
					gameObject.AddComponent<SceneViewerFlag>();
				}

				if (GetComponent<RectTransform>() == null) {
					gameObject.AddComponent<RectTransform>();
				}

				var color = new Color(0.7f, 0.7f, 0.7f, 0.3f);
				gameObject.AddComponent<Image>().color = color;

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
				codeInput.AddComponent<Image>().color = color;
				var codeInputR = codeInput.GetComponent<RectTransform>();
				codeInputR.anchorMin = Vector2.zero;
				codeInputR.anchorMax = Vector2.right;
				codeInputR.pivot = new Vector2(0.5f, 0);
				codeInputR.anchoredPosition = Vector2.zero;
				codeInputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				codeInputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreator.Size);
				var codeInputText = new GameObject("Text");
				codeInputText.transform.SetParent(codeInput.transform);
				var codeInputTextT = codeInputText.AddComponent<Text>();
				codeInputTextT.font = ViewerCreator.font;
				codeInputTextT.fontSize = ViewerCreator.FontSize;
				codeInputTextT.color = Color.black;
				var codeInputTextR = codeInputText.GetComponent<RectTransform>();
				codeInputTextR.anchorMin = Vector2.zero;
				codeInputTextR.anchorMax = Vector2.one;
				codeInputTextR.pivot = new Vector2(0.5f, 0.5f);
				codeInputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width - 10);
				codeInputTextR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreator.Size - 10);
				codeInputTextR.anchoredPosition = Vector2.zero;

				var output = new GameObject("Output");
				output.transform.parent = transform;
				var outputS = output.AddComponent<ScrollRect>();
				var outputR = output.GetComponent<RectTransform>();
				outputR.anchorMin = Vector2.up;
				outputR.anchorMax = Vector2.one;
				outputR.pivot = new Vector2(0.5f, 1);
				outputR.anchoredPosition = new Vector2(0, -10);
				outputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				outputR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - ViewerCreator.Size - 120);
				var viewport = new GameObject("Viewport");
				viewport.transform.parent = output.transform;
				viewportR = viewport.AddComponent<RectTransform>();
				viewportR.anchorMin = Vector2.zero;
				viewportR.anchorMax = Vector2.one;
				viewportR.pivot = new Vector2(0, 1);
				viewportR.anchoredPosition = Vector2.zero;
				viewportR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
				viewportR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - ViewerCreator.Size - 120);
				viewport.AddComponent<Mask>();
				viewport.AddComponent<Image>().color = color;
				var outputText = new GameObject("OutputText");
				outputText.transform.parent = viewport.transform;
				outputT = outputText.AddComponent<Text>();
				outputT.font = ViewerCreator.font;
				outputT.fontSize = ViewerCreator.FontSize;
				outputT.color = Color.black;
				outputT.text = "Output";
				outputTR = outputText.GetComponent<RectTransform>();
				outputTR = outputT.rectTransform;
				outputTR.anchorMin = new Vector2(0.5f, 1);
				outputTR.anchorMax = new Vector2(0.5f, 1);
				outputTR.pivot = new Vector2(0.5f, 1);
				outputTR.anchoredPosition = new Vector2(5, -5);
				outputTR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width - 10);
				outputT.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
				outputS.content = outputTR;
				outputS.movementType = ScrollRect.MovementType.Clamped;
				outputS.decelerationRate = 0.5f;
				outputS.viewport = viewportR;
				outputS.scrollSensitivity = 30f;

				codeInputTextT.supportRichText = false;
				codeInputI = codeInput.AddComponent<InputField>();
				codeInputI.textComponent = codeInputTextT;

				var autoComplete = new GameObject("AutoComplete");
				autoComplete.transform.parent = transform;
				var autoCompleteS = autoComplete.AddComponent<ScrollRect>();
				var autoCompleteR = autoComplete.GetComponent<RectTransform>();
				autoCompleteR.anchorMin = Vector2.zero;
				autoCompleteR.anchorMax = Vector2.zero;
				autoCompleteR.pivot = Vector2.zero;
				autoCompleteR.anchoredPosition = new Vector2(0, 40);
				autoCompleteR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
				autoCompleteR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
				var aViewport = new GameObject("Viewport");
				aViewport.transform.SetParent(autoCompleteR);
				var aViewportR = aViewport.AddComponent<RectTransform>();
				aViewportR.anchorMin = Vector2.zero;
				aViewportR.anchorMax = Vector2.one;
				aViewportR.pivot = new Vector2(0, 1);
				aViewportR.anchoredPosition = Vector2.zero;
				aViewportR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
				aViewportR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
				aViewport.AddComponent<Mask>();
				aViewport.AddComponent<Image>().color = color;
				var content = new GameObject("Content");
				content.transform.SetParent(aViewportR);
				var contentR = content.AddComponent<RectTransform>();
				contentR.anchorMin = Vector2.zero;
				contentR.anchorMax = Vector2.one;
				contentR.pivot = new Vector2(0, 1);
				contentR.anchoredPosition = Vector2.zero;
				contentR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
				contentR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
				AutoComplete.ScrollView = autoCompleteR;
				AutoComplete.Content = contentR;
				autoCompleteS.movementType = ScrollRect.MovementType.Clamped;
				autoCompleteS.decelerationRate = 0.5f;
				autoCompleteS.viewport = aViewportR;
				autoCompleteS.content = contentR;
				autoCompleteS.scrollSensitivity = 30f;

				var type = typeof(Debug);
				var sLogger = type.GetField("s_Logger", BindingFlags.NonPublic | BindingFlags.Static);
				sLogger?.SetValue(type, new InGameLogger(Debug.unityLogger.logHandler));
				InGameLogger.Logging = log => {
					switch (log.logType) {
					case LogType.Error:
						WriteLine($"<color=#ee0000ff>UnityError:\n{log.message}</color>");
						break;
					case LogType.Assert:
						WriteLine($"<color=#ee0000ff>UnityAssert:\n{log.message}</color>");
						break;
					case LogType.Warning:
						WriteLine($"<color=#55dd00ff>UnityWarning:\n{log.message}</color>");
						break;
					case LogType.Log:
						WriteLine($"<color=#444444ff>UnityLog:\n{log.message}</color>");
						break;
					case LogType.Exception:
						WriteLine($"<color=#ee0000ff>UnityException:\n{log.message}</color>");
						break;
					}
				};

				AutoComplete.Initialize();
				AutoComplete.OnClick += AutoCompleteClick;
				writer = new StringWriter();
				printer = new StreamReportPrinter(writer);
				var compilerContext = new CompilerContext(new CompilerSettings(), printer);
				evaluator = new Evaluator(compilerContext);
				foreach (var assembly in AutoComplete.Assemblies) {
					evaluator.ReferenceAssembly(assembly);
				}
				evaluator.Run("using System;using UnityEngine;using UnityEngine.UI;using UnityEngine.Events;using System.Reflection;using System.Collections;using System.Collections.Generic;using System.Linq;using System.IO;using System.Text;");

				codeInputI.onValueChanged.AddListener(str => {
					caretPosition = codeInputI.caretPosition;
					str = str.Substring(0, caretPosition);
					if (str == string.Empty) {
						autoComplete.SetActive(false);
						return;
					}
					//autoCompleteR.anchoredPosition = new Vector2(codeInputI.caretPosition * ViewerCreator.FontSize, autoCompleteR.anchoredPosition.y);
					var quo = false;
					startIndex = 0;
					for (var i = 0; i < str.Length; i++) {
						if (str[i] == '"') {
							quo = !quo;
						}
						if (str[i] == ';' || str[i] == '(' || str[i] == '[' || str[i] == '{' || str[i] == ','
							|| str[i] == '+' || str[i] == '-' || str[i] == '*' || str[i] == '/' || str[i] == '='
							|| str[i] == '|' || str[i] == '&' || str[i] == ':' || str[i] == '<' || str[i] == '>') {
							if (!quo) {
								startIndex = i + 1;
							}
						}
					}
					if (quo) {
						return;
					}
					str = str.Substring(startIndex);  // 找到最后一个语句的位置，从此断开

					endIndex = 0;
					for (var i = 0; i < str.Length; i++) {
						if (str[i] == '"') {
							quo = !quo;
						}
						if (str[i] == '.') {
							if (!quo && (i == str.Length - 1 || !char.IsDigit(str[i + 1]))) {
								var flag = true;
								for (var j = i + 1; j < str.Length; j++) {
									if (!char.IsLetterOrDigit(str[j]) && str[j] != '_') {
										flag = false;
										break;
									}
								}
								if (flag) {
									endIndex = i; // 找到最后一个.的位置
								}
							}
						}
					}

					if (endIndex == 0) {
						AutoComplete.Update(null, str);
					} else {
						var prefix = str.Substring(0, endIndex);
						var t = AutoComplete.ParseType(prefix);
						if (t == null) {
							var method = evaluator.Compile(prefix);
							writer.Flush();
							var builder = writer.GetStringBuilder();
							if (printer.ErrorsCount > 0) {
								return;
							}
							builder.Clear();
							t = method.Method.ReturnType;
							if (printer.ErrorsCount == 0 && t != typeof(void)) {
								AutoComplete.Update(t, str.Substring(endIndex + 1));
							}
						} else {
							AutoComplete.Update(t, str.Substring(endIndex + 1));
						}
					}
				});

				if (PlayerPrefs.HasKey("InGameConsoleCommandHistory")) {
					foreach (var command in PlayerPrefs.GetString("InGameConsoleCommandHistory").Split('\n')) {
						commandList.AddLast(command);
					}
				}
			} catch (Exception e) {
				Utils.LogError(e.ToString(), "Error in InGameConsole Start");
			}
		}

		private void AutoCompleteClick(string str) {
			codeInputI.Select();
			codeInputI.text = codeInputI.text.Substring(0, startIndex + endIndex + 1) + str + codeInputI.text.Substring(caretPosition);
		}

		public void WriteLine(string message) {
			outputList.AddFirst(message);
			if (outputList.Count >= 128) {
				outputList.RemoveLast();
			}
			buffer.Clear();
			foreach (var output in outputList) {
				buffer.AppendLine(output);
			}
			outputT.text = buffer.ToString();
			//if (outputTR.rect.height > viewportR.rect.height) {
			//	outputTR.anchoredPosition = new Vector2(0, outputTR.rect.height - viewportR.rect.height);
			//}
		}
	}
}