using UnityEngine;
using System.Collections;

public class TowerBehavior : MonoBehaviour
{
    [Header("Tower Configuration")]
    public TowerData towerData;

    [Header("Current Stats")]
    public float damage;
    public float rateOfFire;
    public float range;
    public int upgradeLevel = 0;

    [Header("Combat")]
    public Transform firePoint;
    public LayerMask enemyLayer;
    public Transform rotatePoint;

    [Header("Visual")]
    public GameObject rangeIndicator;
    public LineRenderer beamLineRenderer;

    private const int MAX_UPGRADE_LEVEL = 3;
    private const float STRONG_MULTIPLIER = 1.5f;
    private const float WEAK_MULTIPLIER = 0.5f;
    private const float SELL_VALUE_PERCENT = 0.7f;

    private Enemy currentTarget;
    private float fireCooldown = 0f;
    private int totalInvested = 0;
    void Start()
    {
        if (towerData != null)
        {
            InitializeTower();
        }

        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
        }

    }

  

    void InitializeTower()
    {
        damage = towerData.baseDamage;
        rateOfFire = towerData.baseRateOfFire;
        range = towerData.baseRange;
        totalInvested = towerData.baseCost;
        if (beamLineRenderer != null)
        {
            beamLineRenderer.startWidth = towerData.beamWidth;
            beamLineRenderer.endWidth = towerData.beamWidth * 0.5f;
            beamLineRenderer.startColor = towerData.beamColor;
            beamLineRenderer.endColor = towerData.beamColor;
        }

        UpdateRangeIndicator();
    }

    void Update()
    {
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        FindTarget();

        if (currentTarget != null)
        {
            RotateTowardsTarget();

            if (fireCooldown <= 0)
            {
                Fire();
                fireCooldown = rateOfFire;
            }
        }
    }

    void FindTarget()
    {
        if (currentTarget == null || !IsTargetValid(currentTarget))
        {
            currentTarget = GetNearestEnemy();
        }
    }

    bool IsTargetValid(Enemy enemy)
    {
        if (enemy == null) return false;

        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        return distance <= range;
    }

    Enemy GetNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

        Enemy nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = enemy;
                }
            }
        }

        return nearest;
    }

    void RotateTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.transform.position - rotatePoint.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        rotatePoint.rotation = Quaternion.Slerp(rotatePoint.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Fire()
    {
        if (currentTarget == null || firePoint == null) return;

        Vector3 targetPosition = currentTarget.transform.position;
        Vector3 direction = (targetPosition - firePoint.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, direction, out hit, range, enemyLayer))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null && enemy == currentTarget)
            {
                float effectiveDamage = CalculateEffectiveDamage(enemy);
                enemy.TakeDamage(effectiveDamage);

                ShowBeamEffect(firePoint.position, hit.point);

                if (towerData.hitEffect != null)
                {
                    Instantiate(towerData.hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }
        else
        {
            ShowBeamEffect(firePoint.position, targetPosition);
        }

        if (towerData.muzzleFlashEffect != null)
        {
            Instantiate(towerData.muzzleFlashEffect, firePoint.position, firePoint.rotation);
        }
    }

    void ShowBeamEffect(Vector3 start, Vector3 end)
    {
        if (beamLineRenderer == null) return;

        StopCoroutine(nameof(BeamEffectCoroutine));
        StartCoroutine(BeamEffectCoroutine(start, end));
    }

    IEnumerator BeamEffectCoroutine(Vector3 start, Vector3 end)
    {
        beamLineRenderer.enabled = true;
        beamLineRenderer.SetPosition(0, start);
        beamLineRenderer.SetPosition(1, end);

        float duration = towerData != null ? towerData.beamDuration : 0.1f;
        yield return new WaitForSeconds(duration);

        beamLineRenderer.enabled = false;
    }

    float CalculateEffectiveDamage(Enemy enemy)
    {
        float effectiveDamage = damage;

        EnemyTypeHolder enemyTypeHolder = enemy.GetComponent<EnemyTypeHolder>();
        if (enemyTypeHolder != null && enemyTypeHolder.enemyType != null)
        {
            if (IsStrongAgainst(enemyTypeHolder.enemyType))
            {
                effectiveDamage *= STRONG_MULTIPLIER;
            }
            else if (IsWeakAgainst(enemyTypeHolder.enemyType))
            {
                effectiveDamage *= WEAK_MULTIPLIER;
            }
        }

        return effectiveDamage;
    }

    bool IsStrongAgainst(EnemyType enemyType)
    {
        if (towerData.strongAgainst == null) return false;

        foreach (EnemyType type in towerData.strongAgainst)
        {
            if (type == enemyType) return true;
        }
        return false;
    }

    bool IsWeakAgainst(EnemyType enemyType)
    {
        if (towerData.weakAgainst == null) return false;

        foreach (EnemyType type in towerData.weakAgainst)
        {
            if (type == enemyType) return true;
        }
        return false;
    }

    public bool CanUpgrade()
    {
        return upgradeLevel < MAX_UPGRADE_LEVEL;
    }

    public int GetUpgradeCost()
    {
        if (!CanUpgrade()) return 0;
        return towerData.GetUpgradeCost(upgradeLevel);
    }
    public int GetSellValue()
    {
        return Mathf.RoundToInt(totalInvested * SELL_VALUE_PERCENT);
    }
    public void UpgradeToLevel(int level)
    {
        if (level < 0 || level > MAX_UPGRADE_LEVEL) return;

        upgradeLevel = level;

        damage = towerData.baseDamage * Mathf.Pow(towerData.damagePerLevel, upgradeLevel);
        range = towerData.baseRange * Mathf.Pow(towerData.rangePerLevel, upgradeLevel);
        rateOfFire = towerData.baseRateOfFire / Mathf.Pow(towerData.rateOfFirePerLevel, upgradeLevel);

        UpdateRangeIndicator();

        Debug.Log($"{towerData.towerName} upgraded to Level {upgradeLevel + 1}");
    }

    public void Upgrade()
    {
        if (!CanUpgrade()) return;

        int cost = GetUpgradeCost();
        totalInvested += cost;

        UpgradeToLevel(upgradeLevel + 1);
    }

    public void Sell()
    {
        int sellValue = GetSellValue();

        if (GameManager.instance != null)
        {
            GameManager.instance.AddMoney(sellValue);
        }

        Debug.Log($"Sold {towerData.towerName} for ${sellValue}");

        Destroy(gameObject);
    }
    void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * range * 2f;
        }
    }

    public void ShowRange()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
        }
    }

    public void HideRange()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint != null ? firePoint.position : transform.position, currentTarget.transform.position);
        }
    }
}
