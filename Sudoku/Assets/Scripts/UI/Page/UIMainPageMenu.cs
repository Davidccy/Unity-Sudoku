using UnityEngine;
using UnityEngine.UI;

public class UIMainPageMenu : IUIMainPageBase {
    #region Serialized Fields
    [SerializeField]
    private Button _btnSolver = null;

    [SerializeField]
    private Button _btnGenerator = null;

    [SerializeField]
    private Button _btnOption = null;
    #endregion

    #region Mono Behavoiur Hooks
    private void Awake() {
        _btnSolver.onClick.AddListener(ButtonSolverOnClick);
        _btnGenerator.onClick.AddListener(ButtonGeneratorOnClick);
        _btnOption.onClick.AddListener(ButtonOptionOnClick);
    }
    #endregion

    #region UI Button Handlings
    private void ButtonSolverOnClick() {
        _controller.EnterPage((int) UIMain.PageType.Solver).DoNotAwait();
    }

    private void ButtonGeneratorOnClick() {
        _controller.EnterPage((int) UIMain.PageType.Generator).DoNotAwait();
    }

    private void ButtonOptionOnClick() {
        UIWindowManager.Instance.OpenWindow(SystemDefine.UI_WINDOW_NAME_OPTION).DoNotAwait();
    }
    #endregion
}
