using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    Transform target;
    public GameObject playerPrefab;

    private void Start()
    {
        if(target == null)
        {
            Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        transform.position = target.position;
    }
}