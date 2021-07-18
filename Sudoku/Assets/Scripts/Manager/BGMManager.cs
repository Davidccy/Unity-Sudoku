using System.Threading.Tasks;
using UnityEngine;

public class BGMManager : ISingleton<BGMManager> {
    #region Internal Fields
    private int _bgmIndex = 0;
    #endregion

    #region Properties
    public int CurrentBGMIndex {
        get {
            return _bgmIndex;
        }
    }
    #endregion

    #region Override Methods
    protected override void Init() {
        _bgmIndex = PlayerPrefs.HasKey(SystemDefine.KEY_BGM_INDEX) ? 
            PlayerPrefs.GetInt(SystemDefine.KEY_BGM_INDEX) : 1;
    }
    #endregion

    #region APIs
    public async Task PlayBGM(int bgmIndex) {
        AudioClip bgm = await LoadBGM(bgmIndex);
        if (bgm == null) {
            return;
        }

        AudioManager.Instance.PlayBGM(bgm, true).DoNotAwait();
    }

    public async Task<AudioClip> LoadBGM(int bgmIndex) {
        string bgmName = GetBGMName(bgmIndex);
        string path = string.Format("Audio/{0}", bgmName);
        ResourceRequest request = Resources.LoadAsync<AudioClip>(path);

        while (!request.isDone) {
            await Task.Delay(1);
        }

        AudioClip ac = request.asset as AudioClip;

        return ac;
    }

    public void SaveBGM(int bgmIndex) {
        _bgmIndex = bgmIndex;
        PlayerPrefs.SetInt(SystemDefine.KEY_BGM_INDEX, _bgmIndex);
    }
    #endregion

    #region Internal Methods
    private string GetBGMName(int bgmIndex) {
        string name = string.Empty;

        switch (bgmIndex) {
            case 1:
                name = SystemDefine.AUDIO_BGM1_NAME;
                break;
            case 2:
                name = SystemDefine.AUDIO_BGM2_NAME;
                break;
            case 3:
                name = SystemDefine.AUDIO_BGM3_NAME;
                break;
            default:
                name = SystemDefine.AUDIO_BGM1_NAME;
                break;
        }

        return name;
    }
    #endregion
}
