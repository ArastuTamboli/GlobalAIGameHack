using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth;
    public float currentHealth;
    public float damage;
    public float speed;
    public int reward;

    [Header("Movement")]
    public Transform[] waypoints;
    public int currentWaypointIndex = 0;

    [Header("Effects")]
    public GameObject deathEffect;
    public GameObject reachGoalEffect;

    [Header("UI")]

    [SerializeField]private EnemyHealthBar healthBar;

    private bool isDead = false;
    private bool isStopped = false;
    private GameManager gameManager;
    public WaveManager waveManager;

    private float originalSpeed;
    private bool isSlowed = false;

    public void Initialize(EnemyType type, int waveNumber, float healthMultiplier, float damageMultiplier)
    {
        maxHealth = type.baseHealth * Mathf.Pow(healthMultiplier, waveNumber - 1);
        currentHealth = maxHealth;
        damage = type.baseDamage * Mathf.Pow(damageMultiplier, waveNumber - 1);
        speed = type.baseSpeed;
        reward = Mathf.RoundToInt(type.baseReward * (1 + (waveNumber - 1) * 0.1f));

        gameManager = GameManager.instance;

        CreateHealthBar();
    }

    void CreateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.Initialize();
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (isDead || isStopped || waypoints == null || waypoints.Length == 0) return;

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        if (currentWaypointIndex >= waypoints.Length)
        {
            ReachGoal();
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (gameManager != null)
        {
            gameManager.AddMoney(reward);
        }

        if (waveManager != null)
        {
            waveManager.UnregisterEnemy(this);
        }

        Destroy(gameObject);
    }

    void ReachGoal()
    {
        isDead = true;

        if (reachGoalEffect != null)
        {
            Instantiate(reachGoalEffect, transform.position, Quaternion.identity);
        }

        if (gameManager != null)
        {
            gameManager.TakeDamage((int)damage);
        }

        if (waveManager != null)
        {
            waveManager.UnregisterEnemy(this);
        }

        Destroy(gameObject);
    }

    public void StopMovement()
    {
        isStopped = true;
    }

    public void ApplySlow(float slowMultiplier)
    {
        if (!isSlowed)
        {
            originalSpeed = speed;
            isSlowed = true;
        }
        speed = originalSpeed * slowMultiplier;
    }

    public void RemoveSlow()
    {
        if (isSlowed)
        {
            speed = originalSpeed;
            isSlowed = false;
        }
    }

    public float GetPathProgress()
    {
        if (waypoints == null || waypoints.Length == 0) return 0f;
        return (float)currentWaypointIndex / waypoints.Length;
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = currentWaypointIndex; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
