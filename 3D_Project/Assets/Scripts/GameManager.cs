using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI TimerTMP;
    public GameObject GameOverPanel;
    public GameObject VictoryPanel;

    public bool GameOver { get; set; }
    public bool Vault { get; set; }

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        GameOverPanel.SetActive(false);
        VictoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (!GameOver)
        {
            TimerTMP.text = string.Format("Time: {0:D1}:{1:D2}", Mathf.FloorToInt(Time.time / 60f), Mathf.FloorToInt(Time.time % 60f));
        }
    }
}
