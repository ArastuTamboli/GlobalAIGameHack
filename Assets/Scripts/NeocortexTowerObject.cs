using Neocortex;
using Neocortex.Data;
using UnityEngine;

public class NeocortexTowerObject : MonoBehaviour
{
    public Interactables Type=>Interactables.TOWER;
    [HideInInspector]public string Name;
    [HideInInspector]public string towerType;
    public bool IsSubject { get; private set; }
    
    public void Init(string _name,string _towerType)
    {
        Name = _name;
        towerType = _towerType;
    }
    public Interactable ToInteractable()
    {
        InteractableProperty[] properties = null;

        if (!string.IsNullOrEmpty(towerType))
        {
            properties = new InteractableProperty[]
            {
                new InteractableProperty { name = "powerType", value = towerType},
            };
        }

        return new Interactable
        {
            type = Type.ToString(),
            name = Name,
            isSubject = IsSubject,
            position = transform.position,
            properties = properties
        };
    }
    public void UpgradeTower()
    {
        // Implement tower upgrade logic here
        Debug.Log($"Upgrading tower {Name} of type {towerType}");
    }
}
