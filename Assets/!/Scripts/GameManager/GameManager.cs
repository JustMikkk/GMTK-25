using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Levels")]
    public LevelManager currentLevel;
    [SerializeField] private List<GameObject> _levels;

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
        float delay = 0.3f;
        if (currentLevel != null) {
            DespawnLevel();
            delay = 1f;
        }
        
        currentLevel = Instantiate(_levels[index]).GetComponent<LevelManager>();
        currentLevel.Appear(delay);
        currentLevel.levelReadyEvent.AddListener(onLevelReady);
    }


    public void DespawnLevel() {
        _transitionCamera.Priority = 100;
        currentLevel.levelReadyEvent.RemoveListener(onLevelReady);
        currentLevel.Disappear();
    }


    private void onLevelReady() {
        _transitionCamera.Priority = 10;
        currentLevel.SwitchCamera(false);
    }

}
