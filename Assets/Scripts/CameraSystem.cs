using Neocortex;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
   
    private Dictionary<string, Vector3> cameraPositions = new Dictionary<string, Vector3>();
    [SerializeField] NeocortexCamPosObject[] cameraPos;

    void Start()
    {
        foreach (var interactable in cameraPos)
        {
            NeocortexCamPosObject obj = interactable.GetComponent<NeocortexCamPosObject>();
            cameraPositions[obj.Name] = obj.transform.position;

        }
    }

    

  
}
