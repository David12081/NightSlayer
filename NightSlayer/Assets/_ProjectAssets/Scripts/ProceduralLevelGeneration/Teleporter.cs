using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] string scene;
    private bool playerOnPlatform;
    private PlayerScript playerScript;

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
        SceneManager.LoadScene(scene);
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