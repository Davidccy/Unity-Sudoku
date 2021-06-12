using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SudokuSolver : MonoBehaviour {
	#region Serialized Fields
	[Header("Option Menu")]
	[SerializeField]
	private Button _btnOptionMenu = null;
	[SerializeField]
	private RectTransform _rtOptionMenu = null;
	[SerializeField]
	private CanvasGroup _cgOption = null;	

	[Header("Reset Menu")]
	[SerializeField]
	private Button _btnReset = null;
	[SerializeField]
	private Button _btnResetYes = null;
	[SerializeField]
	private Button _btnResetNo = null;
	[SerializeField]
	private RectTransform _rtResetMenu = null;
	[SerializeField]
	private CanvasGroup _cgReset = null;

	[Header("Serial Number Menu")]
	[SerializeField]
	private Button _btnSerialNumber = null;
	[SerializeField]
	private Button _btnSerialNumberYes = null;
	[SerializeField]
	private Button _btnSerialNumberNo = null;
	[SerializeField]
	private RectTransform _rtSerialNumberMenu = null;
	[SerializeField]
	private CanvasGroup _cgSerialNumber = null;
	[SerializeField]
	private List<TMP_InputField> _inputFieldSerialNumberList = null;	
		
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

	// reset menu
	private bool _resetMenuOpened = false;
	private Tween _tweenResetMenu = null;

	// option menu
	private bool _optionMenuOpened = false;
	private Tween _tweenOptionMenu = null;

	// serial number menu
	private bool _serialNumberMenuOpened = false;
	private Tween _tweenSerialNumberMenu = null;
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

	#region UI Button Call Backs
	public void ButtonOptionMenuOnClick() {
		ShowOptionMenu(!_optionMenuOpened, false).DoNotAwait();
	}

	public void ButtonResetOnClick() {
		ShowResetMenu(!_resetMenuOpened, false).DoNotAwait();
	}

	public void ButtonResetYesOnClick() {
		ResetData();
		ShowResetMenu(false, false).DoNotAwait();
	}

	public void ButtonResetNoOnClick() {
		ShowResetMenu(false, false).DoNotAwait();
	}

	public void ButtonSerialNumberOnClick() {
		ShowSerialNumberMenu(!_serialNumberMenuOpened, false).DoNotAwait();
	}

	public void ButtonSerialNumberYesOnClick() {
		// check
		if (_inputFieldSerialNumberList == null || _inputFieldSerialNumberList.Count != SudokuUtility.PUZZLE_LENGTH) {
			Debug.LogErrorFormat("Null input field or unexpected count of input field");
			return;
		}

		for (int i = 0; i < _inputFieldSerialNumberList.Count; i++) {
			string input = _inputFieldSerialNumberList[i].text;
			if (input.Length != SudokuUtility.PUZZLE_LENGTH) {
				Debug.LogErrorFormat("Unexpected length of input in row {0}", i + 1);
				return;
			}
		}

		// fill input
		for (int i = 0; i < _inputFieldSerialNumberList.Count; i++) {
			string input = _inputFieldSerialNumberList[i].text;
			for (int j = 0; j < SudokuUtility.PUZZLE_LENGTH; j++) {
				int slotIndex = i * SudokuUtility.PUZZLE_LENGTH + j;
				int inputValue = int.Parse(input.Substring(j, 1));
				_sData.SetSlotValueAndReason(slotIndex, inputValue, FillReason.QuestionInput);
			}
		}

		RefreshUISlots();
		ShowSerialNumberMenu(false, false).DoNotAwait();
	}

	public void ButtonSerialNumberNoOnClick() {
		ShowSerialNumberMenu(false, false).DoNotAwait();
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

		FillSolutionIntoSlot(slot.SlotIndex, _selectingInput.InputValue, FillReason.QuestionInput);
		_undoIndex += 1;

		RefreshUndoButton();
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

		RefreshUISlots();
		RefreshUIInputs();
	}
	#endregion

	#region Internal Fields
	private void InitUI() {
		// option menu
		_btnOptionMenu.onClick.AddListener(ButtonOptionMenuOnClick);

		// reset menu
		_btnReset.onClick.AddListener(ButtonResetOnClick);
		_btnResetYes.onClick.AddListener(ButtonResetYesOnClick);
		_btnResetNo.onClick.AddListener(ButtonResetNoOnClick);

		// serial number menu
		_btnSerialNumber.onClick.AddListener(ButtonSerialNumberOnClick);
		_btnSerialNumberYes.onClick.AddListener(ButtonSerialNumberYesOnClick);
		_btnSerialNumberNo.onClick.AddListener(ButtonSerialNumberNoOnClick);

		// operation
		_btnUndo.onClick.AddListener(ButtonUndoOnClick);
		_btnFindAllSolution.onClick.AddListener(ButtonFindAllSolutionOnClick);
		_btnFindNextSolution.onClick.AddListener(ButtonFindNextSolutionOnClick);

		// log
		_btnLog.onClick.AddListener(ButtonLogOnClick);

		// slots		
		_uiSlotBoard.SetOnClickAction(ButtonSlotOnClick);

		// inputs
		_uiInputBoard.SetOnClickAction(ButtonInputOnClick);
	}

	private void ResetData() {
		// sudoku data
		ClearSudokuData();
		RefreshUISlots();

		// selected input
		_selectingInput = null;
		RefreshUIInputs();

		// undo
		ClearUndoData();
		RefreshUndoButton();

		// log
		ClearLogData();
	}

	private void ResetUI() {
		ShowOptionMenu(false, true, true).DoNotAwait();
		ShowResetMenu(false, true, true).DoNotAwait();
		ShowSerialNumberMenu(false, true, true).DoNotAwait();
		ShowLogMenu(false);
	}

	private void ClearSudokuData() {
		_sData = new SudokuData();
		_uiSlotBoard.SetSudokuData(_sData);
	}

	private void RefreshUISlots() {
		_uiSlotBoard.RefreshAllSlot();
	}

	private void RefreshUIInputs() {
		int selectingInput = _selectingInput == null ? -1 : _selectingInput.InputValue;
		_uiInputBoard.SetMarking(selectingInput);
	}

	private void ClearUndoData() {
		_undoIndex = 0;
		_undoCmdList.Clear();
	}

	private void RefreshUndoButton() {
		_btnUndo.interactable = _undoCmdList.Count > 0;
	}

	private async Task ShowOptionMenu(bool show, bool skipTween, bool forcibly = false) {
		if (!forcibly && _optionMenuOpened == show) {
			return;
		}

		// init tween
		if (_tweenOptionMenu != null && _tweenOptionMenu.IsActive() && _tweenOptionMenu.IsPlaying()) {
			return;
		}

		_cgOption.interactable = false;

		if (show) {
			_rtOptionMenu.gameObject.SetActive(true);
		}

		_cgOption.interactable = false;

		float startAlpha = show ? 0.0f : 1.0f;
		float goalAlpha = show ? 1.0f : 0.0f;
		float duration = skipTween ? 0.0f : 0.3f;

		_tweenOptionMenu = DOTween.To(
			() => startAlpha,
			(v) => {
				_cgOption.alpha = v;
				startAlpha = v;
			},
			goalAlpha, duration).SetUpdate(true);

		await _tweenOptionMenu.AsyncWaitForCompletion();

		if (!show) {
			_rtOptionMenu.gameObject.SetActive(false);
		}
		else {
			_cgOption.interactable = true;
		}

		_optionMenuOpened = show;
	}

	private async Task ShowResetMenu(bool show, bool skipTween, bool forcibly = false) {
		if (!forcibly && _resetMenuOpened == show) {
			return;
		}

		// init tween
		if (_tweenResetMenu != null && _tweenResetMenu.IsActive() && _tweenResetMenu.IsPlaying()) {
			return;
		}

		if (show) {
			_rtResetMenu.gameObject.SetActive(true);
		}

		_cgReset.blocksRaycasts = false;

		float startAlpha = show ? 0.0f : 1.0f;
		float goalAlpha = show ? 1.0f : 0.0f;
		float duration = skipTween ? 0.0f : 0.3f;

		_tweenResetMenu = DOTween.To(
			() => startAlpha,
			(v) => {
				_cgReset.alpha = v;
				startAlpha = v;
			},
			goalAlpha, duration).SetUpdate(true);

		await _tweenResetMenu.AsyncWaitForCompletion();

		if (!show) {
			_rtResetMenu.gameObject.SetActive(false);
		}
		else {
			_cgReset.blocksRaycasts = true;
		}

		_resetMenuOpened = show;
	}

	private async Task ShowSerialNumberMenu(bool show, bool skipTween, bool forcibly = false) {
		if (!forcibly && _serialNumberMenuOpened == show) {
			return;
		}

		// init tween
		if (_tweenSerialNumberMenu != null && _tweenSerialNumberMenu.IsActive() && _tweenSerialNumberMenu.IsPlaying()) {
			return;
		}

		if (show) {
			_rtSerialNumberMenu.gameObject.SetActive(true);
		}

		_cgSerialNumber.blocksRaycasts = false;

		float startAlpha = show ? 0.0f : 1.0f;
		float goalAlpha = show ? 1.0f : 0.0f;
		float duration = skipTween ? 0.0f : 0.3f;

		_tweenSerialNumberMenu = DOTween.To(
			() => startAlpha,
			(v) => {
				_cgSerialNumber.alpha = v;
				startAlpha = v;
			},
			goalAlpha, duration).SetUpdate(true);

		await _tweenSerialNumberMenu.AsyncWaitForCompletion();

		if (!show) {
			_rtSerialNumberMenu.gameObject.SetActive(false);
		}
		else {
			_cgSerialNumber.blocksRaycasts = true;
		}

		_serialNumberMenuOpened = show;
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
		}

		_undoIndex -= 1;
		_undoCmdList.RemoveAt(_undoCmdList.Count - 1);

		RefreshUISlots();
		RefreshUndoButton();
	}

	private void FindSolution(bool findAll) {
		bool foundSolution = false;
		if (findAll) {
			while (TryToFindNextStep()) {
				// repeat until find no step
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

		RefreshUndoButton();
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

		// update data
		_sData.SetSlotValueAndReason(slotIndex, value, reason);

		UISudokuSlot uiSlot = _uiSlotBoard.GetUISlot(slotIndex);
		// add to undo
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
