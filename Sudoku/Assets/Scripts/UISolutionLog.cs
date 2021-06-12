using UnityEngine;
using TMPro;

public class UISolutionLog : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    private TextMeshProUGUI _text = null;
    #endregion

    #region APIs
    public void SetFillReason(int rowIndex, int columnIndex, int value, FillReason reason) {
        _text.text = string.Format("Solution {0} found at Row {1}, Column {2} by {3} method",
            rowIndex, columnIndex, value, GetReasonString(reason));
    }
    #endregion

    #region Internal Methods
    private string GetReasonString(FillReason reason) {
        string s = string.Empty;
        switch (reason) {
            case FillReason.QuestionInput:
                // do nothing
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
