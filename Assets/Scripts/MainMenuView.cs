using System;
using TMPro;
using UnityEngine;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsHighscore;
    [SerializeField] private TMP_Text kmhHighscore;

    private void Update()
    {
        int points = PlayerPrefs.GetInt("Points");
        int speed = PlayerPrefs.GetInt("Kmh");
        pointsHighscore.text = points + " Points";
        kmhHighscore.text = speed + " Kmh";
    }
}
