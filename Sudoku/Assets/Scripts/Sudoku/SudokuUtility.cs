using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SudokuData {
    /* Index in Sudoku Data:
	 * 
	 * 	  	     column 0  column 1  column 2  column 3  column 4  column 5  column 6  column 7  column 8
	 * 	             |         |         |         |         |         |         |         |         |
	 *               v         v         v         v         v         v         v         v         v
	 * row 0 =>  0(0, 0),  1(0, 1),  2(0, 2),  3(0, 3),  4(0, 4),  5(0, 5),  6(0, 6),  7(0, 7),  8(0, 8)
	 * row 1 =>  9(1, 0), 10(1, 1), 11(1, 2), 12(1, 3), 13(1, 4), 14(1, 5), 15(1, 6), 16(1, 7), 17(1, 8)
	 * row 2 => 18(2, 0), 19(2, 1), 20(2, 2), 21(2, 3), 22(2, 4), 23(2, 5), 24(2, 6), 25(2, 7), 26(2, 8)
	 * row 3 => 27(3, 0), 28(3, 1), 29(3, 2), 30(3, 3), 31(3, 4), 32(3, 5), 33(3, 6), 34(3, 7), 35(3, 8)
	 * row 4 => 36(4, 0), 37(4, 1), 38(4, 2), 39(4, 3), 40(4, 4), 41(4, 5), 42(4, 6), 43(4, 7), 44(4, 8)
	 * row 5 => 45(5, 0), 46(5, 1), 47(5, 2), 48(5, 3), 49(5, 4), 50(5, 5), 51(5, 6), 52(5, 7), 53(5, 8)
	 * row 6 => 54(6, 0), 55(6, 1), 56(6, 2), 57(6, 3), 58(6, 4), 59(6, 5), 60(6, 6), 61(6, 7), 62(6, 8)
	 * row 7 => 63(7, 0), 64(7, 1), 65(7, 2), 66(7, 3), 67(7, 4), 68(7, 5), 69(7, 6), 70(7, 7), 71(7, 8)
	 * row 8 => 72(8, 0), 73(8, 1), 74(8, 2), 75(8, 3), 76(8, 4), 77(8, 5), 78(8, 6), 79(8, 7), 80(8, 8)
	*/

    #region Internal Fields
    private List<SlotData> _slotDataList;		// 81 elements, slot index 0 ~ 80
	private List<NumberData> _numberDataList;	// 9 elements, base number 1 ~ 9 
	private List<RowData> _rowDataList;         // 9 elements, row index 0 ~ 8
	private List<ColumnData> _columnDataList;   // 9 elements, column index 0 ~ 8
	private List<SquareData> _squareDataList;   // 9 elements, square index 0 ~ 8

	private List<SlotData> _emptySlotDataList;	// Slot with no value
	private List<SlotData> _filledSlotDataList; // Slot filled with value

	private Action _onInputChanged = null;
    #endregion

    #region Initializations
    public SudokuData() {
		Init();
	}

	public SudokuData(CompleteData data, FillReason deafultReason) {
		Init();

		for (int i = 0; i < data.ArrayData.GetLength(0); i++) {
			for (int j = 0; j < data.ArrayData.GetLength(1); j++) {
				int value = data.ArrayData[i, j];
				SetSlotValueAndReason(i, j, value, deafultReason);
			}
		}

		Update();
	}
    #endregion

    #region Properties
    public List<SlotData> SlotDataList {
		get {
			return _slotDataList;
		}
	}

	public List<NumberData> NumberDataList {
		get {
			return _numberDataList;
		}
	}

	public List<RowData> RowDataList {
		get {
			return _rowDataList;
		}
	}

	public List<ColumnData> ColumnDataList {
		get {
			return _columnDataList;
		}
	}

	public List<SquareData> SquareDataList {
		get {
			return _squareDataList;
		}
	}
    #endregion

    #region APIs
    public void Init() {
		InitSlotData();
		InitNumberData();
		InitRowData();
		InitColumnData();
		InitSquareData();
	}

	public SudokuData GetClone() {
		SudokuData sData = new SudokuData();

		for (int i = 0; i < sData.SlotDataList.Count; i++) {
			SlotData slot = GetSlotData(i);
			sData.SetSlotValueAndReason(i, slot.Value, slot.Reason);
		}

		return sData;
	}

	public SlotData GetSlotData(int rowIndex, int columnIndex) {
		int slotIndex = SudokuUtility.ConvertToSlotIndex(rowIndex, columnIndex);
		return GetSlotData(slotIndex);
	}

	public SlotData GetSlotData(int slotIndex) {
		if (_slotDataList == null) {
			Debug.LogErrorFormat("Null slot data");
			return null;
		}

		if (slotIndex < 0 || _slotDataList.Count <= slotIndex) {
			Debug.LogErrorFormat("Invalid slot index {0} to get value, slot count = {1}", slotIndex, _slotDataList.Count);
			return null;
		}

		return _slotDataList[slotIndex];
	}

	public void SetInputChangedAction(Action action) {
		_onInputChanged = action;
	}

	public void SetSlotValueAndReason(int rowIndex, int columnIndex, int value, FillReason reason) {
		int slotIndex = SudokuUtility.ConvertToSlotIndex(rowIndex, columnIndex);
		SetSlotValueAndReason(slotIndex, value, reason);
	}

	public void SetSlotValueAndReason(int slotIndex, int value, FillReason reason) {
		_slotDataList[slotIndex].Value = value;
		_slotDataList[slotIndex].Reason = reason;

		if (_onInputChanged != null) {
			_onInputChanged();
		}
	}

	public bool HasEmptySlot() {
		return _emptySlotDataList.Count > 0;
	}

	public bool HasAnyInput() {
		return _filledSlotDataList.Count > 0;
	}

	public List<int> GetAllEmptySlotIndex() {
		List<int> a = _emptySlotDataList.Select(
			(item, index) => {
				return item.SlotIndex;
			})
			.ToList();
		return a;
	}

	public SlotData GetRandomEmptySlot() {
		if (_emptySlotDataList == null || _emptySlotDataList.Count <= 0) {
			return null;
		}

		int rndIndex = UnityEngine.Random.Range(0, _emptySlotDataList.Count);
		return _emptySlotDataList[rndIndex];
	}

	public List<int> GetAllFilledSlotIndex() {
		return _filledSlotDataList.Select((item, index) => item.SlotIndex).ToList();
	}

	public SlotData GetRandomFilledSlot() {
		if (_filledSlotDataList == null || _filledSlotDataList.Count <= 0) {
			return null;
		}

		int rndIndex = UnityEngine.Random.Range(0, _filledSlotDataList.Count);
		return _filledSlotDataList[rndIndex];
	}

	public void Update() {
		UpdateData();
	}

	public bool IsSolvable() {
		SudokuData cloneData = GetClone();
		cloneData.Update();

		while (cloneData.HasEmptySlot()) {
			SolutionInfo solution = SudokuUtility.FindSolution(cloneData);
			if (solution == null) {
				return false;
			}

			cloneData.SetSlotValueAndReason(solution.SlotData.SlotIndex, solution.Value, FillReason.None);
			cloneData.Update();
		}

		return true;
	}

	public bool FillAllSolution() {
		if (!IsSolvable()) {
			return false;
		}

		while (this.HasEmptySlot()) {
			SolutionInfo solution = SudokuUtility.FindSolution(this);
			SetSlotValueAndReason(solution.SlotData.SlotIndex, solution.Value, FillReason.PlayerInput);
			Update();
		}

		return true;
	}

	public void ClearAll() {
		Init();
	}

	public void Print() {
		Debug.LogErrorFormat("Print");
		string str = string.Empty;
		for (int i = 0; i < _slotDataList.Count; i++) {
			if (i == 0) {
				str = string.Format("{0}", _slotDataList[i].Value);
			}
			else if (i % SudokuUtility.PUZZLE_LENGTH == 0) {
				str = string.Format("{0}\n{1}", str, _slotDataList[i].Value);
			}
			else { 
				str = string.Format("{0} {1}", str, _slotDataList[i].Value);
			}
		}
		Debug.LogErrorFormat("{0}", str);
    }
    #endregion

    #region Internal Methods
    private void InitSlotData() {
		_slotDataList = new List<SlotData>();
		for (int i = 0; i < SudokuUtility.PUZZLE_LENGTH; i++) {
			for (int j = 0; j < SudokuUtility.PUZZLE_LENGTH; j++) {
				_slotDataList.Add(new SlotData {
					SlotIndex = i * SudokuUtility.PUZZLE_LENGTH + j,
					RowIndex = i,
					ColumnIndex = j,
					SquareIndex = (j / 3) + (i / 3) * 3,
				});
			}
		}

		_emptySlotDataList = new List<SlotData>();
		_filledSlotDataList = new List<SlotData>();
	}

	private void InitNumberData() {
		_numberDataList = new List<NumberData>();
		for (int i = 0; i < SudokuUtility.PUZZLE_LENGTH; i++) {
			_numberDataList.Add(new NumberData {
				BaseNumber = i + 1,
			});
		}
	}

	private void InitRowData() {
		_rowDataList = new List<RowData>();
		for (int i = 0; i < SudokuUtility.PUZZLE_LENGTH; i++) {
			RowData rowdata = new RowData();
			rowdata.Index = i;
			rowdata.SlotDataList = new List<SlotData>();
			for (int j = 0; j < SudokuUtility.PUZZLE_LENGTH; j++) {
				rowdata.SlotDataList.Add(_slotDataList[i * SudokuUtility.PUZZLE_LENGTH + j]);
			}
			_rowDataList.Add(rowdata);
		}
	}

	private void InitColumnData() {
		_columnDataList = new List<ColumnData>();
		for (int i = 0; i < SudokuUtility.PUZZLE_LENGTH; i++) {
			ColumnData columndata = new ColumnData();
			columndata.Index = i;
			columndata.SlotDataList = new List<SlotData>();
			for (int j = 0; j < SudokuUtility.PUZZLE_LENGTH; j++) {
				columndata.SlotDataList.Add(_slotDataList[i + j * SudokuUtility.PUZZLE_LENGTH]);
			}
			_columnDataList.Add(columndata);
		}
	}

	private void InitSquareData() {
		_squareDataList = new List<SquareData>();
		for (int i = 0; i < SudokuUtility.PUZZLE_LENGTH; i++) {
			SquareData squareData = new SquareData();
			squareData.Index = i;
			squareData.SlotDataList = new List<SlotData>();
			int squareStartIndex = (i % SudokuUtility.SQUARE_LENGTH) * SudokuUtility.SQUARE_LENGTH + (i / SudokuUtility.SQUARE_LENGTH) * SudokuUtility.PUZZLE_LENGTH * SudokuUtility.SQUARE_LENGTH; // 該九宮格的起始點(左上)的 Slot 的 Index, 用 0 ~ 80 表示
			for (int j = 0; j < SudokuUtility.PUZZLE_LENGTH; j++) {
				int slotIndex = squareStartIndex + (j % SudokuUtility.SQUARE_LENGTH) + ((j / SudokuUtility.SQUARE_LENGTH) * SudokuUtility.PUZZLE_LENGTH); // 該九宮格的目標點位置, 用 0 ~ 80 表示
				squareData.SlotDataList.Add(_slotDataList[slotIndex]);
			}
			_squareDataList.Add(squareData);
		}
	}

	private void UpdateData() {
        UpdateNumberData();
        UpdateRowData();
		UpdateColumnData();
		UpdateSquareData();
		UpdateEmptyAndFilledSlotData();
	}

	private void UpdateNumberData() {
		// Update content of slot
		// Update conditions of number 1 ~ 9

		for (int i = 0; i < SudokuUtility.PUZZLE_LENGTH; i++) {
			_numberDataList[i].FilledRow.Clear();
			_numberDataList[i].FilledColumn.Clear();
			_numberDataList[i].FilledSquare.Clear();
		}

		int rowIndex;
		int columnIndex;
		int squareIndex;
		int value;
		for (int i = 0; i < _slotDataList.Count; i++) {
			// Calculate the index of row, column, square of slot
			rowIndex = i / SudokuUtility.PUZZLE_LENGTH;
			columnIndex = i % SudokuUtility.PUZZLE_LENGTH;
			squareIndex = (rowIndex / SudokuUtility.SQUARE_LENGTH) * SudokuUtility.SQUARE_LENGTH + (columnIndex / SudokuUtility.SQUARE_LENGTH);

			value = _slotDataList[i].Value;

			if (value < 0 || value > SudokuUtility.PUZZLE_LENGTH) {
				Debug.LogErrorFormat("Invalid input {0} in slot ({1}, {2})", value, rowIndex, columnIndex);
				return;
			}

			// Update numbers' condition
			// Ex: number 2 is now filled in row 1, row 3
			//								 column 6,
			//								 square 2, square 8
			// Need to check the condition of all numbers
			// The condition is not correct if a number appears in same row, column and square several times
			if (value == 0) {
				continue;
			}

			int numberDataIndex = value - 1;
			NumberData numberData = _numberDataList[numberDataIndex];
			if (numberData.FilledRow.Contains(rowIndex)) {
				Debug.LogErrorFormat("Invalid value: {0} is appreaed repeatedly in row {1}, i = {2}", value, rowIndex, i);
				return;
			}
			numberData.FilledRow.Add(rowIndex);

			if (numberData.FilledColumn.Contains(columnIndex)) {
				Debug.LogErrorFormat("Invalid value: {0} is appreaed repeatedly in column {1}", value, columnIndex);
				return;
			}
			numberData.FilledColumn.Add(columnIndex);

			if (numberData.FilledSquare.Contains(squareIndex)) {
				Debug.LogErrorFormat("Invalid value: {0} is appreaed repeatedly in square {1}", value, squareIndex);
				return;
			}
			numberData.FilledSquare.Add(squareIndex);
		}
	}

	private void UpdateRowData() {
		for (int i = 0; i < _rowDataList.Count; i++) {
			_rowDataList[i].Update();
		}
	}

	private void UpdateColumnData() {
		for (int i = 0; i < _columnDataList.Count; i++) {
			_columnDataList[i].Update();
		}
	}

	private void UpdateSquareData() {
		for (int i = 0; i < _squareDataList.Count; i++) {
			_squareDataList[i].Update();
		}
	}

	private void UpdateEmptyAndFilledSlotData() {
		_emptySlotDataList = new List<SlotData>();
		_filledSlotDataList = new List<SlotData>();
		for (int i = 0; i < _slotDataList.Count; i++) {
			if (_slotDataList[i].Value == 0) {
				_emptySlotDataList.Add(_slotDataList[i]);
			}
			else {
				_filledSlotDataList.Add(_slotDataList[i]);
			}
		}
	}
    #endregion
}

public class SlotData {
	// Row index, column index, square index of this slot,
	// and it's filled number

	public int SlotIndex;
	public int RowIndex;
	public int ColumnIndex;
	public int SquareIndex;
	public int Value;
	public FillReason Reason = FillReason.None;
}

public class NumberData {
	// Which row, column and square is filled with base number

	public int BaseNumber;
	public List<int> FilledRow = new List<int>();
	public List<int> FilledColumn = new List<int>();
	public List<int> FilledSquare = new List<int>();
}

public class RowData {
	public int Index;
	public List<SlotData> SlotDataList = new List<SlotData>();

	public List<int> RemainedNumber = new List<int>();
	public List<int> FilledNumber = new List<int>();
	public List<SlotData> RemainedSlot = new List<SlotData>();
	public List<SlotData> FilledSlot = new List<SlotData>();

	public void Update() {
		RemainedNumber.Clear();
		FilledNumber.Clear();
		RemainedSlot.Clear();
		FilledSlot.Clear();

		if (SlotDataList == null) {
			return;
		}

		List<int> remainedNumber = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		for (int i = 0; i < SlotDataList.Count; i++) {
			if (SlotDataList[i].Value == 0) {
				RemainedSlot.Add(SlotDataList[i]);
			}
			else {
				FilledNumber.Add(SlotDataList[i].Value);
				FilledSlot.Add(SlotDataList[i]);

				remainedNumber.Remove(SlotDataList[i].Value);
			}
		}
		RemainedNumber = remainedNumber;
	}
}

public class ColumnData {
	public int Index;
	public List<SlotData> SlotDataList = new List<SlotData>();

	public List<int> RemainedNumber = new List<int>();
	public List<int> FilledNumber = new List<int>();
	public List<SlotData> RemainedSlot = new List<SlotData>();
	public List<SlotData> FilledSlot = new List<SlotData>();

	public void Update() {
		RemainedNumber.Clear();
		FilledNumber.Clear();
		RemainedSlot.Clear();
		FilledSlot.Clear();

		if (SlotDataList == null) {
			return;
		}

		List<int> remainedNumber = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		for (int i = 0; i < SlotDataList.Count; i++) {
			if (SlotDataList[i].Value == 0) {
				RemainedSlot.Add(SlotDataList[i]);
			}
			else {
				FilledNumber.Add(SlotDataList[i].Value);
				FilledSlot.Add(SlotDataList[i]);

				remainedNumber.Remove(SlotDataList[i].Value);
			}
		}
		RemainedNumber = remainedNumber;
	}
}

public class SquareData {
	public int Index;
	public List<SlotData> SlotDataList = new List<SlotData>();

	public List<int> RemainedNumber = new List<int>();
	public List<int> FilledNumber = new List<int>();
	public List<SlotData> RemainedSlot = new List<SlotData>();
	public List<SlotData> FilledSlot = new List<SlotData>();

	public void Update() {
		RemainedNumber.Clear();
		FilledNumber.Clear();
		RemainedSlot.Clear();
		FilledSlot.Clear();

		if (SlotDataList == null) {
			return;
		}

		List<int> remainedNumber = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		for (int i = 0; i < SlotDataList.Count; i++) {
			if (SlotDataList[i].Value == 0) {
				RemainedSlot.Add(SlotDataList[i]);
			}
			else {
				FilledNumber.Add(SlotDataList[i].Value);
				FilledSlot.Add(SlotDataList[i]);

				remainedNumber.Remove(SlotDataList[i].Value);
			}
		}
		RemainedNumber = remainedNumber;
	}
}

public enum FillReason {
	None,
	QuestionInput,
	RowRemain,
	ColumnRemain,
	SquareRemain,
	SlotExclude,
	CommonSolution,
	PlayerInput,
}

public class UndoCommand {
	public List<int> ValueList = new List<int>();
	public List<FillReason> FillReasonList = new List<FillReason>();
	public List<int> SlotIndexList = new List<int>();

	public void AddNewCommand(int value, FillReason reason, int slotIndex) {
		ValueList.Add(value);
		FillReasonList.Add(reason);
		SlotIndexList.Add(slotIndex);
	}
}

public class SolutionInfo {
	public SlotData SlotData;
	public FillReason Reason;
	public int Value;
}

public class CompleteData {
	public string Name;
	public int[,] ArrayData;
}

public static class SudokuUtility {
	public static readonly List<CompleteData> CompleteDataList = new List<CompleteData> {
		new CompleteData {
			Name = "Data1",
			ArrayData = new int[9, 9] {
					{6, 9, 3, 8, 5, 2, 7, 4, 1},
					{2, 5, 1, 6, 4, 7, 8, 9, 3},
					{8, 4, 7, 3, 1, 9, 5, 2, 6},
					{9, 8, 6, 4, 3, 1, 2, 5, 7},
					{1, 7, 5, 2, 8, 6, 9, 3, 4},
					{3, 2, 4, 7, 9, 5, 1, 6, 8},
					{4, 3, 2, 9, 7, 8, 6, 1, 5},
					{5, 6, 8, 1, 2, 3, 4, 7, 9},
					{7, 1, 9, 5, 6, 4, 3, 8, 2}
			},
		},
		new CompleteData {
			Name = "Data2",
			ArrayData = new int[9, 9] {
					{9, 3, 4, 2, 5, 7, 1, 6, 8},
					{2, 8, 5, 1, 6, 9, 3, 4, 7},
					{6, 1, 7, 3, 4, 8, 2, 5, 9},
					{3, 6, 8, 7, 9, 1, 5, 2, 4},
					{1, 4, 9, 5, 2, 3, 7, 8, 6},
					{5, 7, 2, 4, 8, 6, 9, 1, 3},
					{7, 2, 1, 8, 3, 4, 6, 9, 5},
					{8, 5, 6, 9, 7, 2, 4, 3, 1},
					{4, 9, 3, 6, 1, 5, 8, 7, 2}
			},
		}
	};

	public const int SQUARE_LENGTH = 3;
	public const int PUZZLE_LENGTH = 9;

	public static Color GetFillReasonColor(FillReason reason) {
		Color c = Color.black;

		switch (reason) {
			case FillReason.RowRemain:
			case FillReason.ColumnRemain:
			case FillReason.SquareRemain:
			case FillReason.SlotExclude:
			case FillReason.CommonSolution:
			case FillReason.PlayerInput:
				c = Color.red;
				break;
			case FillReason.None:
			case FillReason.QuestionInput:
			default:
				c = Color.black;
				break;
		}

		return c;
	}

	public static int ConvertToSlotIndex(int rowIndex, int columnIndex) {
		return rowIndex * PUZZLE_LENGTH + columnIndex;
	}

	public static void ConvertToIndex(int slotIndex, out int rowIndex, out int columnIndex, out int squareIndex) {
		rowIndex = slotIndex / PUZZLE_LENGTH;
		columnIndex = slotIndex % PUZZLE_LENGTH;
		squareIndex = 3 * (slotIndex % (PUZZLE_LENGTH * SQUARE_LENGTH)) + (slotIndex % PUZZLE_LENGTH) / SQUARE_LENGTH;
	}

	public static bool IsSudokuDataComplete(SudokuData sData) {
		if (sData == null) {
			return false;
		}

		for (int i = 0; i < PUZZLE_LENGTH; i++) {
			if (!IsRowComplete(sData, i)) {
				return false;
			}
		}

		for (int i = 0; i < PUZZLE_LENGTH; i++) {
			if (!IsColumnComplete(sData, i)) {
				return false;
			}
		}

		for (int i = 0; i < PUZZLE_LENGTH; i++) {
			if (!IsSquareComplete(sData, i)) {
				return false;
			}
		}

		return true;
	}

	public static bool IsRowComplete(SudokuData sData, int rowIndex) {
		List<int> inputs = new List<int>();
		int slotIndex;
		for (int i = 0; i < PUZZLE_LENGTH; i++) {
			slotIndex = rowIndex * PUZZLE_LENGTH + i;

			// The row is failed if any value is 0
			SlotData slotData = sData.GetSlotData(slotIndex);
			if (slotData == null || slotData.Value == 0) {
				return false;
			}

			inputs.Add(slotData.Value);
		}

		return IsListComplete(inputs);
	}

	public static bool IsColumnComplete(SudokuData sData, int columnIndex) {
		List<int> inputs = new List<int>();
		int slotIndex;
		for (int i = 0; i < PUZZLE_LENGTH; i++) {
			slotIndex = columnIndex + PUZZLE_LENGTH * i;

			// The column is failed if any value is 0
			SlotData slotData = sData.GetSlotData(slotIndex);
			if (slotData == null || slotData.Value == 0) {
				return false;
			}

			inputs.Add(slotData.Value);
		}

		return IsListComplete(inputs);
	}

	public static bool IsSquareComplete(SudokuData sData, int squareIndex) {
		List<int> inputs = new List<int>();
		int slotIndex;

		// NOTE:
		// This step maybe wrong if shape of sudoku is not 9 * 9 anymore
		int squareStartIndex = (squareIndex % SQUARE_LENGTH) * SQUARE_LENGTH + (squareIndex / SQUARE_LENGTH) * PUZZLE_LENGTH * SQUARE_LENGTH;
		for (int i = 0; i < PUZZLE_LENGTH; i++) {
			slotIndex = squareStartIndex + (i % SQUARE_LENGTH) + ((i / SQUARE_LENGTH) * PUZZLE_LENGTH);

			// The square is not complete if any value is 0
			SlotData slotData = sData.GetSlotData(slotIndex);
			if (slotData == null || slotData.Value == 0) {
				return false;
			}

			inputs.Add(slotData.Value);
		}

		return IsListComplete(inputs);
	}

	private static bool IsListComplete(List<int> inputs) {
		// Check inputs are in range 1 ~ 9
		for (int i = 0; i < PUZZLE_LENGTH; i++) {
			if (!inputs.Contains(i + 1)) {
				return false;
			}
		}
		return true;
	}

	public static bool IsNumberContainedInRow(SudokuData sData, int number, int rowIndex) {
		if (rowIndex >= PUZZLE_LENGTH) {
			Debug.LogErrorFormat("Unexpected row index: {0}", rowIndex);
			return false;
		}

		NumberData numberData = sData.NumberDataList[number - 1];
		return numberData.FilledRow.Contains(rowIndex);
	}

	public static bool IsNumberContainedInColumn(SudokuData sData, int number, int columnIndex) {
		if (columnIndex >= PUZZLE_LENGTH) {
			Debug.LogErrorFormat("Unexpected column index: {0}", columnIndex);
			return false;
		}

		NumberData numberData = sData.NumberDataList[number - 1];
		return numberData.FilledColumn.Contains(columnIndex);
	}

	public static bool IsNumberContainedInSquare(SudokuData sData, int number, int squareIndex) {
		if (squareIndex >= PUZZLE_LENGTH) {
			Debug.LogErrorFormat("Unexpected square index: {0}", squareIndex);
			return false;
		}

		NumberData numberData = sData.NumberDataList[number - 1];
		return numberData.FilledSquare.Contains(squareIndex);
	}

	public static bool IsSlotSameRow(List<SlotData> slots) {
		if (slots == null) {
			Debug.LogErrorFormat("Null slot data list");
			return false;
		}

		if (slots.Count <= 1) {
			Debug.LogErrorFormat("Unexpected count of slot data list");
			return false;
		}

		int rowIndex = -1;
		for (int i = 0; i < slots.Count; i++) {
			if (rowIndex == -1) {
				rowIndex = slots[i].RowIndex;
				continue;
			}

			if (rowIndex != slots[i].RowIndex) {
				return false;
			}
		}

		return true;
	}

	public static bool IsSlotSameColumn(List<SlotData> slots) {
		if (slots == null) {
			Debug.LogErrorFormat("Null slot data list");
			return false;
		}

		if (slots.Count <= 1) {
			Debug.LogErrorFormat("Unexpected count of slot data list");
			return false;
		}

		int columnIndex = -1;
		for (int i = 0; i < slots.Count; i++) {
			if (columnIndex == -1) {
				columnIndex = slots[i].ColumnIndex;
				continue;
			}

			if (columnIndex != slots[i].ColumnIndex) {
				return false;
			}
		}

		return true;
	}

	private static List<Func<SudokuData, SolutionInfo>> _solutionMethods = null; 
	public static SolutionInfo FindSolution(SudokuData sData) {
		if (_solutionMethods == null) {
			_solutionMethods = new List<Func<SudokuData, SolutionInfo>>();

			_solutionMethods.Add(FindByMethodRowRemain);
			_solutionMethods.Add(FindByMethodColumnRemain);
			_solutionMethods.Add(FindByMethodSquareRemain);

			_solutionMethods.Add(FindByExclusion);

			_solutionMethods.Add(FindByMethodIntersection);
		}

		for (int i = 0; i < _solutionMethods.Count; i++) {
			SolutionInfo solution = _solutionMethods[i](sData);
			if (solution != null) {
				return solution;
			}
		}

		return null;
	}

	public static SolutionInfo FindByMethodRowRemain(SudokuData sData) {
		//////////////////////////////////////
		// Method : 檢查是否只缺一筆 part 1 //
		//////////////////////////////////////

		// 行當中未填的欄位是否只剩一筆
		List<RowData> rowDataList = sData.RowDataList;
		for (int i = 0; i < rowDataList.Count; i++) {
			if (rowDataList[i].RemainedSlot.Count == 1) {
				SlotData remainedSlot = rowDataList[i].RemainedSlot[0];
				int remainedNumber = rowDataList[i].RemainedNumber[0];
				return new SolutionInfo { SlotData = remainedSlot, Reason = FillReason.RowRemain, Value = remainedNumber };
			}
		}

		return null;
	}

	public static SolutionInfo FindByMethodColumnRemain(SudokuData sData) {
		//////////////////////////////////////
		// Method : 檢查是否只缺一筆 part 2 //
		//////////////////////////////////////

		// 列當中未填的欄位是否只剩一筆
		List<ColumnData> columnDataList = sData.ColumnDataList;
		for (int i = 0; i < columnDataList.Count; i++) {
			if (columnDataList[i].RemainedSlot.Count == 1) {
				SlotData remainedSlot = columnDataList[i].RemainedSlot[0];
				int remainedNumber = columnDataList[i].RemainedNumber[0];
				return new SolutionInfo { SlotData = remainedSlot, Reason = FillReason.ColumnRemain, Value = remainedNumber };
			}
		}

		return null;
	}

	public static SolutionInfo FindByMethodSquareRemain(SudokuData sData) {
		//////////////////////////////////////
		// Method : 檢查是否只缺一筆 part 3 //
		//////////////////////////////////////

		// 九宮格當中未填的欄位是否只剩一筆
		List<SquareData> squareDataList = sData.SquareDataList;
		for (int i = 0; i < squareDataList.Count; i++) {
			if (squareDataList[i].RemainedSlot.Count == 1) {
				SlotData remainedSlot = squareDataList[i].RemainedSlot[0];
				int remainedNumber = squareDataList[i].RemainedNumber[0];
				return new SolutionInfo { SlotData = remainedSlot, Reason = FillReason.SquareRemain, Value = remainedNumber };
			}
		}

		return null;
	}

	public static SolutionInfo FindByExclusion(SudokuData sData) {
		/////////////////////////////////////////////////////////
		// Method : 排除法, 已填了數字的欄位, 				   //
		//          與該數字同行同列的位置不會再出現同樣的數字 //
		/////////////////////////////////////////////////////////

		// 需要建立該數字已出現在那行那列的資料
		// 檢查 1 ~ 9, 在每個九宮格中的 "可能出現位置" 是不是只剩下一個 ?
		for (int i = 0; i < PUZZLE_LENGTH; i++) {
			// For 1 ~ 9
			int checkNumber = i + 1;

			// NOTE: 若某一個數字在一個九宮格的可能出現欄位在同一行 or 列上
			//       則可以視為此行 or 列必定有一個該數字, 進而排除其他欄位
			bool recheck = true;
			List<int> tempRow = new List<int>();
			List<int> tempColumn = new List<int>();

			while (recheck) {
				recheck = false;
				for (int squareIndex = 0; squareIndex < PUZZLE_LENGTH; squareIndex++) {
					// For 每一個九宮格
					if (SudokuUtility.IsNumberContainedInSquare(sData, checkNumber, squareIndex)) {
						continue;
					}

					// For 九宮格裡的每一小格
					List<SlotData> fillableSlot = new List<SlotData>();
					int squareStartIndex = (squareIndex % SQUARE_LENGTH) * SQUARE_LENGTH + (squareIndex / SQUARE_LENGTH) * PUZZLE_LENGTH * SQUARE_LENGTH; // 該九宮格的起始點(左上)的 Slot 的 Index, 用 0 ~ 80 表示
					for (int j = 0; j < PUZZLE_LENGTH; j++) {
						int slotIndex = squareStartIndex + (j % SQUARE_LENGTH) + ((j / SQUARE_LENGTH) * PUZZLE_LENGTH); // 該九宮格的目標點位置, 用 0 ~ 80 表示
						SlotData slotData = sData.SlotDataList[slotIndex];
						if (slotData.Value != 0) {
							continue;
						}

						if (IsNumberContainedInRow(sData, checkNumber, slotData.RowIndex)) {
							continue;
						}

						if (tempRow.Contains(slotData.RowIndex)) {
							continue;
						}

						if (IsNumberContainedInColumn(sData, checkNumber, slotData.ColumnIndex)) {
							continue;
						}

						if (tempColumn.Contains(slotData.ColumnIndex)) {
							continue;
						}

						fillableSlot.Add(slotData);
					}

					if (fillableSlot.Count == 0) {
						// Do nothing
					}
					else if (fillableSlot.Count == 1) {
						return new SolutionInfo { SlotData = fillableSlot[0], Reason = FillReason.SlotExclude, Value = checkNumber };
					}
					else {
						if (SudokuUtility.IsSlotSameRow(fillableSlot)) {
							// 若這些候補 Slot 位置剛好在同一行上
							tempRow.Add(fillableSlot[0].RowIndex);
							recheck = true;
						}
						else if (SudokuUtility.IsSlotSameColumn(fillableSlot)) {
							// 若這些候補 Slot 位置剛好在同一列上
							tempColumn.Add(fillableSlot[0].ColumnIndex);
							recheck = true;
						}
					}
				}
			}
		}

		return null;
	}

	public static SolutionInfo FindByMethodIntersection(SudokuData sData) {
		/////////////////////////////////////////////////////////////////
		// Method : 交集法											   //
		//          對某一個欄位來說 				                   //
		//          若該行該列該九宮格所缺少的數字集合的交集為一個數字 //
		//          則該數字為解                                       //
		/////////////////////////////////////////////////////////////////

		List<SlotData> slotDataList = sData.SlotDataList;
		for (int i = 0; i < slotDataList.Count; i++) {
			// 若已填入數字則略過
			SlotData slotData = slotDataList[i];
			if (slotData.Value != 0) {
				continue;
			}

			// 該欄位的行, 列, 九宮格 Index 值
			int rowIndex = slotData.RowIndex;
			int columnIndex = slotData.ColumnIndex;
			int squareIndex = slotData.SquareIndex;

			List<int> commonSolutionList = new List<int>();
			// 從 Row 的所缺少的數字開始計算, 查看是否也同時存在於 Column 和 Square 的缺少數字清單中
			// 若有, 則加入 commonSolutionList 清單內
			for (int j = 0; j < sData.RowDataList[rowIndex].RemainedNumber.Count; j++) {
				int remainNumberInRow = sData.RowDataList[rowIndex].RemainedNumber[j];
				if (sData.ColumnDataList[columnIndex].RemainedNumber.Contains(remainNumberInRow) &&
					sData.SquareDataList[squareIndex].RemainedNumber.Contains(remainNumberInRow)) {
					commonSolutionList.Add(remainNumberInRow);
				}
			}

			// 檢查最後加入到 commonSolutionList 的數字是否只有一筆
			if (commonSolutionList.Count == 1) {
				return new SolutionInfo { SlotData = slotData, Reason = FillReason.CommonSolution, Value = commonSolutionList[0] };
			}
		}

		return null;
	}
}
