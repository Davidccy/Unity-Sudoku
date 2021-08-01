using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class UIWindowCongratulations : UIGenericWindow {
    #region Serialized Fields
    [SerializeField]
    private AudioClip _ac = null;

    [SerializeField]
    private PlayableDirector _pd = null;
    #endregion

    #region Exposed Fields
    public override string WindowName => SystemDefine.UI_WINDOW_NAME_CONGRATULATIONS;
    #endregion

    #region Internal Fields
    private bool _initializing = false;
    #endregion

    #region Override Methods
    protected override void OnWindowAwake() {
    }

    protected override void OnWindowDestroy() {
    }
    #endregion

    #region APIs
    public async void PlayAnimation() {
        if (_ac != null) {
            AudioManager.Instance.PlaySE(_ac);
        }
        
        _pd.Play();

        while (_pd.state == PlayState.Playing) {
            await Task.Delay(1);
        }

        Show(false, false).DoNotAwait();
    }
    #endregion
}
