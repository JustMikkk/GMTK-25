using TMPro;
using UnityEngine;

public class ZoomBtn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private bool _isZooming = false;

    private void Start() {
        EventBus.levelStartedEvent.AddListener(() => {
            _isZooming = false;
            _text.text = "Play";
        });
    }


    public void OnClick() {
        _isZooming = !_isZooming;
        _text.text = _isZooming ? "Stop" : "Play";

        if (GameManager.instance.currentLevel != null)
            GameManager.instance.currentLevel.Zoom(_isZooming);
    }
}
