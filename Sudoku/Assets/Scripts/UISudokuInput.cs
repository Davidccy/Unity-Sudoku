using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISudokuInput : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    private Button _btn = null;

    [SerializeField]
    private TextMeshProUGUI _textValue = null;

    [SerializeField]
    private int _value = 0;

    [SerializeField]
    private Image _imageBG = null;
    #endregion

    #region Internal Fields
    private Action<UISudokuInput> _btnAction;
    #endregion

    #region Mono Behaviurs Hooks
    private void Awake() {
        _btn.onClick.AddListener(ButtonOnClick);
    }
    #endregion

    #region Button Handlings
    private void ButtonOnClick() {
        if (_btnAction == null) {
            return;
        }

        _btnAction(this);
    }
    #endregion

    #region Properties
    public int InputValue {
        get {
            return _value;
        }
    }
    #endregion

    #region APIs
    public void SetButtonOnClick(Action<UISudokuInput> cb) {
        _btnAction = cb;
    }

    public void SetMarking(bool marking) {
        _imageBG.color = marking ? Color.gray : Color.white;
        _textValue.color = marking ? Color.white : Color.black;
    }
    #endregion

#if UNITY_EDITOR
    #region Testing
    [ContextMenu("TestImportValueToText")]
    public void TestImportValueToText() {
        _textValue.text = string.Format("{0}", _value);
    }
    #endregion
#endif
}
