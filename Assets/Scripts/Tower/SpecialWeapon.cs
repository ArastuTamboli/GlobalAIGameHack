using UnityEngine;
using System.Collections;

public class SpecialWeaponBehavior : MonoBehaviour
{
    [Header("Weapon Info")]
    public string weaponName;
    public SpecialWeaponType weaponType;

    [Header("Stats")]
    public float cooldownTime = 40f;
    public float damage = 1200f;
    public float duration = 5f;
    public float slowMultiplier = 0.5f;

    [Header("Visual")]
    public GameObject activationEffect;
    public Transform firePoint;

    [Header("Targeting")]
    public LayerMask enemyLayer;

    private float currentCooldown = 0f;
    private WaveManager waveManager;

    public bool IsReady => currentCooldown <= 0f;

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
    }

    void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    public void Activate()
    {
        if (!IsReady)
        {
            Debug.Log($"{weaponName} is on cooldown: {currentCooldown:F1}s remaining");
            return;
        }

        switch (weaponType)
        {
            case SpecialWeaponType.OmegaRailgun:
                ActivateOmegaRailgun();
                break;
            case SpecialWeaponType.ChronoPulseCannon:
                ActivateChronoPulseCannon();
                break;
        }

        currentCooldown = cooldownTime;
    }

    void ActivateOmegaRailgun()
    {
        int currentWave = waveManager != null ? waveManager.currentWaveIndex + 1 : 1;
        float scaledDamage = damage * (1 + 0.1f * currentWave);

        Enemy target = GetFurthestEnemy();

        if (target != null)
        {
            target.TakeDamage(scaledDamage);

            if (activationEffect != null && firePoint != null)
            {
                GameObject effect = Instantiate(activationEffect, firePoint.position, firePoint.rotation);
                Destroy(effect, 3f);
            }

            Debug.Log($"{weaponName} fired! Dealt {scaledDamage} damage to {target.name}");
        }
        else
        {
            Debug.Log($"{weaponName} has no target!");
        }
    }

    void ActivateChronoPulseCannon()
    {
        StartCoroutine(ChronoSlowCoroutine());

        if (activationEffect != null && firePoint != null)
        {
            GameObject effect = Instantiate(activationEffect, firePoint.position, Quaternion.identity);
            Destroy(effect, duration);
        }

        Debug.Log($"{weaponName} activated! Slowing all enemies for {duration}s");
    }

    IEnumerator ChronoSlowCoroutine()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, 1000f, enemyLayer);

        foreach (Collider col in enemyColliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ApplySlow(slowMultiplier);
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (Collider col in enemyColliders)
        {
            if (col != null)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.RemoveSlow();
                }
            }
        }
    }

    Enemy GetFurthestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1000f, enemyLayer);

        Enemy furthest = null;
        float furthestProgress = -1f;

        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                float progress = enemy.GetPathProgress();
                if (progress > furthestProgress)
                {
                    furthestProgress = progress;
                    furthest = enemy;
                }
            }
        }

        return furthest;
    }

    public float GetCooldownPercent()
    {
        return Mathf.Clamp01(1f - (currentCooldown / cooldownTime));
    }
}

public enum SpecialWeaponType
{
    OmegaRailgun,
    ChronoPulseCannon
}
