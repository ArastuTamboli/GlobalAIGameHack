using Neocortex;
using Neocortex.Data;
using UnityEngine;

public class NeocortexTowerObject : MonoBehaviour
{
   
    public Interactables Type=>Interactables.TOWER;

    [Tooltip("The name of the interactable. This name will be used to identify the interactable object.")]
    public string Name;

    [Header("Grid Properties")]
    public string powerType;
  

    public Interactable ToInteractable(bool isSubject = false)
    {
        InteractableProperty[] properties = null;

        if (!string.IsNullOrEmpty(powerType))
        {
            properties = new InteractableProperty[]
            {
                new InteractableProperty { name = "powerType", value = powerType},
            };
        }

        return new Interactable
        {
            type = Type.ToString(),
            name = Name,
            isSubject = isSubject,
            position = transform.position,
            properties = properties
        };
    }
}
