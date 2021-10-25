using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : Singleton<CinemachineShake>
{
    private CinemachineFreeLook freeLookCam;
    
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    private void Start()
    {
        freeLookCam = this.GetComponent<CinemachineFreeLook>();
        
        for (int i = 0; i < 3; i++)
        {
            var multiChannelPerlin =
                        freeLookCam.GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            multiChannelPerlin.m_AmplitudeGain = 0f;
        }
    }

    private void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f) 
            {
                for (int i = 0; i < 3; i++)
                {
                    var multiChannelPerlin =
                        freeLookCam.GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    
                    multiChannelPerlin.m_AmplitudeGain = 
                        Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
                }
            }
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        for(int i = 0; i < 3; i++)
        {
            var multiChannelPerlin =
                        freeLookCam.GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            multiChannelPerlin.m_AmplitudeGain = intensity;
        }

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }
}
