using UnityEngine;

public abstract class ISingleton<T> : MonoBehaviour where T : MonoBehaviour {
    #region Internal Fields
    private static T _instance = null;
    #endregion

    #region Properties
    public static T Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject(string.Format("AutoGenerate_{0}", typeof(T).ToString()));
                _instance = go.AddComponent<T>();

                DontDestroyOnLoad(go);
            }

            return _instance;
        }        
    }
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        if (_instance == null) {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }                

        Init();
    }

    private void OnDestroy() {

        Finish();
    }
    #endregion

    #region Virtual Methods
    protected virtual void Init() { 
    }

    protected virtual void Finish() {
    }
    #endregion
}
