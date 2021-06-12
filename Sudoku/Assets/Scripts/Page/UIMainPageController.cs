using System.Collections.Generic;
using UnityEngine;

public class UIMainPageController : MonoBehaviour {
    public enum PageType { 
        Login,
        Menu,
        Solver,
        Generator,
    }

    #region Serialized Fields
    [SerializeField]
    private List<IUIMainPageBase> _uiPages = null;

    [SerializeField]
    private int _initPageIndex = 0;
    #endregion

    #region Internal Fields
    private int _curPageIndex = -1;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        Init();
        EnterPage(_initPageIndex);
    }
    #endregion

    #region APIs
    public async void EnterPage(int pageIndex) {
        if (_curPageIndex != -1) {
            await _uiPages[_curPageIndex].Close();
        }

        await _uiPages[pageIndex].Open();

        _curPageIndex = pageIndex;
    }
    #endregion

    #region Internal Methods
    private void Init() {
        for (int i = 0; i < _uiPages.Count; i++) {
            _uiPages[i].gameObject.SetActive(false);
        }
    }
    #endregion
}
