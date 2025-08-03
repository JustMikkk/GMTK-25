using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelBtn : MonoBehaviour
{
    [SerializeField] private int _index;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _star;
    
    private RawImage _rawImg;
    private RectTransform _rectTransform;
    private Button _button;
    private EventTrigger _eventTrigger;


    private void Awake() {
        _button = GetComponent<Button>();
        _rectTransform = GetComponent<RectTransform>();
        _rawImg = GetComponent<RawImage>();
        _eventTrigger = GetComponent<EventTrigger>();
    }

    private void Start() {
        updateUI(99);
        EventBus.levelUnlockedEvent.AddListener(updateUI);
    }


    private void updateUI(int moves) {
        if (GameManager.instance.IsLevelUnlocked(_index)) {
            _rawImg.color = Color.white;
            _text.color = Color.white;
            _rawImg.raycastTarget = true;
            _button.enabled = true;
            _eventTrigger.enabled = true;
            _rectTransform.localScale = Vector3.one;

            if (moves < GameManager.instance.GetMinimumMoves(_index)) {
                _star.SetActive(true);
            }

        } else {
            _rawImg.color = Color.gray;
            _text.color = Color.gray;
            _rawImg.raycastTarget = false;
            _button.enabled = false;
            _eventTrigger.enabled = false;
            _rectTransform.localScale = Vector3.one * 0.8f;
        }
    }

    
}
