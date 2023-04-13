using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomType : MonoBehaviour
{
    public int type;
    private LevelGeneration levelGeneration;

    private void Start()
    {
        levelGeneration = GameObject.Find("Level Generation").GetComponent<LevelGeneration>();
        levelGeneration.spawnedRooms.Add(this.gameObject);
    }

    public void RoomDestruction()
    {
        Destroy(gameObject);
    }
}