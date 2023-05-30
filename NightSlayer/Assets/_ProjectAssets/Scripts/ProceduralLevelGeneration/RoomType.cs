using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomType : MonoBehaviour
{
    [SerializeField] private int type;
    private Collider2D platformCollider;

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
        platformCollider = this.gameObject.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if(levelGeneration.StopGeneration)
        {
            platformCollider.enabled = false;
        }
    }

    public void RoomDestruction()
    {
        Destroy(gameObject);
    }
}