using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class SeguridadNPC : MonoBehaviour
{
    /* -------------------------------------------------
       CONFIGURACIÓN EN EL INSPECTOR
       ------------------------------------------------- */
    [Header("Velocidades")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 5f;

    [Header("Patrulla")]
    public Transform[] waypoints;                     // Asigna en el Inspector

    [Header("Detección y Atrapar")]
    public float radioDeteccion = 8f;                 // Radio propio del NPC
    public float distanciaAtrapar = 2f;               // Distancia para Game Over
    public float radioSampleNavMesh = 5f;             // Radio para validar destinos

    [Header("Game Over (opcional)")]
    public GameObject panelGameOver;                  // Panel de derrota (si no hay GameManager)

    [Header("Debug")]
    public bool mostrarLogs = true;                   // Activa / desactiva logs

    /* -------------------------------------------------
       VARIABLES PRIVADAS
       ------------------------------------------------- */
    private NavMeshAgent agent;
    private Transform jugador;
    private int indiceWaypointActual;

    public enum EstadoNPC { Patrulla, Persecucion, CazaFinal }
    private EstadoNPC estadoActual = EstadoNPC.Patrulla;

    /* -------------------------------------------------
       INICIALIZACIÓN
       ------------------------------------------------- */
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (jugador == null) LogError("No se encontró Player con tag \"Player\"");
        if (agent == null) LogError("NavMeshAgent requerido");
    }

    void Start()
    {
        // ---- Reposicionar en NavMesh si es necesario ----
        if (!agent.isOnNavMesh)
        {
            LogWarning("NPC fuera de NavMesh. Intentando reposicionar...");
            if (!ReposicionarEnNavMesh())
            {
                LogError("No se pudo colocar en NavMesh. Desactivando NPC.");
                enabled = false;
                return;
            }
        }

        agent.speed = velocidadPatrulla;
        agent.stoppingDistance = 0.5f;

        // ---- Waypoints ----
        if (waypoints == null || waypoints.Length == 0)
        {
            LogWarning("No hay waypoints asignados. El NPC permanecerá quieto.");
        }
        else
        {
            indiceWaypointActual = 0;
            SetValidDestination(waypoints[indiceWaypointActual].position, "Waypoint inicial");
        }

        Log("NPC listo.");
    }

    /* -------------------------------------------------
       UPDATE
       ------------------------------------------------- */
    void Update()
    {
        if (!agent || !agent.enabled || !agent.isOnNavMesh || !jugador) return;

        switch (estadoActual)
        {
            case EstadoNPC.Patrulla: ActualizarPatrulla(); break;
            case EstadoNPC.Persecucion: ActualizarPersecucion(); break;
            case EstadoNPC.CazaFinal: ActualizarCazaFinal(); break;
        }
    }

    /* -------------------------------------------------
       ESTADOS
       ------------------------------------------------- */
    void ActualizarPatrulla()
    {
        if (waypoints.Length == 0) return;

        // Llegó al waypoint
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance + 0.1f)
            IrAlSiguienteWaypoint();

        DetectarJugadorCercano();
    }

    void ActualizarPersecucion()
    {
        float dist = Vector3.Distance(transform.position, jugador.position);

        // Salir del radio → volver a patrulla
        if (dist > radioDeteccion)
        {
            DetenerPersecucion();
            return;
        }

        SetValidDestination(jugador.position, "Jugador (persecución)");

        if (dist <= distanciaAtrapar)
            ActivarGameOver();
    }

    void ActualizarCazaFinal()
    {
        // Persecución permanente, sin límite de radio
        SetValidDestination(jugador.position, "Jugador (Caza Final)");

        if (Vector3.Distance(transform.position, jugador.position) <= distanciaAtrapar)
        {
            // Especifica la clase completa para evitar ambigüedad
            if (global::GameManager.Instance != null)
                global::GameManager.Instance.Derrota();
            else
                ActivarGameOver(); // Usa GameManager si existe
        }
    }

    /* -------------------------------------------------
       DETECCIÓN PROPIA
       ------------------------------------------------- */
    void DetectarJugadorCercano()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radioDeteccion);
        foreach (Collider c in hits)
        {
            if (c.CompareTag("Player"))
            {
                IniciarPersecucion();
                Log("Detecté jugador en mi radio de patrulla.");
                return;
            }
        }
    }

    /* -------------------------------------------------
       MÉTODOS PÚBLICOS (cámara / GameManager)
       ------------------------------------------------- */
    public void Alertar()               // Llamado por la cámara
    {
        IniciarPersecucion();
        Log("Alerta de cámara recibida → Persecución.");
    }

    public void DetenerAlerta()         // Llamado cuando la cámara pierde al jugador
    {
        if (estadoActual == EstadoNPC.CazaFinal) return; // No se detiene en CazaFinal
        DetenerPersecucion();
    }

    public void IniciarCazaFinal()      // Llamado por GameManager al terminar el timer
    {
        if (estadoActual == EstadoNPC.CazaFinal) return;

        estadoActual = EstadoNPC.CazaFinal;
        agent.speed = velocidadPersecucion * 1.2f; // Un poco más rápido
        agent.ResetPath();
        Log("¡CAZA FINAL INICIADA! Persecución sin límite.");
    }

    /* -------------------------------------------------
       TRANSICIONES INTERNAS
       ------------------------------------------------- */
    private void IniciarPersecucion()
    {
        if (estadoActual == EstadoNPC.Persecucion || estadoActual == EstadoNPC.CazaFinal) return;

        estadoActual = EstadoNPC.Persecucion;
        agent.speed = velocidadPersecucion;
        agent.ResetPath();
    }

    private void DetenerPersecucion()
    {
        if (estadoActual == EstadoNPC.Patrulla || estadoActual == EstadoNPC.CazaFinal) return;

        estadoActual = EstadoNPC.Patrulla;
        agent.speed = velocidadPatrulla;
        agent.ResetPath();

        if (waypoints.Length > 0)
        {
            // Volver al waypoint más cercano (evita saltos)
            indiceWaypointActual = EncontrarWaypointMasCercano();
            SetValidDestination(waypoints[indiceWaypointActual].position, "Waypoint más cercano");
        }
    }

    private void IrAlSiguienteWaypoint()
    {
        if (waypoints.Length == 0) return;

        indiceWaypointActual = (indiceWaypointActual + 1) % waypoints.Length;
        SetValidDestination(waypoints[indiceWaypointActual].position, "Siguiente waypoint");
    }

    private int EncontrarWaypointMasCercano()
    {
        float min = float.MaxValue;
        int idx = 0;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (!waypoints[i]) continue;
            float d = Vector3.Distance(transform.position, waypoints[i].position);
            if (d < min) { min = d; idx = i; }
        }
        return idx;
    }

    /* -------------------------------------------------
       NAVMESH VALIDATION
       ------------------------------------------------- */
    private bool SetValidDestination(Vector3 target, string descripcion)
    {
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, radioSampleNavMesh, NavMesh.AllAreas))
        {
            if (agent.SetDestination(hit.position))
            {
                Log($"Destino válido → {descripcion} ({hit.position})");
                return true;
            }
            else
            {
                LogWarning($"SetDestination falló aunque Sample OK → {descripcion}");
                return false;
            }
        }
        else
        {
            LogError($"Posición inválida en NavMesh → {descripcion} ({target})");
            return false;
        }
    }

    private bool ReposicionarEnNavMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, radioSampleNavMesh, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.enabled = false;
            agent.enabled = true;
            agent.Warp(hit.position);
            return true;
        }
        return false;
    }

    /* -------------------------------------------------
       GAME OVER
       ------------------------------------------------- */
    private void ActivarGameOver()
    {
        Log("¡Jugador atrapado! Game Over.");

        // Desactivar todos los NPCs para evitar múltiples GameOver
        foreach (var npc in FindObjectsOfType<SeguridadNPC>())
            npc.enabled = false;

        // Si hay GameManager, delegamos
        if (global::GameManager.Instance != null)
        {
            global::GameManager.Instance.Derrota();
            return;
        }

        // Fallback: panel propio
        if (panelGameOver != null) panelGameOver.SetActive(true);
        StartCoroutine(ReiniciarEscena(3f));
    }

    private IEnumerator ReiniciarEscena(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /* -------------------------------------------------
       LOGS (controlados por variable)
       ------------------------------------------------- */
    private void Log(string msg) { if (mostrarLogs) Debug.Log($"[{name}] {msg}"); }
    private void LogWarning(string msg) { if (mostrarLogs) Debug.LogWarning($"[{name}] {msg}"); }
    private void LogError(string msg) { Debug.LogError($"[{name}] {msg}"); }

    /* -------------------------------------------------
       GIZMOS
       ------------------------------------------------- */
    void OnDrawGizmosSelected()
    {
        // Radio detección
        Gizmos.color = (estadoActual == EstadoNPC.Persecucion || estadoActual == EstadoNPC.CazaFinal) ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);

        // Distancia atrapar
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, distanciaAtrapar);

        // Waypoints
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (!waypoints[i]) continue;
                Gizmos.DrawWireCube(waypoints[i].position, Vector3.one * 0.6f);
                int next = (i + 1) % waypoints.Length;
                Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
            }
        }
    }
}