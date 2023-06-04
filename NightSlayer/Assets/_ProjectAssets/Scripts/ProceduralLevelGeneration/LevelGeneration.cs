using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField] GameObject playerPosMinimap;
    [SerializeField] private Transform[] startingPositions;
    [SerializeField] private GameObject[] rooms; // index 0 -> LR, index 1 -> LRB, index 2 -> LRT, index 3 -> LRTB

    public List<GameObject> spawnedRooms;

    private int direction;
    [SerializeField] private float moveAmountX;
    [SerializeField] private float moveAmountY;

    private float timeBtwRoom;
    [SerializeField] private float startTimeBtwRoom = 0.25f;

    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    private bool stopGeneration;
    public bool StopGeneration
    {
        get => stopGeneration;
        set => stopGeneration = value;
    }

    [SerializeField] private LayerMask room;

    private int downCounter;

    [SerializeField] private GameObject[] prefabs;

    [SerializeField] private GameObject[] initialRooms;

    private bool spawned = false;

    private void Start()
    {
        int randStartingPos = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randStartingPos].position;
        Instantiate(rooms[0], transform.position, Quaternion.identity);

        direction = Random.Range(1, 6);

        spawned = false;
    }

    private void Update()
    {
        if(timeBtwRoom <= 0 && !stopGeneration)
        {
            Move();
            timeBtwRoom = startTimeBtwRoom;
        }
        else
        {
            timeBtwRoom -= Time.deltaTime;
        }

        if(stopGeneration)
        {
            for (int i = 0; i < spawnedRooms.Count; i++)
            {
                if (spawnedRooms[i] == null)
                {
                    spawnedRooms.RemoveAt(i);
                }
            }

            if(!spawned)
                StartCoroutine(SpawnPrefabs());
        }
    }

    IEnumerator SpawnPrefabs()
    {
        spawned = true;
        RoomType roomType = spawnedRooms[0].GetComponent<RoomType>();
        if (roomType.Type == 0)
        {
            Instantiate(initialRooms[0], spawnedRooms[0].transform.position, Quaternion.identity);
            Debug.Log(roomType.Type);
            Destroy(roomType.gameObject);
        }
        else if(roomType.Type == 1)
        {
            Instantiate(initialRooms[1], spawnedRooms[0].transform.position, Quaternion.identity);
            Debug.Log(roomType.Type);
            Destroy(roomType.gameObject);
        }
        else if(roomType.Type == 3)
        {
            Instantiate(initialRooms[2], spawnedRooms[0].transform.position, Quaternion.identity);
            Debug.Log(roomType.Type);
            Destroy(roomType.gameObject);
        }
        yield return new WaitForSeconds(1f);
        Instantiate(prefabs[0], spawnedRooms[spawnedRooms.Count - 1].transform.position, Quaternion.identity);
        Instantiate(prefabs[1], spawnedRooms[spawnedRooms.Count - 1].transform.position, Quaternion.identity);
        CinemachineShake.Instance.cinemachineConfiner.m_BoundingShape2D = spawnedRooms[spawnedRooms.Count - 1].GetComponentInChildren<PolygonCollider2D>();
        spawnedRooms[spawnedRooms.Count - 1].gameObject.transform.Find("MinimapIcon").gameObject.SetActive(true);
        Instantiate(playerPosMinimap, new Vector3(spawnedRooms[spawnedRooms.Count - 1].transform.position.x, spawnedRooms[spawnedRooms.Count - 1].transform.position.y, playerPosMinimap.transform.position.z), Quaternion.identity);
        Destroy(this.gameObject);
    }

    void Move()
    {
        if(direction == 1 || direction == 2) // Move RIGHT
        {
            if(transform.position.x < maxX)
            {
                downCounter = 0;

                Vector2 newPos = new Vector2(transform.position.x + moveAmountX, transform.position.y);
                transform.position = newPos;

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(1, 6);
                if(direction == 3)
                {
                    direction = 2;
                }
                else if(direction == 4)
                {
                    direction = 5;
                }
            }
            else
            {
                direction = 5;
            }
        }
        else if(direction == 3 || direction == 4) // Move LEFT
        {
            if (transform.position.x > minX)
            {
                downCounter = 0;

                Vector2 newPos = new Vector2(transform.position.x - moveAmountX, transform.position.y);
                transform.position = newPos;

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(3, 6);
            }
            else
            {
                direction = 5;
            }
        }
        else if(direction == 5) // Move DOWN
        {
            downCounter++;
            
            if(transform.position.y > minY)
            {
                Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, room);
                if(roomDetection.GetComponent<RoomType>().Type != 1 && roomDetection.GetComponent<RoomType>().Type != 3)
                {
                    if(downCounter >= 2)
                    {
                        roomDetection.GetComponent<RoomType>().RoomDestruction();
                        Instantiate(rooms[3], transform.position, Quaternion.identity);
                    }
                    else
                    {
                        roomDetection.GetComponent<RoomType>().RoomDestruction();

                        int randBottomRoom = Random.Range(1, 4);
                        if (randBottomRoom == 2)
                        {
                            randBottomRoom = 1;
                        }
                        Instantiate(rooms[randBottomRoom], transform.position, Quaternion.identity);
                    }
                }
                
                Vector2 newPos = new Vector2(transform.position.x, transform.position.y - moveAmountY);
                transform.position = newPos;

                int rand = Random.Range(2, 4);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(1, 6);
            }
            else
            {
                stopGeneration = true;
            }
        } 
    }
}