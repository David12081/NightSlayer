using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    [SerializeField] GameObject[] objects;

    private void Start()
    {
        int rand = Random.Range(0, objects.Length);
        Instantiate(objects[rand], transform.position, Quaternion.identity);
        StartCoroutine(DestroyTemplate());
    }

    IEnumerator DestroyTemplate()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}