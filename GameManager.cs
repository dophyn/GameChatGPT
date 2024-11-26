using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartButton;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        if (stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            Reposition();
            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            // 게임 클리어
            Time.timeScale = 0;
            Debug.Log("Game Clear");
            
            Text btnText = RestartButton.GetComponentInChildren<Text>();
            btnText.text = "GAME CLEAR!";
            RestartButton.SetActive(true);
        }
        

        totalPoint += stagePoint;
        stagePoint = 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (health > 1)
                Reposition();
            
            HealthDown();
        }
    }

    void Reposition()
    {
        player.transform.position = new Vector3(-9.27f, 2f, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            player.OnDead();
            Debug.Log("Player Die");
            RestartButton.SetActive(true);
        }
    }
}
