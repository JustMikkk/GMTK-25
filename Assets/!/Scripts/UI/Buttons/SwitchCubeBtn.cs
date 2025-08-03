using UnityEngine;

public class SwitchCubeBtn : MonoBehaviour
{
    [SerializeField] private bool goToNext = true;

    public void OnClick()
    {

        if (GameManager.instance.currentLevel != null)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClickSound, this.transform.position);
            GameManager.instance.currentLevel.SwitchCube(goToNext);
        }
    }
}
