using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onDeath;
    
    private void Awake()
    {
        CharacterMoveBehaviour.onDeath += () =>
        {
            onDeath?.Invoke();
            StartCoroutine(StopTime());
        };
    }
    
    private void Start()
    {
        Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        CharacterMoveBehaviour.onDeath -= () =>
        {
            onDeath?.Invoke();
            StartCoroutine(StopTime());
        };
    }

    public void StartTime()
    {
        Time.timeScale = 1f;
    }

    private IEnumerator StopTime()
    {
        yield return null;
        Time.timeScale = 0f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
