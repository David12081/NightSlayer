using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFade : MonoBehaviour
{
    public Image blackImage;
    float alpha;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeTo(string targetScene)
    {
        StartCoroutine(FadeOut(targetScene));
    }

    IEnumerator FadeIn()
    {
        alpha = 1;

        while(alpha > 0)
        {
            alpha -= Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0f);
        }
    }

    IEnumerator FadeOut(string targetScene)
    {
        alpha = 0;

        while (alpha < 0)
        {
            alpha += Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0f);
        }
        SceneManager.LoadScene(targetScene);
    }
}