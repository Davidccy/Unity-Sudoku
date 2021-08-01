using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowSerialNumber : UIGenericWindow {
    #region Serialized Fields
    [SerializeField]
    private List<TMP_InputField> _inputFieldList = null;

    [SerializeField]
    private TextMeshProUGUI _textWarningMsg = null;

    [SerializeField]
    private Button _btnConfirm = null;

    [SerializeField]
    private Button _btnCancle = null;
    #endregion

    #region Exposed Fields
    public override string WindowName => SystemDefine.UI_WINDOW_NAME_OPTION;
    #endregion

    #region Internal Fields
    private bool _initializing = false;
    private Action<List<string>> _onClickActionConfirm;
    #endregion

    #region Override Methods
    protected override void OnWindowAwake() {
        _btnConfirm.onClick.AddListener(ButtonConfirmOnClick);
        _btnCancle.onClick.AddListener(ButtonCancleOnClick);

        for (int i = 0; i < _inputFieldList.Count; i++) {
            _inputFieldList[i].onValueChanged.AddListener(InputFieldValueOnChanged);
        }
    }

    protected override void OnWindowEnable() {
        _initializing = true;

        for (int i = 0; i < _inputFieldList.Count; i++) {
            _inputFieldList[i].text = string.Empty;
        }

        _initializing = false;

        Refresh();
    }

    protected override void OnWindowDestroy() {
        _btnConfirm.onClick.RemoveAllListeners();
        _btnCancle.onClick.RemoveAllListeners();
        for (int i = 0; i < _inputFieldList.Count; i++) {
            _inputFieldList[i].onValueChanged.AddListener(InputFieldValueOnChanged);
        }
    }
    #endregion

    #region Input Field Handlings
    private void InputFieldValueOnChanged(string str) {
        if (_initializing) {
            return;
        }

        Refresh();
    }
    #endregion

    #region UI Button Handlings
    private void ButtonConfirmOnClick() {
        if (_onClickActionConfirm != null) {
            _onClickActionConfirm(GetAllInput());
        }

        Show(false, false).DoNotAwait();
    }

    private void ButtonCancleOnClick() {
        Show(false, false).DoNotAwait();
    }
    #endregion

    #region APIs
    public void SetConfirmAction(Action<List<string>> action) {
        _onClickActionConfirm = action;
    }
    #endregion

    #region Internal Methods
    private void Refresh() {
        _btnConfirm.interactable = IsAllInputValid();
        _textWarningMsg.text = GetWarningMessage();
    }

    private bool IsAllInputValid() {
        // Check all input fields
        for (int i = 0; i < _inputFieldList.Count; i++) {
            string input = _inputFieldList[i].text;
            if (string.IsNullOrEmpty(input) || input.Length != SudokuUtility.PUZZLE_LENGTH) {
                return false;
            }

            // Check all character
            for (int j = 0; j < input.Length; j++) {
                string subInput = input.Substring(j, 1);
                if (!int.TryParse(subInput, out int outputInt)) {
                    return false;
                }
            }
        }

        return true;
    }

    private string GetWarningMessage() {
        bool reasonHasEmpty = false;
        bool reasonLengthNotMatch = false;
        bool reasonNotNumber = false;

        for (int i = 0; i < _inputFieldList.Count; i++) {
            string input = _inputFieldList[i].text;

            // Check empty
            if (string.IsNullOrEmpty(input)) {
                reasonHasEmpty = true;
            }

            // Check length
            if (input.Length != SudokuUtility.PUZZLE_LENGTH) {
                reasonLengthNotMatch = true;
            }

            // Check character
            for (int j = 0; j < input.Length; j++) {
                string subInput = input.Substring(j, 1);
                if (!int.TryParse(subInput, out int outputInt)) {
                    reasonNotNumber = true;
                }
            }
        }

        string msg = string.Empty;
        if (reasonHasEmpty) {
            msg = "Input field can not be empty";
        }
        else if (reasonLengthNotMatch) {
            msg = "Input length not match\n(The length must be 9)";
        }
        else if (reasonNotNumber) {
            msg = "Input must be number";
        }
        else { 
        }

        return msg;
    }

    private List<string> GetAllInput() {
        List<string> inputs = new List<string>();

        for (int i = 0; i < _inputFieldList.Count; i++) {
            inputs.Add(_inputFieldList[i].text);
        }

        return inputs;
    }
    #endregion
}
