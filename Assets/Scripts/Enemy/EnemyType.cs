using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Type", menuName = "Tower Defense/Enemy Type")]
public class EnemyType : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName;
    public GameObject prefab;

    [Header("Base Stats")]
    public float baseHealth = 100f;
    public float baseDamage = 10f;
    public float baseSpeed = 3f;
    public int baseReward = 10;


}
