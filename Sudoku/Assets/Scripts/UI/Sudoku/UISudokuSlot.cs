using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UISudokuSlot : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    private Button _btn = null;

    [SerializeField]
    private TextMeshProUGUI _textValue = null;

    [SerializeField]
    private Image _imageBG = null;

    [SerializeField]
    private int _slotIndex = 0;
    #endregion

    #region Internal Fields
    private Action<UISudokuSlot> _btnAction;
    private int _value;
    private FillReason _reason = FillReason.None;
    private Tween _tweenHighlight;
    #endregion

    #region Mono Behaviurs Hooks
    private void Awake() {
        _btn.onClick.AddListener(ButtonOnClick);
    }
    #endregion

    #region Button Handlings
    private void ButtonOnClick() {
        if (_btnAction == null) {
            return;
        }

        _btnAction(this);
    }
    #endregion

    #region Properties
    public int SlotIndex {
        get {
            return _slotIndex;
        }
    }

    public int Value {
        get {
            return _value;
        }
    }

    public FillReason FillReason {
        get {
            return _reason;
        }
    }
    #endregion

    #region APIs
    public void SetButtonOnClick(Action<UISudokuSlot> cb) {
        _btnAction = cb;
    }

    public void SetValueAndReason(int value, FillReason reason) {
        _value = value;
        _reason = reason;
        RefreshDisplay();
    }

    public void ClearValue() {
        _value = 0;
        RefreshDisplay();
    }

    public void RefreshDisplay() {
        _textValue.color = SudokuUtility.GetFillReasonColor(_reason);
        _textValue.text = _value == 0 ? string.Empty : string.Format("{0}", _value);
    }

    public void PlayTweenHighlight() {
        if (_tweenHighlight != null && _tweenHighlight.IsActive()) {
            _tweenHighlight.Kill();
        }

        float progress = 0;
        _tweenHighlight = DOTween.To(
            () => progress,
            (v) => {
                progress = v;
                _textValue.transform.localScale = Vector3.one * (1 + Mathf.Sin(Mathf.PI * progress) * 0.5f);
            },
            1,
            0.3f
            ).SetEase(Ease.Linear).SetUpdate(true);
    }
    #endregion
}
