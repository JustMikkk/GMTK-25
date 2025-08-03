using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference MusicPlayer { get; private set; }

    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference UIClickSound { get; private set; }
    [field: SerializeField] public EventReference ChangeLevel { get; private set; }
    [field: SerializeField] public EventReference TalkingSounds { get; private set; }
    [field: SerializeField] public EventReference CameraChangeSound { get; private set; }
    [field: SerializeField] public EventReference CubeLandSound { get; private set; }
    [field: SerializeField] public EventReference CubeMoveSound { get; private set; }
    [field: SerializeField] public EventReference OpenLevelSound { get; private set; }


    public static FMODEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one FMODEvents in the scene");
        }
        Instance = this;
    }
}
