//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using TMPro; // Usa TextMeshPro si prefieres, si no, cambia a UnityEngine.UI.Text

//public class LevelManager : MonoBehaviour
//{
//    [Header("=== Tutorial ===")]
//    public GameObject[] tutorialPanels;     // Arreglo de paneles (ordenados)
//    public Button nextButton;               // Botón "Siguiente" o "Entendido"
//    private int currentPanelIndex = 0;

//    [Header("=== Progreso Misión ===")]
//    public int missionGoal = 50;            // Meta de la misión
//    private int currentProgress = 0;
//    public TextMeshProUGUI progressText;    // Ej: "Objetos: 23 / 50"

//    [Header("=== Portal y Tiempo de Salida ===")]
//    public GameObject portalObject;         // El GameObject del portal (desactivado al inicio)
//    public GameObject panelExitTimer;       // Panel que muestra el cronómetro de salida
//    public TextMeshProUGUI exitTimerText;
//    public float timeToExitAfterGoal = 15f; // Segundos para salir una vez alcanzada la meta
//    private float exitTimer;
//    private bool missionCompleted = false;

//    [Header("=== Tiempo Total del Nivel ===")]
//    private float levelStartTime;
//    public GameObject panelVictory;
//    public TextMeshProUGUI totalTimeText;
//    public Button continueButton;

//    [Header("=== Siguiente Nivel ===")]
//    public string nextSceneName = "Nivel_2"; // Nombre de la siguiente escena

//    void Start()
//    {
//        // Iniciar tutorial
//        levelStartTime = Time.time;
//        ShowTutorialPanel(0);
//        portalObject.SetActive(false);
//        panelExitTimer.SetActive(false);
//        panelVictory.SetActive(false);

//        // Configurar botón siguiente
//        if (nextButton != null)
//            nextButton.onClick.AddListener(NextTutorialPanel);

//        if (continueButton != null)
//            continueButton.onClick.AddListener(LoadNextLevel);
//    }

//    void Update()
//    {
//        // Cronómetro de salida después de completar la misión
//        if (missionCompleted && portalObject.activeSelf)
//        {
//            exitTimer -= Time.deltaTime;
//            exitTimerText.text = $"¡Sal por el portal!\n{exitTimer:F1} segundos";

//            if (exitTimer <= 0f)
//            {
//                // Tiempo agotado → derrota
//                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//            }
//        }
//    }

//    // Llamar este método desde otros scripts (enemigos, recolectables, etc.)
//    public void AddProgress(int amount = 1)
//    {
//        if (missionCompleted) return;

//        currentProgress += amount;
//        progressText.text = $"Progreso: {currentProgress} / {missionGoal}";

//        if (currentProgress >= missionGoal)
//        {
//            CompleteMission();
//        }
//    }

//    private void CompleteMission()
//    {
//        missionCompleted = true;
//        portalObject.SetActive(true);
//        panelExitTimer.SetActive(true);
//        exitTimer = timeToExitAfterGoal;
//        exitTimerText.text = $"¡Sal por el portal!\n{exitTimer:F1} segundos";
//    }

//    // ---------- Tutorial ----------
//    private void ShowTutorialPanel(int index)
//    {
//        for (int i = 0; i < tutorialPanels.Length; i++)
//            tutorialPanels[i].SetActive(i == index);

//        currentPanelIndex = index;

//        // Si es el último panel, cambiar texto del botón a "Comenzar" o similar (opcional)
//        if (index == tutorialPanels.Length - 1)
//        {
//            TextMeshProUGUI btnText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
//            if (btnText) btnText.text = "¡Comenzar!";
//        }
//    }

//    public void NextTutorialPanel()
//    {
//        if (currentPanelIndex < tutorialPanels.Length - 1)
//        {
//            ShowTutorialPanel(currentPanelIndex + 1);
//        }
//        else
//        {
//            // Fin del tutorial → iniciar juego
//            foreach (GameObject panel in tutorialPanels)
//                panel.SetActive(false);

//            // Aquí puedes activar controles del jugador, etc.
//        }
//    }

//    // ---------- Victoria (llamar desde el trigger del portal) ----------
//    public void OnPlayerEnterPortal()
//    {
//        if (!missionCompleted) return;

//        float totalTime = Time.time - levelStartTime;
//        panelVictory.SetActive(true);
//        panelExitTimer.SetActive(false);
//        portalObject.SetActive(false);

//        totalTimeText.text = $"¡Nivel completado!\nTiempo total: {totalTime:F1} segundos";

//        Time.timeScale = 0f; // Pausar el juego
//    }

//    public void LoadNextLevel()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene(nextSceneName);
//    }

//    // Opcional: método para reiniciar manualmente
//    public void RestartLevel()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Cambia a UnityEngine.UI.Text si no usas TextMeshPro

public class LevelManager : MonoBehaviour
{
    [Header("=== Tutorial ===")]
    public GameObject[] tutorialPanels;
    public Button nextButton;
    private int currentPanelIndex = 0;

    [Header("=== Progreso Misión ===")]
    public int missionGoal = 50;                    // Meta (ej. recolectar 50 objetos)
    private int currentProgress = 0;
    public TextMeshProUGUI progressText;            // Texto "Progreso: 23 / 50"

    [Header("=== Portal y Tiempo de Salida ===")]
    public GameObject portalObject;                 // Portal (desactivado al inicio)
    public GameObject panelExitTimer;               // Panel con el cronómetro de salida
    public TextMeshProUGUI exitTimerText;
    public float timeToExitAfterGoal = 15f;         // Segundos para salir una vez alcanzada la meta
    private float exitTimer;
    private bool missionCompleted = false;

    [Header("=== Panel Victoria y Cambio Automático ===")]
    private float levelStartTime;
    public GameObject panelVictory;                 // Panel que muestra el tiempo total
    public TextMeshProUGUI totalTimeText;
    [Tooltip("Segundos que se muestra el panel de victoria antes de pasar al siguiente nivel")]
    public float victoryDisplayTime = 5f;           // Tiempo que dura el panel de victoria visible

    [Header("=== Siguiente Nivel ===")]
    public string nextSceneName = "Nivel_2";        // Nombre exacto de la siguiente escena

    private bool victoryShown = false;

    void Start()
    {
        levelStartTime = Time.time;

        // Tutorial activo al inicio
        ShowTutorialPanel(0);

        portalObject.SetActive(false);
        panelExitTimer.SetActive(false);
        panelVictory.SetActive(false);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextTutorialPanel);
    }

    void Update()
    {
        // Cronómetro de salida después de completar la misión
        if (missionCompleted && portalObject.activeSelf)
        {
            exitTimer -= Time.deltaTime;
            exitTimerText.text = $"¡Entra en el portal!\n{exitTimer:F1} s";

            if (exitTimer <= 0f)
            {
                // Pierde y reinicia el nivel
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    // Llamar desde los objetos recolectables, enemigos, etc.
    public void AddProgress(int amount = 1)
    {
        if (missionCompleted) return;

        currentProgress += amount;
        progressText.text = $"Progreso: {currentProgress} / {missionGoal}";

        if (currentProgress >= missionGoal)
            CompleteMission();
    }

    private void CompleteMission()
    {
        missionCompleted = true;
        portalObject.SetActive(true);
        panelExitTimer.SetActive(true);
        exitTimer = timeToExitAfterGoal;
        exitTimerText.text = $"¡Entra en el portal!\n{exitTimer:F1} s";
    }

    // ---------- Tutorial ----------
    private void ShowTutorialPanel(int index)
    {
        for (int i = 0; i < tutorialPanels.Length; i++)
            tutorialPanels[i].SetActive(i == index);

        currentPanelIndex = index;

        if (index == tutorialPanels.Length - 1)
        {
            TextMeshProUGUI btnText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText) btnText.text = "¡Comenzar!";
        }
    }

    public void NextTutorialPanel()
    {
        if (currentPanelIndex < tutorialPanels.Length - 1)
            ShowTutorialPanel(currentPanelIndex + 1);
        else
        {
            foreach (GameObject p in tutorialPanels)
                p.SetActive(false);
            // Aquí ya puede jugar
        }
    }

    // ---------- Llamado desde el Trigger del Portal ----------
    public void OnPlayerEnterPortal()
    {
        if (!missionCompleted || victoryShown) return;

        victoryShown = true;

        // Mostrar panel de victoria
        float totalTime = Time.time - levelStartTime;
        panelVictory.SetActive(true);
        panelExitTimer.SetActive(false);
        totalTimeText.text = $"¡Nivel completado!\nTiempo total: {totalTime:F1} segundos";

        Time.timeScale = 0f; // Pausar el juego mientras se muestra la victoria

        // Cambio automático después de X segundos
        Invoke(nameof(LoadNextLevel), victoryDisplayTime);
    }

    private void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    // Opcional: reiniciar manualmente (para pruebas)
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}