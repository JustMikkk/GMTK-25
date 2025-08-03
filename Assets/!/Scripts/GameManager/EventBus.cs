using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    public static UnityEvent levelStartedEvent = new();
    public static UnityEvent videoModeEnteredEvent = new();
    public static UnityEvent destroyingLevelEvent = new();
}
