using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveScene : MonoBehaviour
{
    private PlayerController playerController;
    public SaveLoadSystem saveLoadSystem;

    public GameObject interactText;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerController = collision.GetComponent<PlayerController>();
            interactText.SetActive(true);
            if (playerController.interact)
            {
                string activeScene = SceneManager.GetActiveScene().name;
                PlayerPrefs.SetString("SavedLevel", activeScene);

                saveLoadSystem.Save();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interactText.SetActive(false);
        }
    }
}