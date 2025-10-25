using Neocortex;
using Neocortex.Data;
using System.Linq;
using UnityEngine;


public class VoiceCommandController : MonoBehaviour
{
    [Header("References")]
    public NeocortexSmartAgent smartAgent;
    public NeocortexAudioReceiver audioReceiver;
    public CameraController cameraController;
    public TowerManager towerManager;
    public CameraSystem gridSystem;

    //[Header("Settings")]
    //public bool autoStartRecording = false;
    //public float recordingDuration = 5f;

    //private bool isProcessing = false;

    void Start()
    {
        StartVoiceInput();
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
      //  smartAgent.AudioToText(audioClip);
        smartAgent.AudioToAudio(audioClip);
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

        Interactable tower = null;
        Interactable cameraPos = null;
        Interactable[] interactables = response.metadata.Where(i => i.isSubject).ToArray();
        foreach (var i in interactables)
        {
            Debug.Log($"Interactable: type={i.type}, name={i.name}");

            if (i.type == Interactables.TOWER.ToString())
            {
                tower = i;
                Debug.Log($"tower: {i.name}");
            }
            else if(i.type == Interactables.CAMPOS.ToString())
            {
                Debug.Log($"camera pos: {i.name}");
                cameraPos = i; 
            }
        }

        string action = response.action;
        if (!string.IsNullOrEmpty(action))
        {
            
                if (action == "GO_TO_GRID" && cameraPos != null)
                {
                    Debug.Log($"{action} {cameraPos.name}");

                    MoveCamera(cameraPos.position);

                }
                else if (action == "UPGRADE_TOWER" && tower !=null)
                {
                    Debug.Log($"{action} {tower.name}");

                    
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
   

}
