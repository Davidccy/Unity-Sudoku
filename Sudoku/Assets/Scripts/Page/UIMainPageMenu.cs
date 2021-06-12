using UnityEngine;
using UnityEngine.UI;

public class UIMainPageMenu : IUIMainPageBase {
    #region Serialized Fields
    [SerializeField]
    private Button _btnSolver = null;

    [SerializeField]
    private Button _btnGenerator = null;
    #endregion

    #region Mono Behavoiur Hooks
    private void Awake() {
        _btnSolver.onClick.AddListener(ButtonSolverOnClick);
        _btnGenerator.onClick.AddListener(ButtonGeneratorOnClick);
    }
    #endregion

    #region UI Button Handlings
    private void ButtonSolverOnClick() {
        _controller.EnterPage((int) UIMainPageController.PageType.Solver);
    }

    private void ButtonGeneratorOnClick() {
        _controller.EnterPage((int)UIMainPageController.PageType.Generator);
    }
    #endregion
}
