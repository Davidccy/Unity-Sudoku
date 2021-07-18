using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public abstract class UIGenericWindow : MonoBehaviour {
    #region Serialized Fields
    [Header("Generic")]
    [SerializeField]
    private RectTransform _rectRoot = null;
    [SerializeField]
    private CanvasGroup _cgRoot = null;
	[SerializeField]
	private float _duration = 0;
	[SerializeField]
	private Button _btnClose = null;
	[SerializeField]
	private AudioClip _audioClipClose = null;
	#endregion

	#region Exposed Fields
	public abstract string WindowName {
		get;
	}
	#endregion

	#region Internal Fields
	private bool _isOpening = false;
	private Tween _tween = null;
    #endregion

    #region Properties
    public bool IsOpened {
		get {
			return _isOpening;
		}
	}

	public bool IsTweenPlaying {
		get {
			if (_tween != null && _tween.IsPlaying()) {
				return true;
			}

			return false;
		}
	}
	#endregion

	#region Mono Behaviour Hooks
	private void Awake() {
		_btnClose.onClick.AddListener(ButtonCloseOnClick);

		OnWindowAwake();
	}

    private void OnEnable() {
		OnWindowEnable();
	}

	private void OnDisable() {
		OnWindowDisable();
	}

	private void OnDestroy() {
		_btnClose.onClick.RemoveAllListeners();

		OnWindowDestroy();
	}
	#endregion

	#region Virtual Methods
	protected virtual void OnWindowAwake() { 

	}

	protected virtual void OnWindowEnable() {

	}

	protected virtual void OnWindowDisable() {

	}

	protected virtual void OnWindowDestroy() {

	}
	#endregion

	#region UI Button Handlings
	private void ButtonCloseOnClick() {
		Show(false, false).DoNotAwait();

		if (_audioClipClose != null) {
			AudioManager.Instance.PlaySound(_audioClipClose);
		}
	}
	#endregion

	#region APIs
	public async Task Show(bool show, bool skipTween, bool forcibly = false) {
		if (!forcibly && _isOpening == show) {
			return;
		}

		// Init tween
		if (_tween != null && _tween.IsActive() && _tween.IsPlaying()) {
			return;
		}

		if (show) {
			_rectRoot.gameObject.SetActive(true);
		}

		_cgRoot.blocksRaycasts = false;

		float startAlpha = show ? 0.0f : 1.0f;
		float goalAlpha = show ? 1.0f : 0.0f;
		float duration = skipTween ? 0.0f : _duration;

		_tween = DOTween.To(
			() => startAlpha,
			(v) => {
				_cgRoot.alpha = v;
				startAlpha = v;
			},
			goalAlpha, duration).SetUpdate(true);

		await _tween.AsyncWaitForCompletion();

		if (!show) {
			_rectRoot.gameObject.SetActive(false);
		}
		else {
			_cgRoot.blocksRaycasts = true;
		}

		_isOpening = show;
	}
    #endregion
}