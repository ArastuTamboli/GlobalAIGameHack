using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 20f;
    public float lifetime = 5f;
    public GameObject hitEffect;

    private Enemy target;
    private float damage;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Enemy targetEnemy, float projectileDamage)
    {
        target = targetEnemy;
        damage = projectileDamage;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            Hit();
        }
    }

    void Hit()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemy == target)
        {
            Hit();
        }
    }
}
