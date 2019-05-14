using UnityEngine;

public class Interactable : MonoBehaviour
{

    // This method is meant to be overwritten.
    public virtual void Interact()
    {
        Debug.Log("Player is interacting with " + transform.name);
    }

}
