using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomType : MonoBehaviour
{
    [SerializeField] private int type;
    private Collider2D collider2D; 

    public int Type
    {
        get => type;
        set => type = value;
    }
    private LevelGeneration levelGeneration;

    private void Start()
    {
        levelGeneration = GameObject.Find("Level Generation").GetComponent<LevelGeneration>();
        levelGeneration.spawnedRooms.Add(this.gameObject);
        collider2D = this.gameObject.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if(levelGeneration.StopGeneration)
        {
            collider2D.enabled = false;
        }
    }

    public void RoomDestruction()
    {
        Destroy(gameObject);
    }
}