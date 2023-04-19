using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfinerSwitcher : MonoBehaviour
{
    [SerializeField] PolygonCollider2D polygonCollider;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform minimapIcon;
    private GameObject playerPosMinimap;
    private PlayerScript playerScript;
    private bool playerOnCollider;

    private void Start()
    {
        StartCoroutine(AssignPlayerMinimap());
    }
    private void Update()
    {
        if (playerOnCollider)
        {
            CinemachineShake.Instance.cinemachineConfiner.InvalidatePathCache();
            CinemachineShake.Instance.cinemachineConfiner.m_BoundingShape2D = polygonCollider;
            playerScript.gameObject.transform.position = spawnPoint.position;
            playerPosMinimap.transform.position = minimapIcon.transform.position;
        }
    }

    IEnumerator AssignPlayerMinimap()
    {
        yield return new WaitForSeconds(5f);
        playerPosMinimap = GameObject.Find("PlayerPositionMinimap(Clone)").gameObject;
    }

    void SetPlayerOnCollider(Collider2D other, bool value)
    {
        var player = other.gameObject.GetComponent<PlayerScript>();
        if (player != null)
        {
            playerScript = player;
            playerOnCollider = value;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetPlayerOnCollider(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SetPlayerOnCollider(collision, false);
    }
}