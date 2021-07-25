using UnityEngine;

public class UIMain : MonoBehaviour {
    public enum PageType {
        Login,
        Menu,
        Solver,
        Generator,
    }

    #region Serialized Fields
    [SerializeField]
    private int _initPageIndex = 0;

    [SerializeField]
    private UIMainPageController _pageController = null;
    #endregion

    #region Mono Bahaviour Hooks
    private void Awake() {
        // Page
        _pageController.EnterPage(_initPageIndex).DoNotAwait();

        // BGM
        int bgmIndex = BGMManager.Instance.CurrentBGMIndex;
        BGMManager.Instance.PlayBGM(bgmIndex).DoNotAwait();

#if UNITY_EDITOR
        Application.runInBackground = true;
#endif
    }
    #endregion
}
;