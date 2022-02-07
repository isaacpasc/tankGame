using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{

    [SerializeField] private int enemiesTotal;
    [SerializeField] private TextMeshProUGUI label;
    private int enemiesLeft;
    
    void Start()
    {
        enemiesLeft = enemiesTotal;
        label.text = enemiesLeft + " / " + enemiesTotal;
    }

    int getEnemiesLeft()
    {
        return enemiesLeft;
    }

    public void enemyDies()
    {
        enemiesLeft = enemiesLeft - 1;
        if (enemiesLeft == 0)
        {
            // stop timer
            GameObject go = GameObject.Find("Timer");
            gameTimer timer = (gameTimer)go.GetComponent(typeof(gameTimer));
            timer.stopTime();
        } else if (enemiesLeft < 0)
        {
            enemiesLeft = 0;
        }
        label.text = enemiesLeft + " / " + enemiesTotal;
    }
}
