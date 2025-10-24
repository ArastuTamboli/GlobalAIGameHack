using Neocortex;
using Neocortex.Data;
using UnityEngine;
public enum Interactables
{
    GRID,
    TOWER,
    CAMERA,
}

public class NeocortexGridObject : MonoBehaviour
{
   
    public Interactables Type=>Interactables.GRID;

    public Interactable ToInteractable(string name,string column,int number, bool isSubject = false)
    {
        InteractableProperty[] properties = null;

        if (!string.IsNullOrEmpty(column) || number >= 0)
        {
            properties = new InteractableProperty[]
            {
                new InteractableProperty { name = "column", value = column },
                new InteractableProperty { name = "number", value = number.ToString() }
            };
        }

        return new Interactable
        {
            type = Type.ToString(),
            name = name,
            isSubject = isSubject,
            position = transform.position,
            properties = properties
        };
    }
}
