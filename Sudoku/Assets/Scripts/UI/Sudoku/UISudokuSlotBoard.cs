using System;
using System.Collections.Generic;
using UnityEngine;

public class UISudokuSlotBoard : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    private List<UISudokuSlot> _uiSlotList = null;
    #endregion

    #region Internal Fields
    private SudokuData _sData;
    private Action<UISudokuSlot> _onClickAction;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        for (int i = 0; i < _uiSlotList.Count; i++) {
            _uiSlotList[i].SetButtonOnClick(SlotOnClick);
        }
    }
    #endregion

    #region APIs
    public void SetOnClickAction(Action<UISudokuSlot> action) {
        _onClickAction = action;
    }

    public void SetSudokuData(SudokuData sData) {
        _sData = sData;
    }

    public UISudokuSlot GetUISlot(int slotIndex) {
        if (_uiSlotList == null) {
            return null;
        }

        if (slotIndex < 0 || slotIndex >= _uiSlotList.Count) {
            return null;
        }

        return _uiSlotList[slotIndex];
    }

    public void RefreshSlot(int slotIndex) {
        _uiSlotList[slotIndex].SetValueAndReason(_sData.SlotDataList[slotIndex].Value, _sData.SlotDataList[slotIndex].Reason);
    }

    public void RefreshAllSlot() {
        for (int i = 0; i < _uiSlotList.Count; i++) {
            _uiSlotList[i].SetValueAndReason(_sData.SlotDataList[i].Value, _sData.SlotDataList[i].Reason);
        }
    }
    #endregion

    #region Internal Methods
    private void SlotOnClick(UISudokuSlot slot) {
        if (slot == null) {
            return;
        }

        if (_onClickAction == null) {
            return;
        }

        _onClickAction(slot);
    }
    #endregion
}
