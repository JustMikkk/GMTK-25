using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public bool DebugMode = false;

    private List<EventInstance> eventInstances;

    private EventInstance ambientInstance;

    private EventInstance musicInstance;

    private EventInstance attackInstance;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // make persistent across scenes
        eventInstances = new List<EventInstance>();
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.Instance.MusicPlayer);
    }

    private void InitializeMusic(EventReference eventReference)
    {
        musicInstance = RuntimeManager.CreateInstance(eventReference);
        musicInstance.start();
    }
    private void InitializeAmbience(EventReference eventReference)
    {
        ambientInstance = RuntimeManager.CreateInstance(eventReference);
        ambientInstance.start();
    }

    public void setMusicArea(MusicArea area)
    {
        musicInstance.setParameterByName("Mode", (float)area);
    }

    public void PlayOneShot(EventReference sound, Vector3 position)
    {
        if (sound.IsNull)
        {
            if (DebugMode)
            {
                Debug.LogWarning($"Oneshot sound is null: {sound}");
            }

            return;
        }

        RuntimeManager.PlayOneShot(sound, position);
    }

    public void PlayLetterSound(EventReference sound, char letter, Vector3 position)
    {
        char lowerLetter = char.ToLower(letter);
        if (!System.Enum.TryParse(lowerLetter.ToString(), out Alphabet letterEnum))
        {
            Debug.LogWarning($"Letter '{letter}' not defined in Alphabet enum.");
            return;
        }
        EventInstance letterInstance = RuntimeManager.CreateInstance(sound);
        letterInstance.setParameterByName("Letter", (float)letterEnum);
        letterInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        letterInstance.start();
        letterInstance.release();
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
