using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtual_Camera;
    private CinemachineBasicMultiChannelPerlin m_ChannelPerlin;
    [Header("Shake Settings")]
    [SerializeField] private float shakeIntencity;
    [SerializeField] private float shakeTime;
    [SerializeField] private float timer;
    private void Awake()
    {
        virtual_Camera = GetComponent<CinemachineVirtualCamera>();

    }
    private void Start()
    {
        StopShake();
    }
    public void ShakeCamera()
    {
        CinemachineBasicMultiChannelPerlin m_ChannelPerlin  = virtual_Camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_ChannelPerlin.m_AmplitudeGain = shakeIntencity;
        timer = shakeTime;
    }
    public void StopShake()
    {
        CinemachineBasicMultiChannelPerlin m_ChannelPerlin = virtual_Camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_ChannelPerlin.m_AmplitudeGain = 0;
        timer = 0;
    }
    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0) 
            {
                StopShake();
            
            }
        
        }
    }





}
