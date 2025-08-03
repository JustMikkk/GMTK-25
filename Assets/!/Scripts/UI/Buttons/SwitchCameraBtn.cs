using UnityEngine;

public class SwitchCameraBtn : MonoBehaviour
{
    [SerializeField] private bool _switchToVideo = false;


    public void OnClick()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClickSound, this.transform.position);
        GameManager.instance.currentLevel.SwitchCamera(_switchToVideo);
    }
}
