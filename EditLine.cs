using UnityEngine;
using UnityEngine.UI;

namespace InGameDebugger {
	public class EditLine : MonoBehaviour {
		public Button funcBtn;
		public Text nameText, label;
		public EditGameObject editGameObject;

		public void Init(Button funcBtn, Text nameText, Text label) {
			this.funcBtn = funcBtn;
			this.nameText = nameText;
			this.label = label;
		}

		public void Set(EditGameObject editGameObject) {
			nameText.text = editGameObject.gameObject.name;
			Color color;
			if (editGameObject.highlight) {
				if (editGameObject.gameObject.activeSelf) {
					color = Color.red;
				} else {
					color = new Color(0.7f, 0.1f, 0.1f);
				}
			} else {
				if (editGameObject.gameObject.activeSelf) {
					color = Color.black;
				} else {
					color = new Color(0.3f, 0.3f, 0.3f);
				}
			}
			nameText.color = color;
			this.editGameObject = editGameObject;
			if (editGameObject.children.Count == 0) {
				gameObject.GetComponent<Button>().interactable = false;
				gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
				label.text = "";
			} else {
				gameObject.GetComponent<Button>().interactable = true;
				gameObject.GetComponent<Image>().color = Color.white;
				label.text = editGameObject.open ? "－" : "＋";
			}
		}

		public void SwitchOpen() {
			editGameObject.SwitchOpen();
			label.text = editGameObject.open ? "－" : "＋";
		}
	}
}