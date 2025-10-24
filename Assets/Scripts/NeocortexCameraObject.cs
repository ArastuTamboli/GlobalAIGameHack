using Neocortex;
using Neocortex.Data;
using UnityEngine;


public class NeocortexCameraObject : MonoBehaviour
{
   
    public Interactables Type => Interactables.CAMERA;

    InteractableProperty[] properties;

    public Interactable ToInteractable(bool isSubject = false)
    {
       
        return new Interactable
        {
            type = Type.ToString(),
           // name = Name,
            isSubject = isSubject,
            position = transform.position,
           // properties = properties
        };
    }
}
