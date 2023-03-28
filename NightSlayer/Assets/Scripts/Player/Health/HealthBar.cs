using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject heartPrefab;
    public PlayerHealth playerHealth;
    List<HealthHeart> hearts = new List<HealthHeart>();

    private void Start()
    {
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearts();

        float maxHealthRemainder = playerHealth.livesLimit % 2;
        int heartsToMake = (int)(playerHealth.livesLimit / 2 + maxHealthRemainder);

        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(playerHealth.lives - (i * 2), 0, 2);
            hearts[i].SetHeartStatus((HeartStatus)heartStatusRemainder);
        }
    }
    
    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab, transform.position, Quaternion.identity, transform);
        newHeart.transform.SetParent(transform);

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartStatus(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }
}