using System;
using System.Collections.Generic;
using UnityEngine;

public class UISudokuInputBoard : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] 
    private List<UISudokuInput> _uiInputList = null;
    #endregion

    #region Internal Fields
    private Action<UISudokuInput> _inputOnClickAction;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        for (int i = 0; i < _uiInputList.Count; i++) {
            _uiInputList[i].SetButtonOnClick(InputOnClick);
        }
    }
    #endregion

    #region APIs
    public void SetOnClickAction(Action<UISudokuInput> action) {
        _inputOnClickAction = action;
    }

    public void SetMarking(int inputValue) {
        for (int i = 0; i < _uiInputList.Count; i++) {
            _uiInputList[i].SetMarking(_uiInputList[i].InputValue == inputValue);
        }
    }
    #endregion

    #region Internal Methods
    private void InputOnClick(UISudokuInput uiInput) {
        if (uiInput == null) {
            return;
        }

        if (_inputOnClickAction == null) {
            return;
        }

        _inputOnClickAction(uiInput);
    }
    #endregion
}
