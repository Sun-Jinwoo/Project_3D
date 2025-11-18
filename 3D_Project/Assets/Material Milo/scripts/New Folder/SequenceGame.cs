using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SequenceGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sequenceText; // Texto para mostrar la secuencia
    [SerializeField] private TextMeshProUGUI feedbackText; // Texto para mensajes de retroalimentación
    [SerializeField] private TextMeshProUGUI scoreText; // Texto para el puntaje
    [SerializeField] private int sequenceLength = 4; // Longitud inicial de la secuencia

    private List<string> sequence = new List<string>(); // Secuencia de teclas
    private List<string> playerInput = new List<string>(); // Entrada del jugador
    private string[] possibleKeys = { "W", "A", "S", "D" }; // Teclas posibles
    private int score = 0; // Puntaje del jugador

    void Start()
    {
        GenerateSequence();
        UpdateUI();
    }

    void Update()
    {
        // Detectar entrada del jugador
    if (Input.GetKeyDown(KeyCode.W)) { Debug.Log("W presionada"); CheckInput("W"); }
    if (Input.GetKeyDown(KeyCode.A)) { Debug.Log("A presionada"); CheckInput("A"); }
    if (Input.GetKeyDown(KeyCode.S)) { Debug.Log("S presionada"); CheckInput("S"); }
    if (Input.GetKeyDown(KeyCode.D)) { Debug.Log("D presionada"); CheckInput("D"); }
    }

    // Generar una nueva secuencia aleatoria
    void GenerateSequence()
    {
        sequence.Clear();
        playerInput.Clear();
        for (int i = 0; i < sequenceLength; i++)
        {
            int randomIndex = Random.Range(0, possibleKeys.Length);
            sequence.Add(possibleKeys[randomIndex]);
        }
    }

    // Verificar la entrada del jugador
    void CheckInput(string key)
    {
        playerInput.Add(key);
        int currentIndex = playerInput.Count - 1;

        // Verificar si la tecla ingresada es correcta
        if (playerInput[currentIndex] == sequence[currentIndex])
        {
            feedbackText.text = "¡Correcto!";
            if (playerInput.Count == sequence.Count)
            {
                // Secuencia completada correctamente
                score++;
                sequenceLength++; // Aumentar dificultad
                feedbackText.text = "¡Bien hecho! Nueva secuencia.";
                GenerateSequence();
            }
        }
        else
        {
            // Error en la secuencia
            feedbackText.text = "¡Error! Intenta de nuevo.";
            playerInput.Clear(); // Reiniciar entrada del jugador
        }

        UpdateUI();
    }

    // Actualizar la interfaz de usuario
    void UpdateUI()
    {
        sequenceText.text = "Secuencia: " + string.Join(" ", sequence);
        scoreText.text = "Puntaje: " + score;
    }
}