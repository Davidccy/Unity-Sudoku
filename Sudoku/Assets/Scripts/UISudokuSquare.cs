using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISudokuSquare : MonoBehaviour {
    public enum ColorType { 
        White,
        Gray,
    }

    #region Serialized Fields
    [SerializeField]
    private ColorType _colorType = 0;

    [SerializeField]
    private Image _imageBG = null;
    #endregion

    #region Mono Behaviurs Hooks
    private void Awake() {
        _imageBG.color = _colorType == ColorType.White ? Color.white : Color.gray;
    }
    #endregion

#if UNITY_EDITOR
    #region Testing
    [ContextMenu("TestImportBGColor")]
    public void TestImportBGColor() {
        _imageBG.color = _colorType == ColorType.White ? Color.white : Color.gray;
    }
    #endregion
#endif
}
