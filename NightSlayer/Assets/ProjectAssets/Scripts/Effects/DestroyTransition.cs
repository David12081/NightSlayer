using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTransition : MonoBehaviour
{
    [SerializeField] LayerMask ground;
    
    private void Update()
    {
        Collider2D groundDetection = Physics2D.OverlapCircle(transform.position, 1, ground);
        if(groundDetection != null)
        {
            Destroy(this.gameObject);
        }
    }
}