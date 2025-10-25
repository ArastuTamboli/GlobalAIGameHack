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
        
        Interactable interactable = response.metadata.FirstOrDefault(i => i.isSubject);

        string action = response.action;

        if (!string.IsNullOrEmpty(action))
        {
                if (action == "GO_TO_POINT")
                {
                    MoveCamera(interactable.position);
                }
                else
                {
                    Debug.Log($"{action} action error");
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
