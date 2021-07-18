using UnityEngine;
using UnityEngine.UI;

public class UIMainPageGenerator : IUIMainPageBase {
    #region Serialized Fields
    [SerializeField]
    private Button _btnBack = null;

    [SerializeField]
    private Button _btnOption = null;
    #endregion

    #region Mono Behavoiur Hooks
    private void Awake() {
        _btnBack.onClick.AddListener(ButtonBackOnClick);
        _btnOption.onClick.AddListener(ButtonOptionOnClick);
    }

    private void OnDestroy() {
        _btnBack.onClick.RemoveAllListeners();
        _btnOption.onClick.RemoveAllListeners();
    }
    #endregion

    #region UI Button Handlings
    private void ButtonBackOnClick() {
        _controller.EnterPage((int) UIMain.PageType.Menu).DoNotAwait();
    }

    private void ButtonOptionOnClick() {
        UIWindowManager.Instance.OpenWindow(SystemDefine.UI_WINDOW_NAME_OPTION).DoNotAwait();
    }
    #endregion
}
