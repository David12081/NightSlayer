using UnityEngine;

public class ShadowAnimations : MonoBehaviour
{
    [SerializeField] SpriteRenderer playerRenderer;
    SpriteRenderer shadowRenderer;

    private void Start()
    {
        shadowRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        shadowRenderer.sprite = playerRenderer.sprite;
    }
}