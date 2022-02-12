using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

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

    private void Test() {
        var tempClassList = new List<(int, string)>();

        // Add and remove of list
        tempClassList.Add((1, "1"));
        tempClassList.Remove((1, "2"));

        // Get and set
        var element1 = tempClassList[0];
        element1.Item1 = 0;
        element1.Item2 = string.Empty;

        (int, string) element2 = tempClassList[0];
        element2.Item1 = 0;
        element2.Item2 = string.Empty;
    }
}
