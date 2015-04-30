using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(InputField))]
public class ChangeableInputField : MonoBehaviour, IChangeable {

    public string textType;

    private InputField field;

	void Awake () {
        field = GetComponent<InputField>();
	}

    public void SetText(string text) {
        field.text = text;
    }

    public string type {
        get { return textType; }
    }

    public bool IsType(string otherType) {
        return type == otherType;
    }
}
