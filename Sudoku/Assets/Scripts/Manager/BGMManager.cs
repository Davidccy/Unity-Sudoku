using System.Threading.Tasks;
using UnityEngine;

public class BGMManager : ISingleton<BGMManager> {
    #region Internal Fields
    private int _bgmIndex = 0;
    private GameConfig _gameConfig = null;
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
        BGMData data = await GetBGMData(bgmIndex);
        if (data == null) {
            return;
        }

        AudioManager.Instance.PlayBGM(data.BGM, true, data.RepeatTime, data.LoopStartTime).DoNotAwait();
    }

    public void SaveBGM(int bgmIndex) {
        _bgmIndex = bgmIndex;
        PlayerPrefs.SetInt(SystemDefine.KEY_BGM_INDEX, _bgmIndex);
    }
    #endregion

    #region Internal Methods
    private async Task<BGMData> GetBGMData(int bgmIndex) {
        if (_gameConfig == null) {
            _gameConfig = await SystemUtility.GetGameConfig();
        }

        if (_gameConfig == null || _gameConfig.BackGroundMusics.Length <= bgmIndex) {
            return null;
        }

        return _gameConfig.BackGroundMusics[bgmIndex];
    }
    #endregion
}
