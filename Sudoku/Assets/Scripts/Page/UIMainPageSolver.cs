using UnityEngine;
using UnityEngine.UI;

public class UIMainPageSolver : IUIMainPageBase {
    #region Serialized Fields
    [SerializeField]
    private Button _btnBack = null;
    #endregion

    #region Mono Behavoiur Hooks
    private void Awake() {
        _btnBack.onClick.AddListener(ButtonBackOnClick);
    }
    #endregion

    #region UI Button Handlings
    private void ButtonBackOnClick() {
        _controller.EnterPage((int) UIMainPageController.PageType.Menu);
    }
    #endregion
}
