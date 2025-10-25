using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Settings")]
    public int damageOnTouch = 10;
    public GameObject destroyEffect;


    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, other.transform.position, Quaternion.identity);
            }

            if (GameManager.instance != null)
            {
                GameManager.instance.TakeDamage((int)enemy.damage);
            }

            Destroy(other.gameObject);
        }
    }
}
