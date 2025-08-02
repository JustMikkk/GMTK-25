using UnityEngine;

public class SwitchCubeBtn : MonoBehaviour
{
    [SerializeField] private bool goToNext = true;

    public void OnClick() {
        GameManager.instance.currentLevel.SwitchCube(goToNext);
    }
}
