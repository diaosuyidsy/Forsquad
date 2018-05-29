using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Tooltip ("Max Health Point")]
    public float maxHealthPoint = 100;

    [Tooltip ("Max Armor, defalt is 1, meaning taking 100% damage")]
    [Range (0f, 2.0f)]
    public float maxArmor = 1f;

    public GameObject BloodParPrefab;

    private float Armor;
    private float HealthPoint;

    // Use this for initialization
    void Start ()
    {
        HealthPoint = maxHealthPoint;
        Armor = maxArmor;
    }

    public void OnHit (float rawDmg)
    {
        // First, deal with visual， on hit drew blood
        Instantiate (BloodParPrefab, transform.position, Quaternion.identity);

        float realDMG = rawDmg * (1f + maxArmor - Armor);
        HealthPoint -= realDMG;
        if (HealthPoint <= 0f)
        {
            Destroy (gameObject);
        }
    }
}
