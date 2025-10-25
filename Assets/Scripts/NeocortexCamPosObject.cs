using Neocortex;
using Neocortex.Data;
using UnityEditor.Rendering;
using UnityEngine;
public enum Interactables
{
    TOWER,
    CAMPOS,
}

public class NeocortexCamPosObject : MonoBehaviour
{
    public Interactables Type => Interactables.CAMPOS;
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
