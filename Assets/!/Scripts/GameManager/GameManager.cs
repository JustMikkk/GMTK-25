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
        if (currentLevel != null) Destroy(currentLevel.gameObject);
        
        currentLevel = Instantiate(_levels[index]).GetComponent<LevelManager>();
        currentLevel.Appear();
    }

}
