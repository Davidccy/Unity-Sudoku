using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIGradientBackground : MonoBehaviour {
    // NOTE:
    // Tween between 2 colors

    #region Serialized Fields
    [SerializeField] private bool _playOnEnable = false;
    [SerializeField] private Image _imageBG = null;
    [SerializeField] private float _duration = 0;

    [SerializeField] private Color _colorFrom = Color.white;
    [SerializeField] private Color _colorTo = Color.white;

    #endregion

    #region Internal Fields
    private Tween _tween;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        if (_playOnEnable) {
            PlayColorTween();
        }
    }
    #endregion

    #region Internal Methods
    private void PlayColorTween() {
        if (_tween != null && _tween.IsActive()) {
            _tween.Kill();
        }

        float progress = 0;
        _tween = DOTween.To(
            () => progress,
            (v) => {
                progress = v;
                _imageBG.color = Color.Lerp(_colorFrom, _colorTo, progress);
            },
            1.0f, 0.5f)
            .SetUpdate(true).SetLoops(-1, LoopType.Yoyo);
    }
    #endregion
}
