using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIMainPageController : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    private List<IUIMainPageBase> _uiPages = null;
    #endregion

    #region Internal Fields
    private int _curPageIndex = -1;
    private bool _inited = false;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        Init();
    }
    #endregion

    #region APIs
    public async Task EnterPage(int pageIndex) {
        if (!_inited) {
            Init();
        }

        if (_curPageIndex != -1) {
            await _uiPages[_curPageIndex].Close();
        }

        await _uiPages[pageIndex].Open();

        _curPageIndex = pageIndex;
    }
    #endregion

    #region Internal Methods
    private void Init() {
        if (_inited) {
            return;
        }

        for (int i = 0; i < _uiPages.Count; i++) {
            _uiPages[i].gameObject.SetActive(false);
        }

        _inited = true;
    }
    #endregion
}
