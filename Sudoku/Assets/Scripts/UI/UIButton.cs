using UnityEngine;
using TMPro;

public class UIButton : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] 
    private TextMeshProUGUI _text = null;
    [SerializeField] 
    private string _initText = string.Empty;
    #endregion

    #region Internal Fields
    private bool _init = false;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        Init();
    }
    #endregion

    #region Internal Methods
    private void Init() {
        if (_init) {
            return;
        }

        _init = true;
        _text.text = _initText;
    }

    [ContextMenu("Refresh")]
    private void Refresh() {
        _text.text = _initText;
    }
    #endregion
}

