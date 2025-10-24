using Neocortex;
using Neocortex.Data;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;
using static NeocortexCameraObject;

public class VoiceCommandController : MonoBehaviour
{
    [Header("References")]
    public NeocortexSmartAgent smartAgent;
    public NeocortexAudioReceiver audioReceiver;
    public CameraController cameraController;
    public TowerManager towerManager;
    public GridSystem gridSystem;

    [Header("Settings")]
    public bool autoStartRecording = false;
    public float recordingDuration = 5f;

    private bool isProcessing = false;

    void Start()
    {
        if (smartAgent == null)
            smartAgent = GetComponent<NeocortexSmartAgent>();
        if (audioReceiver == null)
            audioReceiver = GetComponent<NeocortexAudioReceiver>();

        smartAgent.OnChatResponseReceived.AddListener(OnAIResponseReceived);
        smartAgent.OnTranscriptionReceived.AddListener(OnTranscriptionReceived);
        smartAgent.OnAudioResponseReceived.AddListener(OnAudioResponseReceived);

        audioReceiver.OnAudioRecorded.AddListener(OnAudioRecorded);
    }

    public void StartVoiceInput()
    {
        Debug.Log("Listening for voice command...");
        audioReceiver.StartMicrophone();
      
    }

    void StopVoiceInput()
    {
        audioReceiver.StopMicrophone();
    }

    void OnAudioRecorded(AudioClip audioClip)
    {
        Debug.Log($"Audio recorded: {audioClip.samples} samples");
        smartAgent.AudioToText(audioClip);
    }

    void OnTranscriptionReceived(string transcription)
    {
        Debug.Log($"Transcription: {transcription}");
    }

    void OnAIResponseReceived(ChatResponse response)
    {
        Debug.Log($"AI Response: {response.message}");
        Debug.Log($"Action: {response.action}");
        Debug.Log($"Meta: {response.metadata.Length}");
        foreach (var item in response.metadata)
        {
            Debug.Log($"Meta: {item.name}");
        }
        // isProcessing = false;
        Interactable gridCell = null;
        Interactable towerData = null;
        Interactable camera = null;
        Interactable[] interactables = response.metadata.Where(i => i.isSubject).ToArray();
        foreach (var i in interactables)
        {
            Debug.Log($"Interactable: type={i.type}, name={i.name}");

            if (i.type == Interactables.GRID.ToString())
            {
                gridCell = i;
                Debug.Log($"Found grid cell: {i.name}");
            }
            else if (i.type == Interactables.TOWER.ToString())
            {
                towerData = i;
                Debug.Log($"tower: {i.name}");
            }
            else if(i.type == Interactables.CAMERA.ToString())
            {
                Debug.Log($"Found camera: {i.name}");
                camera = i; 
            }
        }

        string action = response.action;
        if (!string.IsNullOrEmpty(action))
        {
            
                if (action == "GO_TO_GRID" && camera != null)
                {
                    Debug.Log($"{action} {camera.name}");

                    MoveCamera(camera.position);

                }
                else if (action == "SPAWN_TOWER" && towerData !=null && gridCell!=null)
                {
                    Debug.Log($"{action} {towerData.name} {gridCell.name}");


                //need 2 interactable in metadata for this action
                HandleSpawnTower(towerData, gridCell);
            }
                else
                {
                    Debug.Log($"{action} action not defined in project");
                }
            
        }
        else
        {
            Debug.Log("Invalid Action");
        }

        StartVoiceInput();
    }

   
    private void OnAudioResponseReceived(AudioClip audioClip)
    {
        Debug.Log("Audio Received");
    }
   
    void MoveCamera(Vector3 Pos)
    {   
        cameraController.MoveToPosition(Pos);
        
    }
    void HandleSpawnTower(Interactable towerData, Interactable gridCell)
    {
        if (towerData == null)
        {
            Debug.LogWarning("No tower specified");
            return;
        }

        if (gridCell == null)
        {
            Debug.LogWarning("No grid cell specified");
            return;
        }
          Debug.Log($"Spawning {towerData.name} at {gridCell.name} )");
            towerManager.PlaceTower(towerData.name, gridCell.position);
        
    }

}
