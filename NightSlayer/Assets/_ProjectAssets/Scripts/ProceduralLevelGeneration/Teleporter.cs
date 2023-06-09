using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] SceneAsset[] scenes;
    int nextSceneIndex;
    private bool playerOnPlatform;
    private PlayerScript playerScript;

    private void Start()
    {
        int nextSceneIndex = Random.Range(0, scenes.Length);
    }

    private void Update()
    {
        if (playerOnPlatform && playerScript.InputY >= 0.5f)
        {
            anim.SetTrigger("Close");
            playerScript.gameObject.SetActive(false);
            FindObjectOfType<SceneFade>().FadeIn();
            StartCoroutine(LoadScene());
        }
    }

    void SetPlayerOnPlatform(Collider2D other, bool value)
    {
        var player = other.gameObject.GetComponent<PlayerScript>();
        if (player != null)
        {
            playerScript = player;
            playerOnPlatform = value;
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scenes[nextSceneIndex].name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetPlayerOnPlatform(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SetPlayerOnPlatform(collision, false);
    }
}