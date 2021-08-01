using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowMessage : UIGenericWindow {
    public class MessageCmd {
        public MessageType Type = MessageType.Confirm;
        public string Title;
        public Action ActionConfirm;
        public Action ActionYes;
        public Action ActionNo;
    }

    public enum MessageType { 
        Confirm,
        YesNo,
    }

    #region Serialized Fields
    [SerializeField]
    private TextMeshProUGUI _textTitle = null;

    [SerializeField]
    private GameObject _goRootConfirm = null;

    [SerializeField]
    private Button _btnConfirm = null;

    [SerializeField]
    private GameObject _goRootYesNo = null;

    [SerializeField]
    private Button _btnYes = null;

    [SerializeField]
    private Button _btnNo = null;
    #endregion

    #region Exposed Fields
    public override string WindowName => SystemDefine.UI_WINDOW_NAME_MESSAGE;
    #endregion

    #region Internal Fields
    private MessageType _type = MessageType.Confirm;
    private Action _onClickActionConfirm = null;
    private Action _onClickActionYes = null;
    private Action _onClickActionNo = null;
    #endregion

    #region Override Methods
    protected override void OnWindowAwake() {
        _btnConfirm.onClick.AddListener(ButtonConfirmOnClick);
        _btnYes.onClick.AddListener(ButtonYesOnClick);
        _btnNo.onClick.AddListener(ButtonNoOnClick);
    }

    protected override void OnWindowDestroy() {
        _btnConfirm.onClick.RemoveAllListeners();
        _btnYes.onClick.RemoveAllListeners();
        _btnNo.onClick.RemoveAllListeners();
    }
    #endregion

    #region UI Button Handlings
    private void ButtonConfirmOnClick() {
        if (_onClickActionConfirm != null) {
            _onClickActionConfirm();
        }
    }

    private void ButtonYesOnClick() {
        if (_onClickActionYes != null) {
            _onClickActionYes();
        }
    }

    private void ButtonNoOnClick() {
        if (_onClickActionNo != null) {
            _onClickActionNo();
        }
    }
    #endregion

    #region APIs
    public void SetInfo(MessageCmd cmd) {
        _type = cmd.Type;
        _onClickActionConfirm = cmd.ActionConfirm;
        _onClickActionYes = cmd.ActionYes;
        _onClickActionNo = cmd.ActionNo;

        _textTitle.text = cmd.Title;

        Refresh();
    }
    #endregion

    #region Internla Methods
    private void Refresh() {
        _goRootConfirm.SetActive(_type == MessageType.Confirm);
        _goRootYesNo.SetActive(_type == MessageType.YesNo);
    }
    #endregion
}
