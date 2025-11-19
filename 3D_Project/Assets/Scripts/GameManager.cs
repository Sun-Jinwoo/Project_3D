using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int dineroTotal = 0;
    public int itemsRequeridos = 3;
    public int itemsActuales = 0;

    public float tiempoInicio;
    public float tiempoFinal;

    public TextMeshProUGUI TimerTMP;
    public GameObject GameOverPanel;

    public bool GameOver { get; set; }
    public bool Vault { get; set; }

    private void Awake()
    {
        Instance = this;
        GameOverPanel.SetActive(false);
    }

    void Start()
    {
        tiempoInicio = Time.time;
    }

    void Update()
    {
        if (!GameOver)
        {
            float t = Time.time - tiempoInicio;
            TimerTMP.text = string.Format("Time: {0:D2}:{1:D2}",
                Mathf.FloorToInt(t / 60f),
                Mathf.FloorToInt(t % 60f));
        }
    }

    public void SumarDinero(int cant)
    {
        dineroTotal += cant;
        itemsActuales++;
    }

    public void RegistrarTiempoFinal()
    {
        tiempoFinal = Time.time - tiempoInicio;
    }
}
