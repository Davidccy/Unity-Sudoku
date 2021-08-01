using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowOption : UIGenericWindow {
    #region Serialized Fields
    [SerializeField]
    private Slider _sliderBGM = null;

    [SerializeField]
    private Slider _sliderSE = null;

    [SerializeField]
    private Button[] _btnBGMs = null;

    [SerializeField]
    private AudioClip _sliderSoundEffect = null;
    #endregion

    #region Exposed Fields
    public override string WindowName => SystemDefine.UI_WINDOW_NAME_OPTION;
    #endregion

    #region Internal Fields
    private bool _initializing = false;
    #endregion

    #region Override Methods
    protected override void OnWindowAwake() {
        _sliderBGM.onValueChanged.AddListener(SliderBGMOnValueChanged);
        _sliderSE.onValueChanged.AddListener(SliderSEOnValueChanged);

        for (int i = 0; i < _btnBGMs.Length; i++) {
            int bgmIndex = i;
            _btnBGMs[i].onClick.AddListener(() => ButtonBGMOnClick(bgmIndex));
        }
    }

    protected override void OnWindowEnable() {
        _initializing = true;

        _sliderBGM.value = AudioManager.Instance.VolumeBGM;
        _sliderSE.value = AudioManager.Instance.VolumeSE;

        _initializing = false;

        Refresh();
    }

    protected override void OnWindowDestroy() {
        _sliderBGM.onValueChanged.RemoveAllListeners();
        _sliderSE.onValueChanged.RemoveAllListeners();

        for (int i = 0; i < _btnBGMs.Length; i++) {
            _btnBGMs[i].onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region UI Slider UI Button Handlings
    private void SliderBGMOnValueChanged(float value) {
        AudioManager.Instance.SetVolumeBGM(value);
        PlaySliderSoundEffect();
    }

    private void SliderSEOnValueChanged(float value) {
        AudioManager.Instance.SetVolumeSE(value);
        PlaySliderSoundEffect();
    }

    private void ButtonBGMOnClick(int bgmIndex) {
        if (BGMManager.Instance.CurrentBGMIndex == bgmIndex) {
            return;
        }

        BGMManager.Instance.PlayBGM(bgmIndex).DoNotAwait();
        BGMManager.Instance.SaveBGM(bgmIndex);
        Refresh();
    }
    #endregion

    #region Internla Methods
    private void Refresh() {
        RefreshBGMButton();
    }

    private void RefreshBGMButton() {
        int curBGMIndex = BGMManager.Instance.CurrentBGMIndex;
        for (int i = 0; i < _btnBGMs.Length; i++) {
            int bgmIndex = i;
            _btnBGMs[i].interactable = bgmIndex != curBGMIndex;
        }
    }

    private void PlaySliderSoundEffect() {
        if (_initializing) {
            return;
        }

        if (_sliderSoundEffect == null) {
            return;
        }

        //if (AudioManager.Instance.IsSEPlaying) {
        //    return;
        //}

        AudioManager.Instance.PlaySE(_sliderSoundEffect);
    }
    #endregion
}
