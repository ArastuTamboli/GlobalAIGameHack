using Neocortex;
using Neocortex.Data;
using UnityEngine;
public class VoiceCommandController : MonoBehaviour
{
    public static VoiceCommandController instance;
    [Header("References")]
    public NeocortexSmartAgent smartAgent;
    public NeocortexAudioReceiver audioReceiver;
    public CameraController cameraController;
    public TowerManager towerManager;
    //[Header("Settings")]
    //public bool autoStartRecording = false;
    //public float recordingDuration = 5f;
    //private bool isProcessing = false;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        EnableDetection();
        //StartVoiceInput();
        if (smartAgent == null)
            smartAgent = GetComponent<NeocortexSmartAgent>();
        if (audioReceiver == null)
           audioReceiver = GetComponent<NeocortexAudioReceiver>();
           

        smartAgent.OnChatResponseReceived.AddListener(OnAIResponseReceived);
        smartAgent.OnTranscriptionReceived.AddListener(OnTranscriptionReceived);
        smartAgent.OnAudioResponseReceived.AddListener(OnAudioResponseReceived);
        audioReceiver.OnAudioRecorded.AddListener(OnAudioRecorded);
    }

    //call this from gameplay logic
    public void GiveTextInstructionsToNeocortex(string message)
    {
        if (!enableText) return;
        
        Debug.Log("GIVE INST");
        smartAgent.TextToAudio(message);
        DisableDetection();
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
       // smartAgent.AudioToText(audioClip);
 
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

        //Interactable interactable = response.metadata.FirstOrDefault(i => i.isSubject);

        //string action = response.action;

        //if (!string.IsNullOrEmpty(action))
        //{
        //        if (action == "GO_TO_POINT")
        //        {
        //            MoveCamera(interactable.position);
        //        }
        //        else
        //        {
        //            Debug.Log($"{action} action error");
        //        }
        //}
        //else
        //{
        //    Debug.Log("Invalid Action");
        //}

        // StartVoiceInput();
    }
    private void OnAudioResponseReceived(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        Debug.Log("Audio Received");
        Invoke(nameof(EnableDetection), audioClip.length);
    }
    bool enableText;

    private void EnableDetection()
    {
        EnableDetection(true);
    }
    private void DisableDetection()
    {
        EnableDetection(false);
    }
    void EnableDetection(bool enable)
    {
        enableText = enable;
    }
    void MoveCamera(Vector3 Pos)
    {   
        cameraController.MoveToPosition(Pos);
        
    }
   

}
