using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class IUIMainPageBase : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    protected UIMainPageController _controller = null;

    [SerializeField]
    protected PlayableDirector _pdPageOpen = null;

    [SerializeField]
    protected PlayableDirector _pdPageClose = null;
    #endregion

    #region APIs
    public virtual async Task Open() {
        this.gameObject.SetActive(true);
        if (_pdPageOpen != null) {
            _pdPageOpen.Play();
            await Task.Delay((int) (_pdPageOpen.duration * 1000));
        }

        Opened();
    }

    public virtual async Task Close() {
        if (_pdPageClose != null) {
            _pdPageClose.Play();
            await Task.Delay((int) (_pdPageClose.duration * 1000));
        }        

        this.gameObject.SetActive(false);

        Closed();
    }

    protected virtual void Opened() { 
    }

    protected virtual void Closed() {
    }
    #endregion
}
