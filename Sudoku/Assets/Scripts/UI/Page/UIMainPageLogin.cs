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
    private float _effectTotalTime = 1.0f;

    [SerializeField]
    private float _effectDuration = 0.1f;
    #endregion

    #region Internal Fields
    private bool _playingEffect = false;
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

    #region Internal Fields
    private async void EnterToMainPage() {
        if (_playingEffect) {
            return;
        }

        // Play text effect
        this.ShowMask(true);
        await PlayTextEffect();

        // Enter page
        _controller.EnterPage((int) UIMain.PageType.Menu).DoNotAwait();
    }

    private async Task PlayTextEffect() {
        if (_playingEffect) {
            return;
        }

        _playingEffect = true;

        _tweenTapToStart.SetAlphaRange(1.0f, 1.0f);
        _tweenTapToStart.SetDuration(0);
        _tweenTapToStart.RestartTween();

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < _effectTotalTime) {
            bool active = (int)((Time.realtimeSinceStartup - startTime) / _effectDuration) % 2 == 1;
            _goTapToStart.SetActive(active);
            await Task.Delay(1);
        }
        _goTapToStart.SetActive(true);

        _playingEffect = false;
    }
    #endregion
}
