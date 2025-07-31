using TMPro;
using UnityEngine;

public class ZoomBtn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private bool _isZooming = false;


    public void OnClick() {
        if (GameManager.instance.currentLevel == null) {
            GameManager.instance.SpawnLevel(1);      
            return; 
        }

        _isZooming = !_isZooming;
        _text.text = _isZooming ? "Stop" : "Play";
        GameManager.instance.currentLevel.Zoom(_isZooming);
    }
}
