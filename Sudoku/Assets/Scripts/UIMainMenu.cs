using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour {
    public enum PageType { 
        MainMenu,
        Solver,
    }

    #region Serialized Fields
    [SerializeField]
    private Button _btnSolver = null;
    [SerializeField]
    private Button _btnGenerater = null;
    [SerializeField]
    private GameObject _goMenu = null;

    [Header("Scene Switching")]
    [SerializeField]
    private GameObject _goMainMenu = null;
    [SerializeField]
    private GameObject _goSolver = null;
    #endregion

    #region Mono Behaviours
    private void Awake() {
        _btnSolver.onClick.AddListener(ButtonSolverOnClick);
        _btnGenerater.onClick.AddListener(ButtonGeneraterOnClick);
    }

    private void Start() {
        EnterPage(PageType.MainMenu);
    }

    private void OnDestroy() {
        _btnSolver.onClick.RemoveAllListeners();
        _btnGenerater.onClick.RemoveAllListeners();
    }
    #endregion

    #region Button Handlings
    private void ButtonSolverOnClick() { 
    }

    private void ButtonGeneraterOnClick() {
       
    }
    #endregion

    #region Internal Methods
    private void EnterPage(PageType pt) {
        _goMainMenu.SetActive(pt == PageType.MainMenu);
        _goSolver.SetActive(pt == PageType.Solver);
    }

    private void EnterSolverMode() { 
    }

    private void EnterGeneraterMode() { 

    }
    #endregion
}
