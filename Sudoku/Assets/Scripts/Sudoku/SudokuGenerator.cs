using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SudokuGenerator : MonoBehaviour {
    #region Serialized Fields
    [Header("Question Area")]
    [SerializeField]
    private UISudokuSlotBoard _uiSlotBoard = null;
    [SerializeField]
    private UISudokuInputBoard _uiInputBoard = null;

    [Header("Operation")]
    [SerializeField]
    private Button _btnNewQuiz = null;
    [SerializeField]
    private Button _btnShowSolution = null;

    [Header("Timer")]
    [SerializeField]
    private TextMeshProUGUI _textTimer = null;
    #endregion

    #region Internal Fields
    private float _timer = 0;
    private bool _isQuizGenerated = false;
    private bool _isResolving = false;
    private int _difficulty = 0;
    private SudokuData _sData;
    private UISudokuInput _selectingInput = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Start() {
        InitUI();
    }

    private void Update() {
        if (_isQuizGenerated && _isResolving) {
            _timer += Time.deltaTime;
        }
    }

    private void OnEnable() {
        ResetData();
        ResetUI();
    }
    #endregion

    #region UI Button Handlings
    private async void ButtonNewQuizOnClick() {
        if (_sData != null && _sData.HasAnyInput()) {
            UIWindowMessage window = await UIWindowManager.Instance.GetWindow(SystemDefine.UI_WINDOW_NAME_MESSAGE) as UIWindowMessage;
            UIWindowMessage.MessageCmd cmd = new UIWindowMessage.MessageCmd();
            cmd.Type = UIWindowMessage.MessageType.YesNo;
            cmd.Title = "Are you sure to generate new quiz ?";
            cmd.ActionYes = async () => {
                UIWindowManager.Instance.CloseWindow(SystemDefine.UI_WINDOW_NAME_MESSAGE).DoNotAwait();
                ShowDifficultyWindow();
            };
            cmd.ActionNo = () => {
                UIWindowManager.Instance.CloseWindow(SystemDefine.UI_WINDOW_NAME_MESSAGE).DoNotAwait();
            };

            window.SetInfo(cmd);
            window.Show(true, false).DoNotAwait();

            return;
        }

        ShowDifficultyWindow();
    }

    private async void ButtonShowSolutionOnClick() {
        UIWindowMessage window = await UIWindowManager.Instance.GetWindow(SystemDefine.UI_WINDOW_NAME_MESSAGE) as UIWindowMessage;
        UIWindowMessage.MessageCmd cmd = new UIWindowMessage.MessageCmd();
        cmd.Type = UIWindowMessage.MessageType.YesNo;
        cmd.Title = "Are you sure to show all solutions ?";
        cmd.ActionYes = async () => {
            UIWindowManager.Instance.CloseWindow(SystemDefine.UI_WINDOW_NAME_MESSAGE).DoNotAwait();
            if (_sData.IsSolvable()) {
                _sData.FillAllSolution();

                _selectingInput = null;

                Refresh();
            }
        };
        cmd.ActionNo = () => {
            UIWindowManager.Instance.CloseWindow(SystemDefine.UI_WINDOW_NAME_MESSAGE).DoNotAwait();
        };

        window.SetInfo(cmd);
        window.Show(true, false).DoNotAwait();
    }

    public void ButtonSlotOnClick(UISudokuSlot slot) {
        if (!_isQuizGenerated) {
            return;
        }

        if (!_isResolving) {
            return;
        }

        if (slot == null) {
            return;
        }

        if (slot.FillReason == FillReason.QuestionInput) {
            return;
        }

        if (_selectingInput == null) {
            return;
        }

        int inputValue = _selectingInput.InputValue;
        if (slot.Value == inputValue) {
            inputValue = 0;
        }

        FillSolutionIntoSlot(slot.SlotIndex, inputValue, FillReason.PlayerInput);
        //_undoIndex += 1;

        //RefreshUndoButton();
    }

    public void ButtonInputOnClick(UISudokuInput input) {
        if (!_isQuizGenerated) {
            return;
        }

        if (!_isResolving) {
            return;
        }

        if (input == null) {
            return;
        }

        if (_selectingInput == input) {
            _selectingInput = null;
        }
        else {
            _selectingInput = input;
        }

        Refresh();
    }
    #endregion

    #region Internal Methods
    private void InitUI() {
        // Operation
        _btnNewQuiz.onClick.AddListener(ButtonNewQuizOnClick);
        _btnShowSolution.onClick.AddListener(ButtonShowSolutionOnClick);

        // Slots		
        _uiSlotBoard.SetOnClickAction(ButtonSlotOnClick);

        // Inputs
        _uiInputBoard.SetOnClickAction(ButtonInputOnClick);
    }

    private void ResetData() {
        // Sudoku data
        ClearSudokuData();

        // Selected input
        _selectingInput = null;

        //// Undo
        //ClearUndoData();
        //RefreshUndoButton();

        // Timer
        _timer = 0;
        CancelInvoke("RefreshTimer");
    }

    private void ResetUI() {
        Refresh();
    }

    private void ClearSudokuData() {
        _sData = new SudokuData();
        _uiSlotBoard.SetSudokuData(_sData);

        _isQuizGenerated = false;
        _isResolving = false;
    }

    private void Refresh() {
        RefreshButtons();
        RefreshUISlots();
        RefreshUIInputs();
        RefreshTimer();
    }

    private void RefreshButtons() {
        _btnNewQuiz.interactable = true; // Always true
        _btnShowSolution.interactable = _isResolving;
    }

    private void RefreshUISlots() {
        _uiSlotBoard.RefreshAllSlot();
    }

    private void RefreshUIInputs() {
        int selectingInput = _selectingInput == null ? -1 : _selectingInput.InputValue;
        _uiInputBoard.SetMarking(selectingInput);
    }

    private void RefreshTimer() {
        int seconds = (int) _timer;
        _textTimer.text = string.Format("{0} : {1:00}", seconds / 60, seconds % 60);
    }

    private async void ShowDifficultyWindow() {
        UIWindowDifficulty dWindow =
                    await UIWindowManager.Instance.GetWindow(SystemDefine.UI_WINDOW_NAME_DIFFICULTY) as UIWindowDifficulty;

        if (dWindow != null) {
            dWindow.Show(true, false).DoNotAwait();
            dWindow.SetDifficultyOnClickAction((difficulty) => {
                ResetData();

                _difficulty = difficulty;
                QuizGeneration();

                ResetUI();

                CancelInvoke("RefreshTimer");
                InvokeRepeating("RefreshTimer", 0, 1.0f);
            });
        }
    }

    private void QuizGeneration() {
        // 1. Get original quiz
        // 2. Replace number (ex: 3->5, 5->9, 9>3)
        // 3. Remove a slot value randomly and then check is solvable
        // 4. Repeat step 3 until reached specific count determined by difficulty

        // Get original quiz
        List<CompleteData> completeDataList = SudokuUtility.CompleteDataList;
        int dataLength = completeDataList.Count;
        int rndDataIndex = Random.Range(0, dataLength);
        CompleteData baseData = completeDataList[rndDataIndex];
        _sData = new SudokuData(baseData, FillReason.QuestionInput);
        _sData.SetInputChangedAction(OnSudokuDataInputChanged);

        // Replace number
        int rndReplaceCount = Random.Range(1, 10);
        int executeCount = 0;
        while (executeCount < rndReplaceCount) {
            ReplaceNumber(_sData);
            executeCount += 1;
        }

        // Remove number, random count (count determined by difficulty)
        int countToRemove = GetRemoveCountByDifficulty(_difficulty);
        RemoveNumberFromData(_sData, countToRemove);
        _uiSlotBoard.SetSudokuData(_sData);
        _uiSlotBoard.RefreshAllSlot();

        _isQuizGenerated = true;
        _isResolving = true;
    }

    private async void OnSudokuDataInputChanged() {
        if (SudokuUtility.IsSudokuDataComplete(_sData)) {
            _isResolving = false;

            UIWindowCongratulations window = await UIWindowManager.Instance.GetWindow(SystemDefine.UI_WINDOW_NAME_CONGRATULATIONS) as UIWindowCongratulations;
            await window.Show(true, true);
            window.PlayAnimation();
        }
    }

    private void ReplaceNumber(SudokuData sData) {
        // Generate replace reference
        List<int> replaceReferenceTemp = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> replaceReference = new List<int>();
        while (replaceReferenceTemp.Count > 0) {
            int rndIndex = Random.Range(0, replaceReferenceTemp.Count);
            replaceReference.Add(replaceReferenceTemp[rndIndex]);
            replaceReferenceTemp.RemoveAt(rndIndex);
        }

        // Replace reference temp maybe : { 2, 5, 7, 9, 4, 1, 3, 6, 8 }
        // It means that 1 will be raplaced as 2
        //               2 ...                 5
        //               3 ...                 7
        // ...

        // Replace
        for (int i = 0; i < sData.SlotDataList.Count; i++) {
            int value = sData.SlotDataList[i].Value;
            int newValue = replaceReference[value - 1];
            sData.SlotDataList[i].Value = newValue;
        }

        sData.Update();
    }

    private bool RemoveNumberFromData(SudokuData sData, int count) {
        // 1. Remove value of a "Filled" slot randomly
        // 2. Check this data is solvable. 
        //    If solvable, count by one, if not solvable, undo step 1.

        sData.Update();
        List<int> filledSlotList = sData.GetAllFilledSlotIndex();
        int retryCount = filledSlotList.Count;
        int exeCount = 0;
        int rndSlotIndex = -1;
        int rndSlotValue = -1;
        FillReason rndSlotReason = FillReason.None;

        while (exeCount < retryCount) {
            // Revert if previous try is available (previous try failed)
            if (rndSlotIndex != -1) {
                sData.SetSlotValueAndReason(rndSlotIndex, rndSlotValue, rndSlotReason);
            }

            // Try remove random slot
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

    private int GetRemoveCountByDifficulty(int difficulty) {
        int count = 0;

        if (difficulty == (int) Difficulty.Easy) {
            count = Random.Range(15, 30);
        }
        else if (difficulty == (int) Difficulty.Normal) {
            count = Random.Range(30, 40);
        }
        else if (difficulty == (int) Difficulty.Hard) {
            count = Random.Range(40, 55);
        }

        return count;
    }

    private void FillSolutionIntoSlot(int rowIndex, int columnIndex, int value, FillReason reason) {
        int slotIndex = rowIndex * SudokuUtility.PUZZLE_LENGTH + columnIndex;

        // Update data
        _sData.SetSlotValueAndReason(slotIndex, value, reason);

        UISudokuSlot uiSlot = _uiSlotBoard.GetUISlot(slotIndex);
        //// Add to undo
        //if (_undoIndex >= _undoCmdList.Count) {
        //    _undoCmdList.Add(new UndoCommand());
        //}
        //_undoCmdList[_undoIndex].AddNewCommand(uiSlot.Value, uiSlot.FillReason, slotIndex);

        uiSlot.SetValueAndReason(value, reason);

        //if (reason != FillReason.QuestionInput) {
        //    AddSolutionLog(rowIndex, columnIndex, value, reason);
        //}
    }

    private void FillSolutionIntoSlot(int slotIndex, int value, FillReason reason) {
        SudokuUtility.ConvertToIndex(slotIndex, out int rowIndex, out int columnIndex, out int _);

        FillSolutionIntoSlot(rowIndex, columnIndex, value, reason);
    }
    #endregion
}
