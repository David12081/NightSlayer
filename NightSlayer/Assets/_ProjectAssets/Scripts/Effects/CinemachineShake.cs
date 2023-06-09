using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }
    
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] Camera cam;
    public CinemachineConfiner cinemachineConfiner;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startIntensity;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cam.enabled = false;
        if(cinemachineVirtualCamera.m_Follow == null)
        {
            StartCoroutine(AssignVariables());
        }
        else
        {
            cam.enabled = true;
        }
    }

    IEnumerator AssignVariables()
    {
        yield return new WaitForSeconds(4f);
        cinemachineVirtualCamera.m_Follow = GameObject.Find("Player(Clone)").transform;
        cam.enabled = true;
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        startIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                    cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal)));
        }
    }
}