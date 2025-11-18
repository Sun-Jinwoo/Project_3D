//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using TMPro;
//using System.Collections;

//public class GameManager : MonoBehaviour
//{
//    [Header("Configuración")]
//    public int topePuntos = 10;
//    public float tiempoAlertaPanel = 5f;
//    public float tiempoEscape = 60f;

//    [Header("UI")]
//    public GameObject panelAlerta;
//    public TextMeshProUGUI textoAlerta;
//    public GameObject panelTimer;
//    public TextMeshProUGUI textoTimer;
//    public GameObject panelVictoria;
//    public GameObject panelGameOver;

//    [Header("Zona Segura (CRÍTICO: Asignar aquí)")]
//    public GameObject zonaSegura;  // ← ¡ASIGNAR EN INSPECTOR!

//    public enum EstadoJuego { Normal, AlertaTimer, CazaFinal }
//    public static GameManager Instance;
//    public EstadoJuego estadoActual = EstadoJuego.Normal;
//    public int puntosActuales = 0;
//    private float tiempoRestante;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            return;
//        }
//        Destroy(gameObject);
//    }

//    void Start()
//    {
//        OcultarTodosPaneles();
//        if (zonaSegura != null)
//            zonaSegura.SetActive(false);  // ← ZONA DESACTIVADA AL INICIO
//        else
//            Debug.LogError("GameManager: ¡ASIGNAR Zona Segura en Inspector!");
//    }

//    void Update()
//    {
//        if (estadoActual == EstadoJuego.AlertaTimer)
//        {
//            tiempoRestante -= Time.deltaTime;
//            textoTimer.text = "Tiempo para escapar: " + Mathf.Ceil(tiempoRestante) + "s";

//            if (tiempoRestante <= 0)
//                IniciarCazaFinal();
//        }
//    }

//    public void SumarPuntos(int cantidad)
//    {
//        if (estadoActual != EstadoJuego.Normal) return;

//        puntosActuales += cantidad;
//        Debug.Log("Puntos: " + puntosActuales + "/" + topePuntos);

//        if (puntosActuales >= topePuntos)
//            IniciarAlerta();
//    }

//    private void IniciarAlerta()
//    {
//        estadoActual = EstadoJuego.AlertaTimer;
//        tiempoRestante = tiempoEscape;

//        StartCoroutine(MostrarPanelAlerta());
//    }

//    private IEnumerator MostrarPanelAlerta()
//    {
//        // ← ¡ACTIVAR ZONA SEGURA AQUÍ!
//        if (zonaSegura != null)
//        {
//            zonaSegura.SetActive(true);
//            Debug.Log("✅ Zona Segura ACTIVADA - ¡Encuéntrala rápido!");
//        }

//        // Mostrar panel alerta 5s
//        panelAlerta.SetActive(true);
//        textoAlerta.text = "¡Van a venir todos los guardias!\nTienes 1 minuto para escapar.";
//        yield return new WaitForSecondsRealtime(tiempoAlertaPanel);
//        panelAlerta.SetActive(false);

//        // Mostrar timer
//        panelTimer.SetActive(true);
//    }

//    private void IniciarCazaFinal()
//    {
//        estadoActual = EstadoJuego.CazaFinal;
//        panelTimer.SetActive(false);

//        SeguridadNPC[] todosNPCs = FindObjectsOfType<SeguridadNPC>();
//        foreach (var npc in todosNPCs)
//            npc.IniciarCazaFinal();

//        Debug.Log("🔥 ¡CAZA FINAL INICIADA! Escapa YA a la zona segura.");
//    }

//    public void Victoria()
//    {
//        panelVictoria.SetActive(true);
//        StartCoroutine(ReiniciarEscena(3f));
//    }

//    public void Derrota()
//    {
//        panelGameOver.SetActive(true);
//        StartCoroutine(ReiniciarEscena(3f));
//    }

//    private IEnumerator ReiniciarEscena(float delay)
//    {
//        yield return new WaitForSecondsRealtime(delay);
//        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//    }

//    private void OcultarTodosPaneles()
//    {
//        panelAlerta?.SetActive(false);
//        panelTimer?.SetActive(false);
//        panelVictoria?.SetActive(false);
//        panelGameOver?.SetActive(false);
//    }
//}