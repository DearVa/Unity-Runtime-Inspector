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
		private readonly StringBuilder buffer = new StringBuilder();
		private readonly LinkedList<string> commandList = new LinkedList<string>();

		private LinkedListNode<string> NowCommand {
			set {
				nowCommand = value;
				codeInputI.text = nowCommand == null ? string.Empty : nowCommand.Value;
			}
		}

		private LinkedListNode<string> nowCommand;

		void Update() {
			if (codeInputI.isFocused && commandList.Count > 0) {
				if (Input.GetKeyDown(KeyCode.UpArrow)) {
					NowCommand = nowCommand == null ? commandList.Last : nowCommand.Previous;
				} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
					NowCommand = nowCommand == null ? commandList.First : nowCommand.Next;
				}
			}
		}

		void Awake() {
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

				codeInputTextT.supportRichText = false;
				codeInputI = codeInput.AddComponent<InputField>();
				codeInputI.textComponent = codeInputTextT;

				var type = typeof(Debug);
				var s_Logger = type.GetField("s_Logger", BindingFlags.NonPublic | BindingFlags.Static);
				s_Logger.SetValue(type, new InGameLogger(Debug.unityLogger.logHandler));
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
							WriteLine($"<color=#aaaaaaff>UnityLog:\n{log.message}</color>");
							break;
						case LogType.Exception:
							WriteLine($"<color=#ee0000ff>UnityException:\n{log.message}</color>");
							break;
					}
				};

				var writer = new StringWriter();
				var printer = new StreamReportPrinter(writer);
				var compilerContext = new CompilerContext(new CompilerSettings(), printer);
				evaluator = new Evaluator(compilerContext);
				evaluator.ReferenceAssembly(Assembly.Load("UnityEngine.CoreModule"));
				evaluator.ReferenceAssembly(Assembly.Load("UnityEngine.PhysicsModule"));
				evaluator.ReferenceAssembly(Assembly.Load("UnityEngine.UI"));
				evaluator.ReferenceAssembly(Assembly.GetExecutingAssembly());
				evaluator.Run("using System;using UnityEngine;using UnityEngine.UI;using UnityEngine.Events;using System.Reflection;using System.Collections;using System.Collections.Generic;");
				codeInputI.onEndEdit.AddListener(str => {
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
							WriteLine($"{str}\n<color=#ff0000ff>{s}</color>");
							return;
						}
						commandList.AddLast(str);
						if (commandList.Count >= 128) {
							commandList.RemoveFirst();
						}
						PlayerPrefs.SetString("InGameConsoleCommandHistory", string.Join("\n", commandList));
						object result = null;
						method.Invoke(ref result);
						if (result == null) {
							result = "Done";
						}
						WriteLine($"{str}\n<color=#00aa00ff>{result}</color>");
					} catch (Exception e) {
						WriteLine($"{str}\n<color=#ff0000ff>{e.Message}</color>"); 
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

		public void WriteLine(string message) {
			buffer.AppendLine(message);
			buffer.AppendLine();
			outputT.text = buffer.ToString();
			if (outputTR.rect.height > viewportR.rect.height) {
				outputTR.anchoredPosition = new Vector2(0, outputTR.rect.height - viewportR.rect.height);
			}
		}
	}
}