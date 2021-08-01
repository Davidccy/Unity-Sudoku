using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SudokuSolver : MonoBehaviour {
	#region Serialized Fields
	[Header("Reset Menu")]
	[SerializeField]
	private Button _btnReset = null;

	[Header("Serial Number Menu")]
	[SerializeField]
	private Button _btnSerialNumber = null;
		
	[Header("Question Area")]
	[SerializeField]
	private UISudokuSlotBoard _uiSlotBoard = null;
	[SerializeField]
	private UISudokuInputBoard _uiInputBoard = null;

	[Header("Operations")]
	[SerializeField]
	private Button _btnUndo = null;
	[SerializeField]
	private Button _btnFindAllSolution = null;
	[SerializeField]
	private Button _btnFindNextSolution = null;

	[Header("Solution Log")]
	[SerializeField]
	private Button _btnLog = null;
	[SerializeField]
	private UISolutionLog _solutionLogRes = null;
	[SerializeField]
	private GameObject _goSolutionLogRoot = null;
	[SerializeField]
	private GameObject _goSolutionLogMenu = null;
	#endregion

	#region Internal Fields
	private SudokuData _sData;
	private UISudokuInput _selectingInput = null;

	private int _undoIndex = 0;
	private List<UndoCommand> _undoCmdList = new List<UndoCommand>();
	private List<UISolutionLog> _solutionLogList = new List<UISolutionLog>();
    #endregion

    #region MonoBehaviour Hooks
    private void Awake() {
		InitUI();
    }

    public void OnEnable() {
		ResetData();
		ResetUI();
	}
	#endregion

	#region UI Button Handlings
	public async void ButtonResetOnClick() {
		UIWindowMessage window = 
			await UIWindowManager.Instance.GetWindow(SystemDefine.UI_WINDOW_NAME_MESSAGE) as UIWindowMessage;
		if (window == null) {
			return;
		}

		UIWindowMessage.MessageCmd cmd = new UIWindowMessage.MessageCmd();
		cmd.Type = UIWindowMessage.MessageType.YesNo;
		cmd.Title = "Are you sure to reset all slot ?";
		cmd.ActionYes = () => {
			ResetData();
			UIWindowManager.Instance.CloseWindow(SystemDefine.UI_WINDOW_NAME_MESSAGE).DoNotAwait();
		};
		cmd.ActionNo = () => {
			UIWindowManager.Instance.CloseWindow(SystemDefine.UI_WINDOW_NAME_MESSAGE).DoNotAwait();
		};

		window.SetInfo(cmd);
		window.Show(true, false).DoNotAwait();
	}

	public async void ButtonSerialNumberOnClick() {
		UIWindowSerialNumber window = 
			await UIWindowManager.Instance.GetWindow(SystemDefine.UI_WINDOW_NAME_SERIAL_NUMBER) as UIWindowSerialNumber;

		if (window == null) {
			return;
		}

		window.SetConfirmAction(
			(inputList) => {
				if (inputList == null || inputList.Count != SudokuUtility.PUZZLE_LENGTH) {
					Debug.LogErrorFormat("Null input field or unexpected count of input field");
					return;
				}

				//for (int i = 0; i < inputList.Count; i++) {
				//	string input = inputList[i];
				//	if (input.Length != SudokuUtility.PUZZLE_LENGTH) {
				//		Debug.LogErrorFormat("Unexpected length of input in row {0}", i + 1);
				//		return;
				//	}
				//}

				// Fill input
				for (int i = 0; i < inputList.Count; i++) {
					string input = inputList[i];
					for (int j = 0; j < SudokuUtility.PUZZLE_LENGTH; j++) {
						int slotIndex = i * SudokuUtility.PUZZLE_LENGTH + j;
						int inputValue = int.Parse(input.Substring(j, 1));
						_sData.SetSlotValueAndReason(slotIndex, inputValue, FillReason.QuestionInput);
					}
				}

				_sData.Update();

				Refresh();
			});

		window.Show(true, false).DoNotAwait();
	}

	public void ButtonUndoOnClick() {
		ExecuteUndo();
	}

	public void ButtonFindAllSolutionOnClick() {
		FindSolution(true);
	}

	public void ButtonFindNextSolutionOnClick() {
		FindSolution(false);
	}

	public void ButtonLogOnClick() {
		ShowLogMenu(!_goSolutionLogMenu.activeSelf);
	}

	public void ButtonSlotOnClick(UISudokuSlot slot) {
		if (slot == null) {
			return;
		}

		if (_selectingInput == null) {
			return;
		}

		int inputValue = _selectingInput.InputValue;
		if (slot.Value == inputValue) {
			inputValue = 0;
		}

		FillSolutionIntoSlot(slot.SlotIndex, inputValue, FillReason.QuestionInput);
		_undoIndex += 1;

		Refresh();
	}

	public void ButtonInputOnClick(UISudokuInput input) {
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

	#region Internal Fields
	private void InitUI() {
		// Reset menu
		_btnReset.onClick.AddListener(ButtonResetOnClick);

		// Serial number menu
		_btnSerialNumber.onClick.AddListener(ButtonSerialNumberOnClick);

		// Operation
		_btnUndo.onClick.AddListener(ButtonUndoOnClick);
		_btnFindAllSolution.onClick.AddListener(ButtonFindAllSolutionOnClick);
		_btnFindNextSolution.onClick.AddListener(ButtonFindNextSolutionOnClick);

		// Log
		_btnLog.onClick.AddListener(ButtonLogOnClick);

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

		// Undo
		ClearUndoData();

		// Log
		ClearLogData();
	}

	private void ResetUI() {
		ShowLogMenu(false);
		Refresh();
	}

	private void ClearSudokuData() {
		_sData = new SudokuData();
		_uiSlotBoard.SetSudokuData(_sData);
	}

	private void ClearUndoData() {
		_undoIndex = 0;
		_undoCmdList.Clear();
	}

	private void Refresh() {
		RefreshUISlots();
		RefreshUIInputs();
		RefreshButtons();
	}

	private void RefreshUISlots() {
		_uiSlotBoard.RefreshAllSlot();
	}

	private void RefreshUIInputs() {
		int selectingInput = _selectingInput == null ? -1 : _selectingInput.InputValue;
		_uiInputBoard.SetMarking(selectingInput);
	}

	private void RefreshButtons() {
		_btnReset.interactable = _sData.HasAnyInput();
		_btnUndo.interactable = _undoCmdList.Count > 0;

		_btnFindAllSolution.interactable = _sData.HasAnyInput();
		_btnFindNextSolution.interactable = _sData.HasAnyInput();
	}

	private void ShowLogMenu(bool show) {
		_goSolutionLogMenu.SetActive(show);
	}

	private void ExecuteUndo() {
		if (_undoCmdList.Count <= 0) {
			return;
		}

		UndoCommand cmd = _undoCmdList[_undoCmdList.Count - 1];
		for (int i = 0; i < cmd.ValueList.Count; i++) {
			int slotIndex = cmd.SlotIndexList[i];
			int value = cmd.ValueList[i];
			FillReason reason = cmd.FillReasonList[i];

			_sData.SetSlotValueAndReason(slotIndex, value, reason);
			_sData.Update();
		}

		_undoIndex -= 1;
		_undoCmdList.RemoveAt(_undoCmdList.Count - 1);

		Refresh();
	}

	private void FindSolution(bool findAll) {
		bool foundSolution = false;
		if (findAll) {
			while (TryToFindNextStep()) {
				// Repeat until find no step
				foundSolution = true;
			}
		}
		else {
			if (TryToFindNextStep()) {
				foundSolution = true;
			}
		}

		if (foundSolution) {
			_undoIndex += 1;
		}

		Refresh();
	}

	private bool TryToFindNextStep() {
		_sData.Update();
		SolutionInfo solution = SudokuUtility.FindSolution(_sData);
		if (solution != null) {
			FillSolutionIntoSlot(solution.SlotData.SlotIndex, solution.Value, solution.Reason);
			return true;
		}

		return false;
	}

	private void FillSolutionIntoSlot(int rowIndex, int columnIndex, int value, FillReason reason) {
		int slotIndex = rowIndex * SudokuUtility.PUZZLE_LENGTH + columnIndex;

		// Update data
		_sData.SetSlotValueAndReason(slotIndex, value, reason);
		_sData.Update();

		UISudokuSlot uiSlot = _uiSlotBoard.GetUISlot(slotIndex);
		// Add to undo
		if (_undoIndex >= _undoCmdList.Count) {
			_undoCmdList.Add(new UndoCommand());
		}
		_undoCmdList[_undoIndex].AddNewCommand(uiSlot.Value, uiSlot.FillReason, slotIndex);

		uiSlot.SetValueAndReason(value, reason);

		if (reason != FillReason.QuestionInput) {
			AddSolutionLog(rowIndex, columnIndex, value, reason);
		}
	}

	private void FillSolutionIntoSlot(int slotIndex, int value, FillReason reason) {
		SudokuUtility.ConvertToIndex(slotIndex, out int rowIndex, out int columnIndex, out int _);

		FillSolutionIntoSlot(rowIndex, columnIndex, value, reason);
	}

	private void AddSolutionLog(int rowIndex, int columnIndex, int value, FillReason reason) {
		UISolutionLog newLog = Instantiate(_solutionLogRes, _goSolutionLogRoot.transform);
		newLog.SetFillReason(rowIndex, columnIndex, value, reason);
		_solutionLogList.Add(newLog);
	}

	private void ClearLogData() {
		for (int i = 0; i < _solutionLogList.Count; i++) {
			Destroy(_solutionLogList[i].gameObject);
		}

		_solutionLogList.Clear();
	}
	#endregion
}
