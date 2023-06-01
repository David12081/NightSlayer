using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMeleeAttackStateData", menuName = "Data/State Data/Melee Attack State")]
public class D_MeleeAttack : ScriptableObject
{
    public float attackRadius = 0.5f;
    public int attackDamage = 10;
    public int knockbackForceX = 1;
    public int knockbackForceY = 1;

    public LayerMask whatIsPlayer;
}
