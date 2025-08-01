using System.Collections.Generic;
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



    private void Awake()  {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }


    public void SpawnLevel(int index) {
        spawnLevel(index, false);
    }


    public void ResetLevel() {
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
        currentLevel.SwitchCamera(false);
    }

    public void SetTransitionCamera(bool setCamera) {
        if (setCamera && _transitionCamera.Priority == 100) return;
        _transitionCamera.Priority = setCamera ? 100 : 10;
    }

}
