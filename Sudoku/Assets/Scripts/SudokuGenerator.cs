using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuGenerator : MonoBehaviour {
    [Header("Question Area")]
    [SerializeField]
    private UISudokuSlotBoard _uiSlotBoard = null;
    [SerializeField]
    private UISudokuInputBoard _uiInputBoard = null;

    [SerializeField]
    private Button _btnNewQuiz = null;
    [SerializeField]
    private Button _btnShowSolution = null;

    #region Internal Fields
    private SudokuData _sData;
    #endregion

    #region Mono Behaviour Hooks
    private void Start() {
        InitUI();
    }
    #endregion

    #region Internal Methods
    private void InitUI() {
        _btnNewQuiz.onClick.AddListener(ButtonNewQuizOnClick);
        _btnShowSolution.onClick.AddListener(ButtonShowSolutionOnClick);
    }
    #endregion

    #region UI Button Handlings
    private void ButtonNewQuizOnClick() {
        QuizGeneration();
    }

    private void ButtonShowSolutionOnClick() {

    }
    #endregion

    public void QuizGeneration() {
        // 1. get original quiz
        // 2. replace number (ex: 3->5, 5->9, 9>3)
        // 3. remove a slot value randomly and then check is solvable
        // 4. repeat step 3 until reached specific count determined by difficulty

        // get original quiz
        int dataLength = GeneratorData.bList.Count;
        int rndDataIndex = Random.Range(0, dataLength);
        GeneratorData.B baseData = GeneratorData.bList[rndDataIndex];
        _sData = new SudokuData(baseData, FillReason.QuestionInput);

        // replace number
        int rndReplaceCount = Random.Range(1, 10);
        int executeCount = 0;
        while (executeCount < rndReplaceCount) {
            ReplaceNumber(_sData);
            executeCount += 1;
        }

        // remove number, random count (count determined by difficulty)
        RemoveNumberFromData(_sData, 10);
        _uiSlotBoard.SetSudokuData(_sData);
        _uiSlotBoard.RefreshAllSlot();
    }

    private void ReplaceNumber(SudokuData sData) {
        // generate replace reference
        List<int> replaceReferenceTemp = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> replaceReference = new List<int>();
        while (replaceReferenceTemp.Count > 0) {
            int rndIndex = Random.Range(0, replaceReferenceTemp.Count);
            replaceReference.Add(replaceReferenceTemp[rndIndex]);
            replaceReferenceTemp.RemoveAt(rndIndex);
        }

        // replace reference temp maybe : { 2, 5, 7, 9, 4, 1, 3, 6, 8 }
        // it means that 1 will be raplaced as 2
        //               2 ...                 5
        //               3 ...                 7
        // ...

        // replace
        for (int i = 0; i < sData.SlotDataList.Count; i++) {
            int value = sData.SlotDataList[i].Value;
            int newValue = replaceReference[value - 1];
            sData.SlotDataList[i].Value = newValue;
        }

        sData.Update();
    }

    private bool RemoveNumberFromData(SudokuData sData, int count) {
        // 1. remove value of a "Not Empty" slot randomly
        // 2. check this data is solvable. 
        //    If solvable, count by one, if not solvable, undo step 1.

        sData.Update();
        List<int> filledSlotList = sData.GetAllFilledSlotIndex();
        int retryCount = filledSlotList.Count;
        int exeCount = 0;
        int rndSlotIndex = -1;
        int rndSlotValue = -1;
        FillReason rndSlotReason = FillReason.None;

        while (exeCount < retryCount) {
            // revert if previous try is available (previous try failed)
            if (rndSlotIndex != -1) {
                sData.SetSlotValueAndReason(rndSlotIndex, rndSlotValue, rndSlotReason);
            }

            // try remove random slot
            if (filledSlotList.Count == 0) {
                return false;
            }

            int rndListIndex = Random.Range(0, filledSlotList.Count);
            rndSlotIndex = filledSlotList[rndListIndex];
            SlotData slotData = sData.GetSlotData(rndSlotIndex);
            rndSlotValue = slotData.Value;
            rndSlotReason = slotData.Reason;
            filledSlotList.RemoveAt(rndListIndex);

            sData.SetSlotValueAndReason(rndSlotIndex, 0, FillReason.None);
            exeCount += 1;

            if (!sData.IsSolvable()) {
                continue;
            }

            if (count <= 1) {
                return true;
            }

            bool success = RemoveNumberFromData(sData, count - 1);
            if (success) {
                return true;
            }
            else {
                continue;
            }
        }

        return false;
    }

    [ContextMenu("Test")]
    public void Test() {
        QuizGeneration();
    }
}
