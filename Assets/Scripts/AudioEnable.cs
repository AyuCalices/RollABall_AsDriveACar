using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnable : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    private void OnEnable()
    {
        audioSource.Play();
    }
    
    private void OnDisable()
    {
        audioSource.Stop();
    }
}
