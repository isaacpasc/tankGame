using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gameTimer : MonoBehaviour
{

    [SerializeField] private float beginTime = 90;
    [SerializeField] private TextMeshProUGUI label;
    private float timeValue;
    private float timeComplete;
    private bool stop;
    
    void Start()
    {
        timeValue = beginTime;
        stop = false;
    }

    
    void Update()
    {
        if (!stop)
        {
            if (timeValue > 15)
            {
                timeValue -= Time.deltaTime;
                displayTime(timeValue);
            }
            else if (timeValue <= 15 && timeValue > 0)
            {
                label.color = Color.red;
                timeValue -= Time.deltaTime;
                displayTime(timeValue);
            } else if (timeValue <= 0)
            {
                timeValue = 0;
            }
        }
    }

    void displayTime(float timeToDisplay)
    {
        if (timeToDisplay <= 0)
        {
            label.text = "00:00";
        } else
        {
            float min = Mathf.FloorToInt(timeToDisplay / 60);
            float sec = Mathf.FloorToInt(timeToDisplay % 60);

            label.text = string.Format("{0:00}:{1:00}", min, sec);
        }
    }

    public void stopTime()
    {
        stop = true;
        timeComplete = beginTime - timeValue;
    }
}
