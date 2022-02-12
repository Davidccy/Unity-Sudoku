using UnityEngine;
using UnityEngine.UI;

public class UIMainPageSolver : IUIMainPageBase {
    #region Serialized Fields
    [SerializeField]
    private Button _btnBack = null;
    [SerializeField]
    private Button _btnOption = null;

    [SerializeField]
    private Canvas _rootCanvas = null;

    [SerializeField]
    private RectTransform _rectUITop = null;
    [SerializeField]
    private RectTransform _rectUIMiddleTop = null;
    [SerializeField]
    private RectTransform _rectUIMiddleBottom = null;
    [SerializeField]
    private RectTransform _rectUIBottom = null;
    #endregion

    #region Mono Behavoiur Hooks
    private void Awake() {
        _btnBack.onClick.AddListener(ButtonBackOnClick);
        _btnOption.onClick.AddListener(ButtonOptionOnClick);

        // Initialize size delat of UI Bottom
        RectTransform rtCanvas = _rootCanvas.transform as RectTransform;
        float remainedSpace = rtCanvas.sizeDelta.y - _rectUITop.sizeDelta.y - _rectUIMiddleTop.sizeDelta.y - _rectUIMiddleBottom.sizeDelta.y;
        _rectUIBottom.sizeDelta = new Vector2(_rectUIBottom.sizeDelta.x, remainedSpace);
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
