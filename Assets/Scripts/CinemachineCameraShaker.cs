using UnityEngine;
using Cinemachine;
using DG.Tweening;  // For other DoTween animations you may want

public class CinemachineCameraShaker : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;  // Reference to the Cinemachine Virtual Camera
    public float shakeDuration = 1f;  // Duration of the camera shake
    public float shakeAmplitude = 2f; // Amplitude of the shake (higher = more intense shake)
    public float shakeFrequency = 2f; // Frequency of the shake (higher = faster shake)

    private CinemachineBasicMultiChannelPerlin camNoise;  // Reference to the noise component on the virtual camera

    void Start()
    {
        if (virtualCamera != null)
        {
            // Get the 'CinemachineBasicMultiChannelPerlin' component from the Cinemachine Virtual Camera
            camNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            // Ensure the noise component is found
            if (camNoise == null)
            {
                Debug.LogError("CinemachineBasicMultiChannelPerlin component not found on the virtual camera!");
            }
        }
    }

    // Public method to start the camera shake
    public void ShakeCamera()
    {
        if (camNoise != null)
        {
            // Set the amplitude and frequency to start shaking
            camNoise.m_AmplitudeGain = shakeAmplitude;
            camNoise.m_FrequencyGain = shakeFrequency;

            // Stop the shake after the given duration
            Invoke("StopShake", shakeDuration);
        }
        else
        {
            Debug.LogWarning("Camera noise is not set. Did you forget to set the virtual camera?");
        }
    }

    // Method to stop the camera shake (sets amplitude and frequency back to 0)
    private void StopShake()
    {
        if (camNoise != null)
        {
            camNoise.m_AmplitudeGain = 0f;
            camNoise.m_FrequencyGain = 0f;
        }
    }
}
