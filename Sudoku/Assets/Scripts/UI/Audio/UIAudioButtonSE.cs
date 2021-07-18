using UnityEngine;
using UnityEngine.UI;

public class UIAudioButtonSE : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    private AudioClip _se = null;
    #endregion

    #region Internal Fields
    private Button _btn;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btn = GetComponent<Button>();
        if (_btn != null) {
            _btn.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnDestroy() {
        if (_btn != null) {
            _btn.onClick.RemoveListener(PlayClickSound);
        }
    }
    #endregion

    #region Internal Methods
    private void PlayClickSound() {
        if (_se == null) {
            return;
        }

        AudioManager.Instance.PlaySound(_se);
    }
    #endregion
}
