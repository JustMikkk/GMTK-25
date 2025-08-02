using DG.Tweening;
using TMPro;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private RectTransform _slider;
    [SerializeField] private TextMeshProUGUI _text;

    private bool isVideoMode {
        get { return _isVideoMode; }
        set {
            if (_isVideoMode == value) return;
            _isVideoMode = value;
            animateSwitch();
        }
    }
    private bool _isVideoMode = false;


    private void Start() {
        EventBus.levelStartedEvent.AddListener(() => {
            isVideoMode = false;
        });
    }


    public void OnClick() {
        isVideoMode = !isVideoMode;
        if (GameManager.instance.currentLevel != null)
            GameManager.instance.currentLevel.SwitchCamera(isVideoMode);
    }


    private void animateSwitch() {
        _slider.DOKill();
        _slider.DOLocalMoveX(_isVideoMode ? 50 : -50, 0.3f).SetEase(Ease.InOutCirc);
        _text.rectTransform.DOKill();
        _text.rectTransform.DOLocalRotate(new Vector3(0, 90, 0), 0.15f).OnComplete(() => {
            _text.rectTransform.rotation = Quaternion.Euler(0, -90, 0);
            _text.text = _isVideoMode ? "Video" : "Edit";
            _text.rectTransform.DOLocalRotate(Vector3.zero, 0.15f);
        });
    }
}
