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
    public async Task<UIGenericWindow> GetWindow(string windowName, bool loadIfNotExist = true) {
        for (int i = 0; i < _loadedWindow.Count; i++) {
            if (_loadedWindow[i].WindowName == windowName) {
                return _loadedWindow[i];
            }
        }

        UIGenericWindow window = await LoadWindow(windowName);

        return window;
    }

    public async Task<UIGenericWindow> OpenWindow(string windowName, bool loadIfNotExist = true) {
        UIGenericWindow window = await GetWindow(windowName, loadIfNotExist);
        if (window != null) {
            await window.Show(true, false);
        }

        return window;
    }

    public async Task<UIGenericWindow> CloseWindow(string windowName, bool loadIfNotExist = false) {
        UIGenericWindow window = await GetWindow(windowName, loadIfNotExist);
        if (window != null) {
            await window.Show(false, false);
        }

        return window;
    }
    #endregion

    #region Internal Methods
    private void AddWindow(UIGenericWindow window) {
        if (window == null) {
            return;
        }

        for (int i = 0; i < _loadedWindow.Count; i++) {
            if (_loadedWindow[i].WindowName == window.name) {
                break;
            }
        }

        _loadedWindow.Add(window);
    }

    private async Task<UIGenericWindow> LoadWindow(string windowName) {
        string path = string.Format("Prefabs/Windows/{0}", windowName);
        ResourceRequest request = Resources.LoadAsync<UIGenericWindow>(path);
        while (!request.isDone) {
            await Task.Delay(1);
        }

        UIGenericWindow window = null;
        UIGenericWindow windowAsset = request.asset as UIGenericWindow;
        if (windowAsset != null) {
            window = Instantiate(windowAsset, _tfWindowRoot);

            AddWindow(window);
        }

        return window;
    }
    #endregion
}
