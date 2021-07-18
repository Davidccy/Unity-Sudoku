using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class IUIMainPageBase : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    protected UIMainPageController _controller = null;

    [SerializeField]
    protected GameObject _goMask = null;

    [SerializeField]
    protected PlayableDirector _pdPageOpen = null;

    [SerializeField]
    protected PlayableDirector _pdPageClose = null;
    #endregion

    #region APIs
    public virtual async Task Open() {
        this.gameObject.SetActive(true);
        _goMask.SetActive(true);
        if (_pdPageOpen != null) {
            _pdPageOpen.Play();
            await Task.Delay((int) (_pdPageOpen.duration * 1000));
        }
        _goMask.SetActive(false);

        Opened();
    }

    public virtual async Task Close() {
        _goMask.SetActive(true);
        if (_pdPageClose != null) {
            _pdPageClose.Play();
            await Task.Delay((int) (_pdPageClose.duration * 1000));
        }        

        this.gameObject.SetActive(false);

        Closed();
    }

    public void ShowMask(bool show) {
        _goMask.SetActive(show);
    }
    #endregion

    #region Virtual Methods
    protected virtual void Opened() { 
    }

    protected virtual void Closed() {
    }
    #endregion
}
