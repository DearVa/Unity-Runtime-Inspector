using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeInspector {
	internal class AutoComplete {
		public static RectTransform ScrollView, Content;
		public static event Action<string> OnClick;
		private static readonly List<Text> Children = new List<Text>();

		public static readonly List<Assembly> Assemblies = new List<Assembly>();
		private static readonly SortedSet<string> Namespaces = new SortedSet<string>();
		private static readonly Dictionary<string, Type> Types = new Dictionary<string, Type>();

		public static void Initialize() {
			Assemblies.Add(Assembly.Load("UnityEngine.CoreModule"));
			Assemblies.Add(Assembly.Load("UnityEngine.PhysicsModule"));
			Assemblies.Add(Assembly.Load("UnityEngine.UI"));
			Assemblies.Add(Assembly.GetExecutingAssembly());

			foreach (var type in Assemblies.SelectMany(assembly => assembly.GetTypes())) {
				if (type.FullName != null) {
					Types.Add(type.FullName, type);
				}
				if (type.Namespace != null) {
					Namespaces.Add(type.Namespace);
				}
			}

			Types.Add("sbyte", typeof(sbyte));
			Types.Add("short", typeof(short));
			Types.Add("int", typeof(int));
			Types.Add("long", typeof(long));

			Types.Add("byte", typeof(byte));
			Types.Add("ushort", typeof(ushort));
			Types.Add("uint", typeof(uint));
			Types.Add("ulong", typeof(ulong));

			Types.Add("char", typeof(char));

			Types.Add("float", typeof(float));
			Types.Add("double", typeof(double));

			Types.Add("decimal", typeof(decimal));

			Types.Add("bool", typeof(bool));

			Types.Add("object", typeof(object));
			Types.Add("string", typeof(string));
		}

		public static Type GetType(string typeStr) {
			if (Types.TryGetValue(typeStr, out var type)) {
				return type;
			}
			foreach (var ns in Namespaces) {
				if (Types.TryGetValue((ns + "." + typeStr), out type)) {
					return type;
				}
			}
			return null;
		}

		public static Type ParseType(string typeStr) {
			var segments = typeStr.Split('.');
			var type = GetType(segments[0]);
			if (type != null) {
				for (var i = 1; i < segments.Length; i++) {
					var mis = type.GetMember(segments[i]);
					if (mis.Length > 0) {
						type = mis[0].ReflectedType;
						if (type == null) {
							return null;
						}
					}
				}
			}
			return type;
		}

		public static void Update(Type type, string str) {
			if (string.IsNullOrEmpty(str)) {
				return;
			}
			str = str.ToLower();
			if (type == null) {
				if (str[0] == '#') {
					UpdateScrollRect(ConsoleCommand.Commands.Keys.Where(c => c.Contains(str)).OrderBy(c => c).ToArray());
				} else {
					UpdateScrollRect(Types.Values.Where(t => t.ReflectedType == null && t.Name.ToLower().Contains(str)).DistinctBy(t => t.Name).OrderBy(t => t.Name).ToArray());
				}
			} else {
				UpdateScrollRect(type.GetMembers().Where(i => i.Name.ToLower().Contains(str)).DistinctBy(i => i.Name).OrderBy(i => i.Name).ToArray());
			}
		}

		private static readonly Color ClassColor = new Color(1f, 0.5f, 0f);
		private static readonly Color FieldColor = new Color(0.46f, 0.74f, 1f);
		private static readonly Color MethodColor = new Color(0.69f, 0.5f, 0.83f);
		private static readonly Color EventColor = new Color(0.76f, 1f, 0f);

		private static void UpdateScrollRect(IReadOnlyList<object> infos) {
			for (var i = infos.Count; i < Children.Count; i++) {
				Children[i].rectTransform.parent.gameObject.SetActive(false);
			}
			if (infos.Count == 0) {
				ScrollView.gameObject.SetActive(false);
				return;
			}
			for (var i = Children.Count; i < infos.Count; i++) {
				var btn = new GameObject("AutoCompleteBtn");
				btn.AddComponent<SceneViewerFlag>();
				btn.transform.SetParent(Content);
				btn.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
				var btnR = btn.GetComponent<RectTransform>();
				btnR.anchorMin = Vector2.one;
				btnR.anchorMax = Vector2.one;
				btnR.pivot = Vector2.one;
				btnR.anchoredPosition = new Vector2(0, -ViewerCreator.Size * i);
				btnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
				btnR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreator.Size);
				var text = new GameObject("AutoCompleteBtnText");
				text.AddComponent<SceneViewerFlag>();
				text.transform.SetParent(btnR);
				var textT = text.AddComponent<Text>();
				textT.font = ViewerCreator.font;
				textT.fontSize = ViewerCreator.FontSize;
				btn.AddComponent<Button>().onClick.AddListener(() => OnClick?.Invoke(textT.text));
				Children.Add(textT);
				var textR = text.GetComponent<RectTransform>();
				textR.anchorMin = Vector2.zero;
				textR.anchorMax = Vector2.one;
				textR.anchoredPosition = Vector2.zero;
				textR.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
				textR.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreator.Size);
			}
			for (var i = 0; i < infos.Count; i++) {
				switch (infos[i]) {
				case Type t:
					if (t.ReflectedType != null) {
						continue;
					}
					Children[i].text = t.Name;
					Children[i].color = ClassColor;
					break;
				case FieldInfo f:
					Children[i].text = f.Name;
					Children[i].color = FieldColor;
					break;
				case PropertyInfo p:
					Children[i].text = p.Name;
					Children[i].color = FieldColor;
					break;
				case MethodInfo m:
					Children[i].text = m.Name;
					Children[i].color = MethodColor;
					break;
				case EventInfo e:
					Children[i].text = e.Name;
					Children[i].color = EventColor;
					break;
				case string s:
					Children[i].text = s;
					Children[i].color = Color.red;
					break;
				}
				Children[i].rectTransform.parent.gameObject.SetActive(true);
			}
			ScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreator.Size * Mathf.Min(infos.Count, 6f));
			Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerCreator.Size * infos.Count);
			ScrollView.gameObject.SetActive(true);
		}
	}
}