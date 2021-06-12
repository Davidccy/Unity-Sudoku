using DG.Tweening;
using UnityEngine;

public class UITweenCanvasAlpha : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    private CanvasGroup _cg = null;

    [SerializeField]
    private float _alphaMax = 0;

    [SerializeField]
    private float _alphaMin = 0;

    [SerializeField]
    private float _duration = 0;
    #endregion

    #region Internal Fields    
    private Tween _tween = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        PlayTween();
    }
    #endregion

    #region APIs
    public void SetAlphaRange(float max, float min) {
        _alphaMax = max;
        _alphaMin = min;
    }

    public void SetDuration(float duration) {
        _duration = duration;
    }

    public void RestartTween() {
        PlayTween();
    }
    #endregion

    #region Internal Methods
    private void PlayTween() {
        if (_tween != null && _tween.IsActive() && _tween.IsPlaying()) {
            _tween.Kill();
        }

        float startValue = _alphaMax;
        float endValue = _alphaMin;
        if (_duration > 0) {
            _tween = DOTween.To(
                () => startValue,
                (v) => { _cg.alpha = v; startValue = v; },
                endValue, _duration).SetUpdate(true).SetLoops(-1, LoopType.Yoyo);
        }
        else {
            _tween = DOTween.To(
               () => startValue,
               (v) => { _cg.alpha = v; startValue = v; },
               endValue, _duration).SetUpdate(true);
        }
    }
    #endregion
}
