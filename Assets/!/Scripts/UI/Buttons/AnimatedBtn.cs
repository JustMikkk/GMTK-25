using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimatedBtn : MonoBehaviour
{

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

    public void OnClick() {
        _rectTransform.DOKill();
        _rawImage.color = new Color(0.5f, 0.5f, 0.5f);
        _rectTransform.DOLocalMoveY(_rectTransform.localPosition.y - 10, 0.1f).OnComplete(() => {
            _rawImage.color = Color.white;
        });
        _rectTransform.DOLocalMoveY(_rectTransform.localPosition.y + 10, 0.1f).SetDelay(0.3f);

    }
}
