using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class alerta : MonoBehaviour
{

    public GameObject textoAlerta; // aqui esta el texto que se debe activar cuando el jugador entre en el trigger
    public float tiempoInicial = 60f; // Tiempo en segundos (ej: 60 para 1 minuto)
    public TextMeshProUGUI textoTimer; // Arrastra aquí el TextMeshPro del timer
    public TextMeshProUGUI textoGameOver; // Arrastra aquí el texto "GAME OVER"
    public GameObject panelGameOver; //Panel de game over
    public GameObject objetoFinal; // este objeto es para activar el objetivo final
    private final juegoTerminado;
    private float tiempoRestante;
    private bool timerActivo = false;
    private bool gameOverActivado = false;
    private PlayerMovement playerMovement;

   


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textoAlerta.SetActive(true);// activa el texto de alerta
            timerActivo = true; // Inicia el temporizador
            textoTimer.gameObject.SetActive(true); // Muestra el temporizador

           
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textoAlerta.SetActive(false); // desactiva el texto de alerta
            gameObject.transform.position = new Vector3(0, -5, 0); // mueve el objeto fuera de la vista
            objetoFinal.SetActive(true); // activa el objetivo final

        }
    }


    void Start()
    {
        tiempoRestante = tiempoInicial;
        if (textoGameOver != null) textoGameOver.gameObject.SetActive(false); // Oculta game over inicial
        if (panelGameOver != null) panelGameOver.SetActive(false);
        
        // Formato inicial
        ActualizarDisplay();
    }


    void ActualizarDisplay()
    {
        int minutos = Mathf.FloorToInt(tiempoRestante / 60);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60);
        
        if (textoTimer != null)
        {
            textoTimer.text = string.Format("{0:00}:{1:00}", minutos, segundos);
            
            // Opcional: Cambiar color a rojo en últimos 10 seg
            if (tiempoRestante <= 10)
                textoTimer.color = Color.red;
        }
    }

    void GameOver()
    {
        if (gameOverActivado) return;
        gameOverActivado = true;
        timerActivo = false;

        // Muestra game over
        if (textoGameOver != null) textoGameOver.gameObject.SetActive(true);
        if (panelGameOver != null) panelGameOver.SetActive(true);
        playerMovement.enabled = false; // Deshabilita movimiento del jugador
    }

    void Update()
    {


        if (juegoTerminado == true){
            timerActivo = false;
            textoTimer.gameObject.SetActive(false); // Oculta el temporizador
            return;
        }

        if (!timerActivo || gameOverActivado) return;

        tiempoRestante -= Time.deltaTime;
        
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            GameOver();
        }
        
        ActualizarDisplay();
    }


}