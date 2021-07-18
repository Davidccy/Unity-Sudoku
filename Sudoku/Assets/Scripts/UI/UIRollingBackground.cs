using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRollingBackground : MonoBehaviour {
    #region Serialized Fields
    [SerializeField]
    public float _speedX;
    [SerializeField]
    public float _speedY;
    [SerializeField]
    public Image _imageRes = null;
    #endregion

    #region Internal Fields
    private List<RectTransform> _rtBGList = null;

    private float _screenWidth = 0;
    private float _screenHeight = 0;

    private float _spriteWidth = 0;
    private float _spriteHeight = 0;

    private int _horCount = 0;
    private int _verCount = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Start() {
        Init();
    }
    #endregion

    #region Internal Methods
    private void Init() {
        RectTransform rt = this.transform as RectTransform;
        _screenWidth = rt.rect.width;
        _screenHeight = rt.rect.height;

        _imageRes.SetNativeSize();
        _spriteWidth = _imageRes.rectTransform.rect.width;
        _spriteHeight = _imageRes.rectTransform.rect.height;        

        // Generate images
        _horCount = (int) (_screenWidth / _spriteWidth) + 1 + 1;
        _verCount = (int) (_screenHeight / _spriteHeight) + 1 + 1;

        _rtBGList = new List<RectTransform>();
        int totalCount = _horCount * _verCount;
        for (int h = 0; h < _horCount; h++) {
            for (int v = 0; v < _verCount; v++) {
                RectTransform newRT = Instantiate<RectTransform>(_imageRes.rectTransform, this.transform);
                newRT.pivot = Vector2.zero;
                newRT.anchoredPosition = new Vector2(h * _spriteWidth, v * _spriteHeight);
                _rtBGList.Add(newRT);
            }
        }
    }

    private void Update() {
        MoveAllBG();
    }

    private void MoveAllBG() {
        Vector2 deltaV = new Vector2(_speedX, _speedY) * Time.deltaTime;
        for (int i = 0; i < _rtBGList.Count; i++)
        {
            MoveBG(_rtBGList[i], deltaV);
        }
    }

    private void MoveBG(RectTransform rt, Vector2 deltaV) {
        rt.anchoredPosition += deltaV;

        // Calculat bound with condition "pivot = Vector2.zero"
        float offsetX = 0;
        if (deltaV.x > 0 && rt.anchoredPosition.x > _screenWidth) {
            offsetX = -_spriteWidth * _horCount;
        }
        else if (deltaV.x < 0 && rt.anchoredPosition.x < -_spriteWidth) {
            offsetX = _spriteWidth * _horCount;
        }

        float offsetY = 0;
        if (deltaV.y > 0 && rt.anchoredPosition.y > _screenHeight) {
            offsetY = -_spriteHeight * _verCount;
        }
        else if (deltaV.y < 0 && rt.anchoredPosition.y < -_spriteHeight) {
            offsetY = _spriteHeight * _verCount;
        }

        if (offsetX == 0 && offsetY == 0) {
            return;
        }

        rt.anchoredPosition += new Vector2(offsetX, offsetY);
    }
    #endregion
}
