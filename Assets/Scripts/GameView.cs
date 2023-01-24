using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private TMP_Text pointText;
    [SerializeField] private TMP_Text speedText;

    [SerializeField] private CharacterMoveBehaviour characterMoveBehaviour;

    private float _currentPoints;

    private void Update()
    {
        pointText.text = "Points: " + Mathf.Floor(characterMoveBehaviour.transform.position.z).ToString(CultureInfo.CurrentCulture);
        speedText.text = Mathf.Floor(characterMoveBehaviour.Kmh).ToString(CultureInfo.CurrentCulture) + " Kmh";
    }
}
