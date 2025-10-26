using UnityEngine;
using UnityEngine.Events;
public enum TriggerPlace
{
    Normal,
    Weak,
    Critical
}
public class PathProgressTrigger : MonoBehaviour
{
    int pathID;
    public TriggerPlace place;
    [Header("Detection Settings")]

    [Range(0f, 1f)]
    public float enemyPercentageThreshold = 0.5f;
    public bool triggerOnce = true;
    public bool debugMode = true;
    
    [Header("Events")]
    public UnityEvent<float, int, int> onThresholdReached;

    private int enemiesPassed = 0;
    private int totalEnemiesExpected = 0;
    private bool hasTriggered = false;

    
    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"{gameObject.name}: Collider is not set as Trigger!");
        }
    }

    public void SetTotalEnemies(int total,int _pathID)
    {
        pathID = _pathID;
        totalEnemiesExpected = total;
        enemiesPassed = 0;
        hasTriggered = false;

        if (debugMode)
        {
            Debug.Log($"<color=cyan>[{gameObject.name}]</color> Expecting {total} enemies. Threshold: {enemyPercentageThreshold * 100}%");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (hasTriggered && triggerOnce) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        enemiesPassed++;

        if (totalEnemiesExpected <= 0)
        {
            if (debugMode)
            {
                Debug.LogWarning($"<color=yellow>[{gameObject.name}]</color> Total enemies not set! Call SetTotalEnemies() first.");
            }
            return;
        }

        float percentagePassed = (float)enemiesPassed / totalEnemiesExpected;

        if (debugMode)
        {
            Debug.Log($"<color=cyan>[{gameObject.name}]</color> Enemy passed! ({enemiesPassed}/{totalEnemiesExpected}) = {percentagePassed * 100:F1}%");
        }

        if (percentagePassed >= enemyPercentageThreshold)
        {
            OnThresholdReached(percentagePassed, enemiesPassed, totalEnemiesExpected);
        }
    }

    void OnThresholdReached(float percentage, int passed, int total)
    {
        if (hasTriggered && triggerOnce) return;

        hasTriggered = true;

        if (debugMode)
        {
            Debug.Log($"<color=yellow THRESHOLD REACHED! </color>");
            Debug.Log($"<color=yellow>[{gameObject.name}]</color> {percentage * 100:F1}% of enemies ({passed}/{total}) have passed this checkpoint!");
        }

        onThresholdReached?.Invoke(percentage, passed, total);

        //string message = "";
        //if(pathID == 0) //Orange Path
        //{
        //    message += "Orange Path";
        //}
        //else //Blue Path
        //{
        //    message += "Blue Path";
        //}

        //switch (place)
        //{
        //    case TriggerPlace.Normal:
        //        message += "Weak";
        //        break;
        //    case TriggerPlace.Weak:
        //        message += "Fragile";
        //        break;
        //    case TriggerPlace.Critical:
        //        message += "Broken";
        //        break;
        //    default:
        //        break;
        //}

     //  VoiceCommandController.instance.GiveTextInstructionsToNeocortex(message);
       VoiceCommandController.instance.GiveTextInstructionsToNeocortex();
      
    }

    public void ResetTrigger()
    {
        enemiesPassed = 0;
        hasTriggered = false;

        if (debugMode)
        {
            Debug.Log($"<color=cyan>[{gameObject.name}]</color> Reset");
        }
    }

    public int GetEnemiesPassed()
    {
        return enemiesPassed;
    }

    public float GetCurrentPercentage()
    {
        if (totalEnemiesExpected <= 0) return 0f;
        return (float)enemiesPassed / totalEnemiesExpected;
    }
}
