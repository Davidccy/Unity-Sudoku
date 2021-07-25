using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class AudioManager : ISingleton<AudioManager> {
    #region Internal Fields
    private AudioSource _asBGM = null;
    private AudioSource _asSE = null;

    private float _volumeBGM = 1;
    private float _volumeSE = 1;

    private Tween _fadeInFadeOut = null;
    private float _fadeInFadeOutDuration = 0.3f;

    private float _bgmRepeatTime = -1;
    private float _bgmLoopStartTime = -1;

    private const float _DEFAULT_VOLUME_BGM = 0.5f;
    private const float _DEFAULT_VOLUME_SE = 0.5f;

    [Header("Audio Clip Testing")]
    [SerializeField]
    private float RepeatTime = 50;
    [SerializeField]
    private float LoopPoint = 10;
    [SerializeField]
    private float JumpToPoint = 0;
    #endregion

    #region Properties
    public bool IsBGMPlaying {
        get {
            return _asBGM != null && _asBGM.isPlaying;
        }
    }

    public bool IsSEPlaying {
        get {
            return _asSE != null && _asSE.isPlaying;
        }
    }

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

    #region Mono Behaviour Hooks
    private void Update() {
        if (_asBGM != null && _asBGM.loop) {
            if (_bgmRepeatTime != -1 && _asBGM.time >= _bgmRepeatTime) {
                _asBGM.time = _bgmLoopStartTime;
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (_asBGM != null && _asBGM.isPlaying) {
                _asBGM.time = JumpToPoint;
            }
        }
#endif
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

        if (_asSE == null) {
            _asSE = gameObject.AddComponent<AudioSource>();
            _asSE.volume = _volumeSE;
        }
    }
    #endregion

    #region APIs
    public async Task PlayBGM(AudioClip ac, bool loop = false, float repeatTime = -1, float loopStartTime = -1) {
        await BGMVolumeFadeOut(false);

        _asBGM.clip = ac;
        _asBGM.loop = loop;
        _asBGM.Play();

        _bgmRepeatTime = repeatTime;
        _bgmLoopStartTime = loopStartTime;

        await BGMVolumeFadeIn(false);
    }

    public void PlaySE(AudioClip ac) {
        _asSE.clip = ac;
        _asSE.Play();
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
        _asSE.volume = value;

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
