using System;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowDifficulty : UIGenericWindow {
    #region Serialized Fields
    [SerializeField]
    private Button _btnEasy = null;

    [SerializeField]
    private Button _btnNormal = null;

    [SerializeField]
    private Button _btnHard = null;

    [SerializeField]
    private Button _btnCancel = null;
    #endregion

    #region Exposed Fields
    public override string WindowName => SystemDefine.UI_WINDOW_NAME_DIFFICULTY;
    #endregion

    #region Internal Fields
    private Action<int> _onclickActionDifficulty = null;
    #endregion

    #region Override Methods
    protected override void OnWindowAwake() {
        _btnEasy.onClick.AddListener(() => ButtonDifficultyOnClick((int) Difficulty.Easy));
        _btnNormal.onClick.AddListener(() => ButtonDifficultyOnClick((int) Difficulty.Normal));
        _btnHard.onClick.AddListener(() => ButtonDifficultyOnClick((int) Difficulty.Hard));
        _btnCancel.onClick.AddListener(ButtonCancleOnClick);
    }

    protected override void OnWindowDestroy() {
        _btnEasy.onClick.RemoveAllListeners();
        _btnNormal.onClick.RemoveAllListeners();
        _btnHard.onClick.RemoveAllListeners();
        _btnCancel.onClick.RemoveAllListeners();
    }
    #endregion

    #region UI Button Handlings
    private void ButtonDifficultyOnClick(int difficulty) {
        if (_onclickActionDifficulty != null) {
            _onclickActionDifficulty(difficulty);
        }

        Show(false, false).DoNotAwait();
    }

    private void ButtonCancleOnClick() {
        Show(false, false).DoNotAwait();
    }
    #endregion

    #region APIs
    public void SetDifficultyOnClickAction(Action<int> action) {
        _onclickActionDifficulty = action;
    }
    #endregion
}
