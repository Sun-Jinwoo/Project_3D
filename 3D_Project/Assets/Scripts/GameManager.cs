using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // --- DATOS DE JUEGO ---
    public int dineroTotal = 0;
    public int itemsRequeridos = 3;
    public int itemsActuales = 0;

    // --- TIEMPO ---
    public float tiempoInicio;
    public float tiempoFinal;

    public TextMeshProUGUI TimerTMP;

    // --- PANEL DE GAME OVER ---
    public GameObject panelGameOver;

    // --- ESTADOS ---
    public bool GameOver { get; set; }
    public bool Vault { get; set; }

    // --- TEMPORIZADOR DE ROBO ---
    float tiempoRestante = -1f;
    bool temporizadorActivo = false;

    private void Awake()
    {
        Instance = this;

        // Asegurar que el panel esté apagado al iniciar
        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    void Start()
    {
        tiempoInicio = Time.time;
    }

    void Update()
    {
        // TEMPORIZADOR DEL HUD NORMAL
        if (!GameOver)
        {
            float t = Time.time - tiempoInicio;
            TimerTMP.text = string.Format("Time: {0:D2}:{1:D2}",
                Mathf.FloorToInt(t / 60f),
                Mathf.FloorToInt(t % 60f));
        }

        // TEMPORIZADOR DE ROBO (al entrar a la casa)
        if (temporizadorActivo && !GameOver)
        {
            tiempoRestante -= Time.deltaTime;
            TimerTMP.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante);

            if (tiempoRestante <= 0)
            {
                MostrarGameOver();
            }
        }
    }

    // --- ACTIVAR EL TEMPORIZADOR DE ROBO ---
    public void IniciarTemporizadorCasa(float tiempo)
    {
        tiempoRestante = tiempo;
        temporizadorActivo = true;
    }

    // --- SUMAR DINERO AL RECOGER OBJETOS ---
    public void SumarDinero(int cant)
    {
        dineroTotal += cant;
        itemsActuales++;
    }

    // --- REGISTRAR EL TIEMPO AL ESCAPAR ---
    public void RegistrarTiempoFinal()
    {
        tiempoFinal = Time.time - tiempoInicio;
    }

    // --- MOSTRAR GAME OVER ---
    public void MostrarGameOver()
    {
        GameOver = true;
        Time.timeScale = 0f;
        panelGameOver.SetActive(true);
    }

    // --- BOTÓN: REINTENTAR ---
    public void Reintentar()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // --- BOTÓN: SALIR ---
    public void Salir()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("00_Menu");
    }
}
