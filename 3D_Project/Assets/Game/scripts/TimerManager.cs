using UnityEngine;
using TMPro;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    public float tiempo = 30f;
    public TextMeshProUGUI textoTiempo;
    public GameObject panelGameOver;

    bool tiempoActivo = false;
    bool gameOver = false;

    public void ActivarTemporizador()
    {
        if (!tiempoActivo)
            tiempoActivo = true;
    }

    void Update()
    {
        if (!tiempoActivo || gameOver) return;

        tiempo -= Time.deltaTime;
        textoTiempo.text = "Tiempo restante: " + Mathf.CeilToInt(tiempo) + "s";

        if (tiempo <= 0)
        {
            Perder();
        }
    }

    void Perder()
{
    gameOver = true;
    tiempoActivo = false;

    textoTiempo.text = "¡Te atraparon!";

    panelGameOver.SetActive(true);

    
    StartCoroutine(PausarDespuesDeMostrar());
}

IEnumerator PausarDespuesDeMostrar()
{
    yield return null; // espera un frame
    Time.timeScale = 0f;
}


    // --- BOTONES ---
    public void Reintentar()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("00_Menu");
    }
}
