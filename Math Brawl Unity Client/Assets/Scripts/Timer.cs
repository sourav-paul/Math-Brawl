using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float currentLevelTimeLeft = 90;
    public bool timerIsRunning = false;
    public TMP_Text timeText;
    private void Start()
    {
        timeText = GetComponent<TMP_Text>();
        // Starts the timer automatically
        timerIsRunning = false;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (currentLevelTimeLeft > 0)
            {
                currentLevelTimeLeft -= Time.deltaTime;
                DisplayTime(currentLevelTimeLeft);
            }
            else
            {
                StartCoroutine(FindObjectOfType<Connection>().AskForNextAfterCorrect());
                currentLevelTimeLeft = 0;
                timerIsRunning = false;
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
