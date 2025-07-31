using UnityEngine;

public class SwitchCameraBtn : MonoBehaviour
{
    [SerializeField] private bool _switchToVideo = false;


    public void OnClick() {
        GameManager.instance.currentLevel.SwitchCamera(_switchToVideo);
    }
}
