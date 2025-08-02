using UnityEngine;

public class DeathArea : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        Debug.Log(other);
    }

}
