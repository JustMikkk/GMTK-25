using UnityEngine;

public class SwitchCubeBtn : MonoBehaviour
{
    [SerializeField] private bool goToNext = true;

    public void OnClick() {
        
        if (GameManager.instance.currentLevel != null)
            GameManager.instance.currentLevel.SwitchCube(goToNext);
    }
}
