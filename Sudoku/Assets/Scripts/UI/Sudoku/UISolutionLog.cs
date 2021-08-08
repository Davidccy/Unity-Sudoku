using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISolutionLog : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    private TextMeshProUGUI _text = null;

    [SerializeField]
    private Button _btn = null;
    #endregion

    #region Internal Fields
    private int _targetSlotIndex = -1;
    private Action<UISolutionLog> _onClickAction = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btn.onClick.AddListener(ButtonOnClick);
    }

    private void OnDestroy() {
        _btn.onClick.RemoveAllListeners();
    }
    #endregion

    #region Properties
    public int TargetSlotIndex {
        get {
            return _targetSlotIndex;
        }
    }
    #endregion

    #region Button Handlings
    private void ButtonOnClick() {
        if (_onClickAction != null) {
            _onClickAction(this);
        }
    }
    #endregion

    #region APIs
    public void SetFillReason(int slotIndex, int value, FillReason reason) {
        _targetSlotIndex = slotIndex;

        SudokuUtility.ConvertToIndex(slotIndex, out int rowIndex, out int columnIndex, out int _);
        _text.text = string.Format("Solution {0} found at Row {1}, Column {2} by {3} method",
            value, rowIndex, columnIndex, GetReasonString(reason), slotIndex);
    }

    public void SetOnClickAction(Action<UISolutionLog> action) {
        _onClickAction = action;
    }
    #endregion

    #region Internal Methods
    private string GetReasonString(FillReason reason) {
        string s = string.Empty;
        switch (reason) {
            case FillReason.QuestionInput:
                // Do nothing
                break;
            case FillReason.RowRemain:
                s = "Row Remain";
                break;
            case FillReason.ColumnRemain:
                s = "Column Remain";
                break;
            case FillReason.SquareRemain:
                s = "Square Remain";
                break;
            case FillReason.SlotExclude:
                s = "Slot Exlude";
                break;
            case FillReason.CommonSolution:
                s = "Common Solution";
                break;
        }

        return s;
    }
    #endregion
}
