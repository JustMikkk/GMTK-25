using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Levels")]
    public LevelManager currentLevel;
    [SerializeField] private List<GameObject> _levels;
    private int _currentLevelIndex = 0;


    [Header("Other")]
    [SerializeField] private CinemachineCamera _transitionCamera;
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private TextMeshProUGUI _cubeIndexText;

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

    public void SpawnLevel(int index) {
        spawnLevel(index, false);
    }


    public void ResetLevel() {
        if (_isReseting) return;
        _isReseting = true;
        StopAllCoroutines();
        DOTween.KillAll();
        spawnLevel(_currentLevelIndex, true);
    }


    private void spawnLevel(int index, bool isReset) {
        _currentLevelIndex = index;
        
        float delay = 0.3f;
        if (currentLevel != null) {
            despawnLevel(isReset);
            delay = 1f;
        }
        
        currentLevel = Instantiate(_levels[index]).GetComponent<LevelManager>();

        if (isReset) currentLevel.AppearReset(delay);
        else currentLevel.AppearNormal(delay);

        EventBus.levelStartedEvent?.Invoke();
        UpdateMovesText(0);
        UpdateCubeText(1);
        
        currentLevel.levelReadyEvent.AddListener(onLevelReady);
    }


    private void despawnLevel(bool isReset) {
        SetTransitionCamera(true);
        currentLevel.levelReadyEvent.RemoveListener(onLevelReady);

        if (isReset) currentLevel.DisappearReset(); 
        else currentLevel.DisappearNormal();
    }


    private void onLevelReady() {
        SetTransitionCamera(false);
        _isReseting = false;
        currentLevel.SwitchCamera(false);
    }

    public void SetTransitionCamera(bool setCamera) {
        if (setCamera && _transitionCamera.Priority == 100) return;
        _transitionCamera.Priority = setCamera ? 100 : 10;
    }

#endregion

    public void UpdateMovesText(int count) {
        _movesText.text = "Moves: " + count;
    }


    public void UpdateCubeText(int index) {
        _cubeIndexText.text = "Cube #" + index;
    }

}
