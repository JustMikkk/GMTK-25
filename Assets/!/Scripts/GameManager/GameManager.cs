using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Serializable]
    private struct LevelData {
        public int levelIndex;
        public GameObject prefab;
        public bool isUnlocked;
        public bool wasCutscenePlayed;
        public int starRating;
        public int minimumMoves;
    }

    [Header("Levels")]
    public LevelManager currentLevel;
    [SerializeField] private List<LevelData> _levels;
    private int _currentLevelIndex = 8;


    [Header("Other")]
    [SerializeField] private CinemachineCamera _transitionCamera;
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private TextMeshProUGUI _cubeIndexText;
    [SerializeField] private CanvasGroup _nextLvlBtnCanvasGroup;
    [SerializeField] private Cubeca _cubica;

    private bool _isReseting = false;


    private void Awake()  {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }


#region Level Handling

    public void SpawnNextLevel() {
        SpawnLevel(_currentLevelIndex + 1);
    }


    public void SpawnLevel(int index) {
        spawnLevel(index, false);
    }


    public void ResetLevel() {
        if (_isReseting) return;
        _isReseting = true;
        
        spawnLevel(_currentLevelIndex, true);
    }


    private void spawnLevel(int index, bool isReset) {
        // StopAllCoroutines();
        // DOTween.KillAll();

        
        float delay = 0.3f;
        if (currentLevel != null) {
            despawnLevel(isReset);
            delay = 1f;
        }
        
        currentLevel = Instantiate(_levels[index].prefab).GetComponent<LevelManager>();

        if (isReset) currentLevel.AppearReset(delay);
        else currentLevel.AppearNormal(delay);

        EventBus.levelStartedEvent?.Invoke();
        UpdateMovesText(0);
        UpdateCubeText(1);
        
        currentLevel.levelReadyEvent.AddListener(onLevelReady);

        _currentLevelIndex = index;
        
        _nextLvlBtnCanvasGroup.DOKill();
        if (isNextLvlUnlocked()) {
            _nextLvlBtnCanvasGroup.interactable = true;
            _nextLvlBtnCanvasGroup.DOFade(1, 0.3f);
        } else {
            _nextLvlBtnCanvasGroup.interactable = false;
            _nextLvlBtnCanvasGroup.DOFade(0, 0.3f);
        }
    }


    private void despawnLevel(bool isReset) {
        EventBus.destroyingLevelEvent?.Invoke();
        SetTransitionCamera(true);
        currentLevel.levelReadyEvent.RemoveListener(onLevelReady);

        if (isReset) currentLevel.DisappearReset(); 
        else currentLevel.DisappearNormal();
    }


    private void onLevelReady() {
        SetTransitionCamera(false);
        _isReseting = false;
        currentLevel.SwitchCamera(false);
        if (!_levels[_currentLevelIndex].wasCutscenePlayed) {
            ShowDialogue(_levels[_currentLevelIndex].levelIndex);
        }
    }


#endregion

    public void ShowDialogue(int index = -1) {
        if (index == -1) index = _currentLevelIndex;
        _cubica.ShowDialogue(index);
    }


    public void DialogueComplete() {
        if (_currentLevelIndex >= _levels.Count) return;

        LevelData level = _levels[_currentLevelIndex];
        level.wasCutscenePlayed = true;
        _levels[_currentLevelIndex] = level;
    }


    public void SetTransitionCamera(bool setCamera) {
        if (setCamera && _transitionCamera.Priority == 100) return;
        _transitionCamera.Priority = setCamera ? 100 : 10;
    }


    public void UpdateMovesText(int count) {
        _movesText.text = "Moves: " + count;
    }


    public void UpdateCubeText(int index) {
        _cubeIndexText.text = "Cube #" + index;
    }


    public void UnlockNextLvl(int moves) {
        if (_currentLevelIndex + 1 == _levels.Count) return;
        var nextLevel = _levels[_currentLevelIndex + 1];
        nextLevel.isUnlocked = true;
        _levels[_currentLevelIndex + 1] = nextLevel;
        Debug.Log("unlocking lvl " + nextLevel.levelIndex);

        _nextLvlBtnCanvasGroup.DOKill();
        _nextLvlBtnCanvasGroup.interactable = true;
        _nextLvlBtnCanvasGroup.DOFade(1, 0.3f);
        EventBus.levelUnlockedEvent?.Invoke(moves);
    }

    public bool IsLevelUnlocked(int index) {
        return _levels[index].isUnlocked;
    }


    private bool isNextLvlUnlocked() {
        if (_currentLevelIndex + 1 == _levels.Count) return false;
        return _levels[_currentLevelIndex + 1].isUnlocked;
    }

    public int GetMinimumMoves(int index) {
        return _levels[index].minimumMoves;
    }
}
