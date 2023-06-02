using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public struct AttackDetails
{
    public Vector3 position;
    public float damageAmount;
    public float stunDamageAmount;
    public int knockbackForceX;
    public int knockbackForceY;
}