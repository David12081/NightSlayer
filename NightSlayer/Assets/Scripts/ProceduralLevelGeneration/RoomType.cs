using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomType : MonoBehaviour
{
    [SerializeField] private int type;
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
    }

    public void RoomDestruction()
    {
        Destroy(gameObject);
    }
}