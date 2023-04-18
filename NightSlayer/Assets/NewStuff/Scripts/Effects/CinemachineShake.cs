using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }
    
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
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
        if(cinemachineVirtualCamera.m_Follow == null)
        {
            StartCoroutine(AssignVariables());
        }
    }

    IEnumerator AssignVariables()
    {
        yield return new WaitForSeconds(0.5f);
        cinemachineVirtualCamera.m_Follow = GameObject.Find("HeroKnight(Clone)").transform;
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