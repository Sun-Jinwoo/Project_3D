using UnityEngine;
using TMPro; // Si usas TextMeshPro, sino usa UnityEngine.UI para Text

public class InteractTrigger1 : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject promptText; // Asigna el Text de "Presiona J..."
    public GameObject objetoAActivar; // El GameObject que se activa/desactiva con J
    public GameObject player; // Arrastra el GameObject del personaje aquí

    private bool jugadorDentro = false;
    private PlayerMovement playerMovement; // Referencia al script de movimiento del player
    private bool accionActivada = false; // Para controlar el estado del toggle
   public GlassBreakMinigame GlassBreakMinigame; 

    void Start()
    {
        // Inicializar texto desactivado
        if (promptText != null)
            promptText.SetActive(false);

        // Obtener script de movimiento del player
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el player tenga tag "Player"
        {
            jugadorDentro = true;
            if (promptText != null)
                promptText.SetActive(true); // Muestra el texto
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            if (promptText != null)
                promptText.SetActive(false); // Oculta el texto
        }
    }

    void Update()
    {
        if (GlassBreakMinigame.isGameOver== true && accionActivada== true)
        {
            playerMovement.enabled = true;

            objetoAActivar.SetActive(false);
            gameObject.SetActive(false);
            promptText.SetActive(false);

         }







        

        if (jugadorDentro && Input.GetKeyDown(KeyCode.J))
        {
            accionActivada = !accionActivada; // Toggle el estado

            if (objetoAActivar != null)
            {
                objetoAActivar.SetActive(accionActivada); // Activa o desactiva
            }

            // Deshabilita/Habilita movimiento del player
            if (playerMovement != null)
            {
                playerMovement.enabled = !accionActivada; 
                // Cuando accionActivada = true → movimiento = false
                // Cuando accionActivada = false → movimiento = true
            }

            // Opcional: Cambiar texto del prompt según el estado
            if (promptText != null)
            {
                TextMeshProUGUI textComponent = promptText.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    if (accionActivada)
                        textComponent.text = "Presiona J para desactivar";
                    else
                        textComponent.text = "Presiona J para activar";
                }
            }
        }
    }
}