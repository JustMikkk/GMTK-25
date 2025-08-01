using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public LevelManager currentLevel;
    [SerializeField] private List<GameObject> _levels;



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
            currentLevel.Disappear();
            delay = 1f;
        }
        
        currentLevel = Instantiate(_levels[index]).GetComponent<LevelManager>();
        currentLevel.Appear(delay);
    }


    public void DespawnLevel() {
        currentLevel.Disappear();
    }

}
