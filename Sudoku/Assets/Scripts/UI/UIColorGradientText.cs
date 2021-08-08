using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIColorGradientText : MonoBehaviour {
    // NOTE:
    // Reference : https://www.youtube.com/watch?v=FXMqUdP3XcE
    //             https://www.youtube.com/watch?v=RgPiVOlq6-s

    #region Serialized Fields
    [SerializeField]
    private TextMeshProUGUI _text = null;

    [SerializeField]
    private float _duration = 0;

    [SerializeField]
    private bool _isSameColor = false; // Is head and tail same color ?

    [SerializeField]
    private Color[] _colorArray = null;
    #endregion

    #region Internal Fields
    private int _colorIndex = 0;
    private Color _colorHead = Color.white;
    private Color _colorTail = Color.white;
    private Tween _tweenColor = null;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        Reset();
        StartColorTween();
    }

    private void Update() {
        UpdateColor();
    }
    #endregion

    #region Internal Methods
    private void Reset() {
        _colorIndex = 0;
    }

    private void StartColorTween() {
        StopColorTween();
        float progress = 0;

        Color colorHeadStart = GetTargetColor(_colorIndex);
        Color colorHeadEnd = GetTargetColor(_colorIndex + 1);

        int colorIndexTail = _isSameColor ? _colorIndex + 1 : _colorIndex;
        Color colorTailStart = GetTargetColor(colorIndexTail);
        Color colorTailEnd = GetTargetColor(colorIndexTail + 1);

        _tweenColor = DOTween.To(
            () => progress,
            (v) => {
                progress = v;
                _colorHead = GetLerpColor(colorHeadStart, colorHeadEnd, progress);
                _colorTail = GetLerpColor(colorTailStart, colorTailEnd, progress);
            },
            1.0f,
            _duration
            ).SetEase(Ease.Linear).SetUpdate(true);
        _tweenColor.onComplete += ToNextColorIndex;

    }

    private void StopColorTween() {
        if (_tweenColor != null && _tweenColor.IsActive()) {
            _tweenColor.Kill();
        }
    }

    private Color GetTargetColor(int index) {
        if (_colorArray == null) {
            return Color.white;
        }

        index %= _colorArray.Length;
        if (_colorArray.Length <= index) {
            return Color.white;
        }

        return _colorArray[index];
    }

    private Color GetLerpColor(Color colorStart, Color colorEnd, float interpolation) {
        // NOTE:
        //
        // Start                                 End
        //  |-------------------------------------|
        //          ^
        //        interpolation (0 - 1)

        interpolation = Mathf.Clamp(interpolation, 0, 1);

        float r = colorStart.r * (1 - interpolation) + colorEnd.r * interpolation;
        float g = colorStart.g * (1 - interpolation) + colorEnd.g * interpolation;
        float b = colorStart.b * (1 - interpolation) + colorEnd.b * interpolation;
        float a = colorStart.a * (1 - interpolation) + colorEnd.a * interpolation;

        return new Color(r, g, b, a);
    }

    private void ToNextColorIndex() {
        _colorIndex += 1;
        _colorIndex %= _colorArray.Length;

        StartColorTween();
    }

    private void UpdateColor() {
        // Update gradient info
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = _colorHead;
        colorKey[0].time = 0.0f;
        colorKey[1].color = _colorTail;
        colorKey[1].time = 1.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = _colorHead.a;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = _colorTail.a;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);

        // Modify colors
        TMP_TextInfo tInfo = _text.textInfo;
        Dictionary<int, Color[]> newColors = new Dictionary<int, Color[]>(); // key = materialReferenceIndex
        int characterCount = tInfo.characterCount;
        for (int i = 0; i < characterCount; i++) {
            TMP_CharacterInfo cInfo = tInfo.characterInfo[i];

            // Clone original color data
            int meshInfoIndex = cInfo.materialReferenceIndex;
            if (!newColors.ContainsKey(meshInfoIndex)) {
                Color32[] oriColors32 = tInfo.meshInfo[cInfo.materialReferenceIndex].colors32.Clone() as Color32[];
                Color[] oriColors = new Color[oriColors32.Length];
                for (int j = 0; j < oriColors32.Length; j++) {
                    oriColors[j] = oriColors32[j];
                }
                newColors.Add(meshInfoIndex, oriColors);
            }

            // Skip handling if not visible
            if (!cInfo.isVisible) {
                continue;
            }

            // Calculation of color value
            float time = 0 + (1.0f / characterCount) * i;
            if (characterCount > 1) {
                time = 0 + (1.0f / (characterCount - 1)) * i;
            }

            Color c = gradient.Evaluate(time);

            // Over write color value
            Color[] colors = newColors[meshInfoIndex];
            for (int j = 0; j < 4; j++) {
                colors[cInfo.vertexIndex + j] = c;
            }
        }

        // Update mesh
        for (int i = 0; i < tInfo.meshInfo.Length; i++) {
            if (!newColors.ContainsKey(i)) {
                continue;
            }

            TMP_MeshInfo mInfo = tInfo.meshInfo[i];
            mInfo.mesh.colors = newColors[i];
            _text.UpdateGeometry(mInfo.mesh, i);
        }
    }
    #endregion
}
