using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{
    public LayerMask room;
    public LevelGeneration levelGeneration;
    [SerializeField] GameObject roomClosed;
    
    void Update()
    {
        Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, room);
        if(roomDetection == null && levelGeneration.StopGeneration == true)
        {
            Instantiate(roomClosed, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if(roomDetection != null && levelGeneration.StopGeneration == true)
        {
            Destroy(gameObject);
        }
    }
}