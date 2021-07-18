using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UIWindowManager : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    private Transform _tfWindowRoot = null;
    #endregion

    #region Internal Fields
    private List<UIGenericWindow> _loadedWindow = new List<UIGenericWindow>();
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        Instance = this;
    }

    private void OnDestroy() {
        Instance = null;
    }
    #endregion

    #region Properties
    public static UIWindowManager Instance {
        get;
        set;
    }
    #endregion

    #region APIs
    public async Task<UIGenericWindow> LoadWindow(string windowName) {
        string path = string.Format("Prefabs/Windows/{0}", windowName);
        ResourceRequest request = Resources.LoadAsync<UIGenericWindow>(path);
        while (!request.isDone) {
            await Task.Delay(1);
        }

        UIGenericWindow window = request.asset as UIGenericWindow;
        UIGenericWindow windowPrefab = null;
        if (window != null) {
            windowPrefab = Instantiate(window, _tfWindowRoot);
        }

        return windowPrefab;
    }

    public async Task OpenWindow(string windowName) {
        UIGenericWindow window = GetWindow(windowName);
        if (window == null) {
            window = await LoadWindow(windowName);
        }

        if (window != null) {
            await window.Show(true, false);
        }
    }

    public async Task CloseWindow(string windowName) {
        UIGenericWindow window = GetWindow(windowName);
        if (window != null) {
            await window.Show(false, false);
        }
    }
    #endregion

    #region Internal Methods
    private UIGenericWindow GetWindow(string windowName) {
        for (int i = 0; i < _loadedWindow.Count; i++) {
            if (_loadedWindow[i].WindowName == windowName) {
                return _loadedWindow[i];
            }
        }

        return null;
    }
    #endregion
}
