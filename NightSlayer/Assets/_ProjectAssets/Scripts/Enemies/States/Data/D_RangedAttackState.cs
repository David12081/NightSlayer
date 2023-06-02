using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newRangedAttackStateData", menuName = "Data/State Data/Ranged Attack State")]
public class D_RangedAttackState : ScriptableObject
{
    public GameObject projectile;
    public float projectileDamage = 10f;
    public int knockbackForceX = 1;
    public int knockbackForceY = 1;
    public float projectileSpeed = 12f;
    public float projectileTravelDistance;
}
