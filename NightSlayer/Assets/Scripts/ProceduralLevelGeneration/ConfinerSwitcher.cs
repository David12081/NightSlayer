using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfinerSwitcher : MonoBehaviour
{
    [SerializeField] PolygonCollider2D polygonCollider;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform minimapIcon;
    [SerializeField] int confinerType;
    private GameObject playerPosMinimap;
    private PlayerScript playerScript;
    private bool playerOnCollider;
    private SceneFade sceneFade;

    private void Start()
    {
        StartCoroutine(AssignPlayerMinimap());
        minimapIcon.gameObject.SetActive(false);
        sceneFade = GameObject.Find("SceneFade").GetComponent<SceneFade>();
    }

    private void Update()
    {
        if (playerOnCollider)
        {
            StartCoroutine(sceneFade.FadeOut());
            minimapIcon.gameObject.SetActive(true);
            CinemachineShake.Instance.cinemachineConfiner.InvalidatePathCache();
            CinemachineShake.Instance.cinemachineConfiner.m_BoundingShape2D = polygonCollider;
            playerPosMinimap.transform.position = minimapIcon.transform.position;
            if(confinerType == 0)
            {
                playerScript.gameObject.transform.position = new Vector3(spawnPoint.position.x, playerScript.gameObject.transform.position.y, spawnPoint.position.z);
            }
            else if(confinerType == 1)
            {
                playerScript.gameObject.transform.position = spawnPoint.position;
            }
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