using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplate : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    public GameObject[] bossRoom;
    public GameObject closedRoom;

    public List<GameObject> rooms;
    private int rand;
    public float waitTime;
    private bool spawnedBoss;
    
    void Update()
    {

        if (waitTime <= 0 && spawnedBoss == false)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (i == rooms.Count - 1)
                {
                    rand = Random.Range(0, bossRoom.Length);
                    Instantiate(bossRoom[rand], rooms[i].transform.position, Quaternion.identity);
                    Destroy(rooms[i].gameObject);
                    spawnedBoss = true;
                }
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }
}