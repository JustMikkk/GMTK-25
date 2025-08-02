using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimatedBtn : MonoBehaviour
{

    [SerializeField] private float _initialY;

    private RectTransform _rectTransform;
    private RawImage _rawImage;


    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _rawImage = GetComponent<RawImage>();
    }


    public void OnPointerEnter() {
        _rectTransform.DOKill();
        _rectTransform.DOScale(Vector3.one * 1.1f, 0.3f);
    }

    public void OnPointerExit() {
        _rectTransform.DOKill();
        _rectTransform.DOScale(Vector3.one, 0.3f);
    }
}
