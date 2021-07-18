using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class AudioManager : ISingleton<AudioManager> {
    #region Internal Fields
    private AudioSource _asBGM = null;
    private AudioSource _asSound = null;

    private float _volumeBGM = 1;
    private float _volumeSE = 1;

    private Tween _fadeInFadeOut = null;
    private float _fadeInFadeOutDuration = 0.3f;

    private const float _DEFAULT_VOLUME_BGM = 0.5f;
    private const float _DEFAULT_VOLUME_SE = 0.5f;
    #endregion

    #region Properties
    public float VolumeBGM {
        get {
            return _volumeBGM;
        }
    }

    public float VolumeSE {
        get {
            return _volumeSE;
        }
    }
    #endregion

    #region Override Methods
    protected override void Init() {
        _volumeBGM = PlayerPrefs.HasKey(SystemDefine.KEY_VOLUME_BGM) ? 
            PlayerPrefs.GetFloat(SystemDefine.KEY_VOLUME_BGM) : _DEFAULT_VOLUME_BGM;

        _volumeSE = PlayerPrefs.HasKey(SystemDefine.KEY_VOLUME_SE) ? 
            PlayerPrefs.GetFloat(SystemDefine.KEY_VOLUME_SE) : _DEFAULT_VOLUME_SE;

        if (_asBGM == null) {
            _asBGM = gameObject.AddComponent<AudioSource>();
            _asBGM.volume = _volumeBGM;
        }

        if (_asSound == null) {
            _asSound = gameObject.AddComponent<AudioSource>();
            _asSound.volume = _volumeSE;
        }
    }
    #endregion

    #region APIs
    public async Task PlayBGM(AudioClip ac, bool loop = false) {
        //bool skipTween = _asBGM.clip == null;
        await BGMVolumeFadeOut(false);

        _asBGM.clip = ac;
        _asBGM.loop = loop;
        _asBGM.Play();

        await BGMVolumeFadeIn(false);
    }

    public void PlaySound(AudioClip ac) {
        _asSound.clip = ac;
        _asSound.Play();
    }

    public void SetVolumeBGM(float value) {
        value = Mathf.Clamp(value, 0, 1);

        _volumeBGM = value;
        _asBGM.volume = value;

        PlayerPrefs.SetFloat(SystemDefine.KEY_VOLUME_BGM, value);
    }

    public void SetVolumeSE(float value) {
        value = Mathf.Clamp(value, 0, 1);

        _volumeSE = value;
        _asSound.volume = value;

        PlayerPrefs.SetFloat(SystemDefine.KEY_VOLUME_SE, value);
    }

    public string GetPlayingBGMName() {
        return (_asBGM != null && _asBGM.clip != null) ? _asBGM.clip.name : string.Empty;
    }
    #endregion

    #region Internal Methods    
    private async Task BGMVolumeFadeIn(bool skipTween) {
        if (_fadeInFadeOut != null && _fadeInFadeOut.IsActive()) {
            _fadeInFadeOut.Kill();
        }

        float duration = skipTween ? 0 : _fadeInFadeOutDuration;
        _fadeInFadeOut = DOTween.To(
            () => _asBGM.volume,
            (v) => _asBGM.volume = v,
            _volumeBGM,
            duration
            ).SetUpdate(true);

        while (_fadeInFadeOut != null && _fadeInFadeOut.IsActive() && _fadeInFadeOut.IsPlaying()) {
            await Task.Delay(1);
        }
    }

    private async Task BGMVolumeFadeOut(bool skipTween) {
        if (_fadeInFadeOut != null && _fadeInFadeOut.IsActive()) {
            _fadeInFadeOut.Kill();
        }

        float duration = skipTween ? 0 : _fadeInFadeOutDuration;
        _fadeInFadeOut = DOTween.To(
            () => _asBGM.volume,
            (v) => _asBGM.volume = v,
            0,
            duration
            ).SetUpdate(true);

        while (_fadeInFadeOut != null && _fadeInFadeOut.IsActive() && _fadeInFadeOut.IsPlaying()) {
            await Task.Delay(1);
        }
    }
    #endregion
}
