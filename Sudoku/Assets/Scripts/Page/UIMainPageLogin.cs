using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIMainPageLogin : IUIMainPageBase {
    #region Serialized Fields
    [SerializeField]
    private Button _btnLogin = null;

    [SerializeField]
    private UITweenCanvasAlpha _tweenTapToStart = null;

    [SerializeField]
    private GameObject _goTapToStart = null;

    [SerializeField]
    private float _effectDuration = 1.0f;
    #endregion

    #region Mono Behavoiur Hooks
    private void Awake() {
        _btnLogin.onClick.AddListener(ButtonLoginOnClick);
    }
    #endregion

    #region Override Methods
    protected override void Opened() {
        _tweenTapToStart.SetAlphaRange(1.0f, 0.4f);
        _tweenTapToStart.SetDuration(1.0f);
        _tweenTapToStart.RestartTween();
    }
    #endregion

    #region UI Button Handlings
    private void ButtonLoginOnClick() {
        EnterToMainPage();
    }
    #endregion

    private async void EnterToMainPage() {
        // play text effect
        await PlayTextEffect();

        // enter page
        _controller.EnterPage((int) UIMainPageController.PageType.Menu);
    }

    private async Task PlayTextEffect() {
        _tweenTapToStart.SetAlphaRange(1.0f, 1.0f);
        _tweenTapToStart.SetDuration(0);
        _tweenTapToStart.RestartTween();

        float passTime = 0.0f;
        while (passTime < _effectDuration) {
            _goTapToStart.SetActive(!_goTapToStart.activeSelf);
            await Task.Delay(1);
            passTime += Time.deltaTime;
        }
    }
}
