using UnityEngine;
using UnityEngine.UI;
using TMPro; // Si usas TextMeshPro
using System.Collections;

public class LockpickingGame : MonoBehaviour
{
    public Button[] pins; // Array de los 5 botones de los pines
    public TextMeshProUGUI statusText; // Texto de estado (usa Text si no usas TextMeshPro)
    public Button resetButton; // Botón de reinicio

    private bool[] pinStates; // Estado de los pines (false = abajo, true = arriba)
    private int[] correctOrder = { 2, 0, 4, 1, 3 }; // Orden correcto (0-based: pin 3, 1, 5, 2, 4)
    private int currentStep; // Paso actual en el orden correcto
    public bool gameWon; // Indica si el juego está ganado
    public GameObject puerta; // puerta que se va a abrir

    void Start()
    {
        pinStates = new bool[5];
        currentStep = 0;
        gameWon = false;

        // Asignar eventos a los botones de los pines
        for (int i = 0; i < pins.Length; i++)
        {
            int pinIndex = i; // Capturar el índice para el evento
            pins[i].onClick.AddListener(() => OnPinClick(pinIndex));
        }

        // Asignar evento al botón de reinicio
        resetButton.onClick.AddListener(ResetGame);

        UpdatePins();
    }

    void OnPinClick(int pinIndex)
    {
        if (gameWon || pinStates[pinIndex]) return; // Ignorar si el juego está ganado o el pin ya está arriba

        if (pinIndex == correctOrder[currentStep])
        {
            // Pin correcto
            pinStates[pinIndex] = true;
            currentStep++;
            if (currentStep == pins.Length)
            {
                gameWon = true;
                statusText.text = "¡Cerradura desbloqueada! ¡Ganaste!";
                statusText.color = Color.green;
                puerta.transform.Rotate(0, 90, 0); // Abrir la puerta girándola 90 grados

            }
            UpdatePins();
        }
        else
        {
            // Pin incorrecto, reiniciar
            pinStates = new bool[5];
            currentStep = 0;
            statusText.text = "¡Orden incorrecto! Intenta de nuevo.";
            statusText.color = Color.red;
            UpdatePins();

            // Restaurar mensaje por defecto después de 1 segundo
            StartCoroutine(ResetStatusText());
        }
    }

    void UpdatePins()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            // Cambiar color según el estado
            var colors = pins[i].colors;
            colors.normalColor = pinStates[i] ? new Color32(76, 175, 80, 255) : new Color32(51, 51, 51, 255); // Verde o gris
            pins[i].colors = colors;
        }
    }

    void ResetGame()
    {
        pinStates = new bool[5];
        currentStep = 0;
        gameWon = false;
        statusText.text = "Empuja los pines en el orden correcto";
        statusText.color = Color.black;
        UpdatePins();
    }

    IEnumerator ResetStatusText()
    {
        yield return new WaitForSeconds(1f);
        if (!gameWon)
        {
            statusText.text = "Empuja los pines en el orden correcto";
            statusText.color = Color.black;
        }
    }
}