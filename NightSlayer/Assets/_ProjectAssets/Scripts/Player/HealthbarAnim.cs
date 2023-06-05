using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarAnim : MonoBehaviour
{
    Vector3 healthbarInitialPos;
    [SerializeField] float shakeMagnitude = 0.05f;
    [SerializeField] float shakeTime = 0.2f;
    [SerializeField] RectTransform rectTransform;

    public void ShakeIt()
    {
        healthbarInitialPos = rectTransform.localPosition;
        InvokeRepeating("StartShake", 0f, 0.005f);
        Invoke("StopShake", shakeTime);
    }

    private void StartShake()
    {
        float shakeOffsetX = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        float shakeOffsetY = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        Vector3 intermediatePos = rectTransform.localPosition;
        intermediatePos.x += shakeOffsetX;
        intermediatePos.y += shakeOffsetY;
        rectTransform.localPosition = intermediatePos;
    }

    void StopShake()
    {
        CancelInvoke("StartShake");
        rectTransform.localPosition = healthbarInitialPos;
    }
}