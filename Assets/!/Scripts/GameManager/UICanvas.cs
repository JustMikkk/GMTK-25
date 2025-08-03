using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private RectTransform _leftPanel, _startPanel, _rightPanel;
    [SerializeField] private RectTransform _creditsRect;
    [SerializeField] private Cubeca _cubeca;
    [SerializeField] private Button _lvl1Btn;

    private bool _isStartPanel = false;

    [SerializeField] private InputActionAsset _inputActions;
    private InputActionMap _inputActionMap;
    private InputAction _exitAction;


    private void Awake()
    {
        _inputActionMap = _inputActions.FindActionMap("Canvas");

        _exitAction = InputSystem.actions.FindAction("Exit");
    }


    private void Start()
    {
        _cubeca.dialogueFinishedEvent.AddListener(onDialogueFinished);
    }


    private void OnEnable()
    {
        _inputActionMap.Enable();
    }


    private void Update()
    {
        if (_exitAction.WasPressedThisFrame())
        {
            _isStartPanel = !_isStartPanel;
            updateStartMenu();
        }
    }


    public void OnCreditsClick()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClickSound, this.transform.position);
        _creditsRect.localScale = new Vector3(1, 0, 1);
        _creditsRect.DOKill();
        _creditsRect.DOScaleY(1, 0.3f).SetEase(Ease.OutBack);
        _creditsRect.DOScaleY(0, 0.3f).SetDelay(8);
    }


    public void OnExitClick()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClickSound, this.transform.position);
        Application.Quit();
    }


    private void updateStartMenu()
    {
        _startPanel.DOKill();
        _startPanel.DOAnchorPosY(_isStartPanel ? 0 : 720, 0.7f).SetEase(Ease.OutSine);
    }

    private void onDialogueFinished()
    {
        if (GameManager.instance.currentLevel != null) return;
        _isStartPanel = false;
        updateStartMenu();
        _leftPanel.DOAnchorPosX(0, 0.7f).SetEase(Ease.InOutCirc);
        _rightPanel.DOAnchorPosX(0, 0.7f).SetEase(Ease.InOutCirc).OnComplete(() =>
        {
            _lvl1Btn.onClick?.Invoke();
        });
    }
}
