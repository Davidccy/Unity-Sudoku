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
    #endregion

    #region Exposed Fields
    public override string WindowName => SystemDefine.UI_WINDOW_NAME_OPTION;
    #endregion

    #region Override Methods
    protected override void OnWindowAwake() {
        _sliderBGM.onValueChanged.AddListener(SliderBGMOnValueChanged);
        _sliderSE.onValueChanged.AddListener(SliderSEOnValueChanged);

        for (int i = 0; i < _btnBGMs.Length; i++) {
            int bgmIndex = i + 1;
            _btnBGMs[i].onClick.AddListener(() => ButtonBGMOnClick(bgmIndex));
        }
    }

    protected override void OnWindowEnable() {
        _sliderBGM.value = AudioManager.Instance.VolumeBGM;
        _sliderSE.value = AudioManager.Instance.VolumeSE;

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

    #region UI Slider Ui Button Handlings
    private void SliderBGMOnValueChanged(float value) {
        AudioManager.Instance.SetVolumeBGM(value);
    }

    private void SliderSEOnValueChanged(float value) {
        AudioManager.Instance.SetVolumeSE(value);
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
            int bgmIndex = i + 1;
            _btnBGMs[i].interactable = bgmIndex != curBGMIndex;
        }
    }
    #endregion
}
