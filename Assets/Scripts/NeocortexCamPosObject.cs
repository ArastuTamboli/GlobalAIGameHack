using Neocortex;
using Neocortex.Data;
using UnityEditor.Rendering;
using UnityEngine;


public class NeocortexCamPosObject : MonoBehaviour
{
    public InteractableType Type => InteractableType.OBJECT;
    public string Name;

    public bool IsSubject { get; private set; }
    [SerializeField]InteractableProperty[] properties;
    
    public Interactable ToInteractable()
    {
        return new Interactable
        {
            type = Type.ToString(),
            name = name,
            isSubject = IsSubject,
            position = transform.position,
            properties = properties
        };
    }
    
}
