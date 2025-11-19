using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NPC_A : MonoBehaviour
{
    [Header("Movimiento base")]
    public float speedPatrulla = 2f;
    public float speedPersecusion = 4.5f;
    public Transform[] waypoints;
    private int currentWaypoint = 0;
    private NavMeshAgent agent;

    [Header("Detección del jugador")]
    public string playerTag = "Player";
    private Transform jugador;
    private Vector3 ultimaPosicionVista;

    [Header("Rangos y visión")]
    public float rangoCorto = 12f;
    public float rangoLargo = 20f;
    [Range(10, 180)] public float anguloVision = 40f;
    public float distanciaMinimaAlJugador = 2f;
    public float tiempoPerderJugador = 6f;
    public float tiempoBusqueda = 6f;
    public float anguloBusqueda = 50f;

    private enum EstadoNPC { Patrulla, Sospecha, Persecucion, Busqueda, Captura }
    private EstadoNPC estadoActual = EstadoNPC.Patrulla;

    [Header("Visual")]
    public Renderer indicadorRenderer;
    public Material materialNormal;
    public Material materialSospecha;
    public Material materialBusqueda;
    public Material materialAlerta;

    private float tiempoSinVer = 0f;
    private bool buscandoDireccion = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        CambiarColor(materialNormal);

        if (waypoints.Length > 0)
        {
            agent.speed = speedPatrulla;
            agent.destination = waypoints[currentWaypoint].position;
        }
    }

    private void Update()
    {
        DetectarJugador();

        switch (estadoActual)
        {
            case EstadoNPC.Patrulla: Patrullar(); break;
            case EstadoNPC.Sospecha: ModoSospecha(); break;
            case EstadoNPC.Persecucion: ModoPersecucion(); break;
            case EstadoNPC.Busqueda: ModoBusqueda(); break;
            case EstadoNPC.Captura: ModoCaptura(); break;
        }

        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoCorto, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoLargo, Color.yellow);
    }

    // --------------------- DETECCIÓN ----------------------------

    void DetectarJugador()
    {
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null) jugador = playerObj.transform;
        }

        if (jugador == null) return;
        if (estadoActual == EstadoNPC.Captura) return;

        Vector3 dir = (jugador.position - transform.position).normalized;
        float distancia = Vector3.Distance(transform.position, jugador.position);
        float angulo = Vector3.Angle(transform.forward, dir);

        bool dentroFOV = angulo < anguloVision / 2f;
        bool lineaDeVista = false;

        if (dentroFOV && Physics.Raycast(transform.position + Vector3.up * 0.4f, dir, out RaycastHit hit, rangoLargo))
        {
            if (hit.collider.CompareTag(playerTag))
                lineaDeVista = true;
        }

        if (distancia <= rangoCorto && lineaDeVista)
        {
            ultimaPosicionVista = jugador.position;
            CambiarEstado(EstadoNPC.Persecucion);
        }
        else if (distancia <= rangoLargo && lineaDeVista && estadoActual != EstadoNPC.Persecucion)
        {
            ultimaPosicionVista = jugador.position;
            CambiarEstado(EstadoNPC.Sospecha);
        }
    }

    // --------------------------- ESTADOS ----------------------------

    void Patrullar()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.destination = waypoints[currentWaypoint].position;
        }
    }

    void ModoSospecha()
    {
        agent.speed = speedPatrulla * 1.2f;
        CambiarColor(materialSospecha);
        agent.destination = ultimaPosicionVista;

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            tiempoSinVer += Time.deltaTime;
            if (tiempoSinVer >= tiempoBusqueda)
            {
                tiempoSinVer = 0f;
                CambiarEstado(EstadoNPC.Patrulla);
            }
        }
    }

    void ModoPersecucion()
    {
        if (jugador == null) return;

        CambiarColor(materialAlerta);
        agent.speed = speedPersecusion;
        agent.SetDestination(jugador.position);

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaMinimaAlJugador)
        {
            CambiarEstado(EstadoNPC.Captura);
            return;
        }

        Vector3 dir = (jugador.position - transform.position).normalized;

        if (!Physics.Raycast(transform.position + Vector3.up * 0.4f, dir, out RaycastHit hit, rangoCorto) ||
            !hit.collider.CompareTag(playerTag))
        {
            tiempoSinVer += Time.deltaTime;
            if (tiempoSinVer >= tiempoPerderJugador)
            {
                ultimaPosicionVista = jugador.position;
                CambiarEstado(EstadoNPC.Busqueda);
            }
        }
        else
        {
            tiempoSinVer = 0f;
        }
    }

    void ModoBusqueda()
    {
        CambiarColor(materialBusqueda);
        agent.speed = speedPatrulla;

        tiempoSinVer += Time.deltaTime;

        float rotacion = buscandoDireccion ? anguloBusqueda : -anguloBusqueda;
        transform.Rotate(Vector3.up * rotacion * Time.deltaTime);

        if (Random.value < 0.01f)
            buscandoDireccion = !buscandoDireccion;

        if (tiempoSinVer >= tiempoBusqueda)
        {
            tiempoSinVer = 0f;
            CambiarEstado(EstadoNPC.Patrulla);
        }
    }

    // ------------------ CAPTURA → GAME OVER DIRECTO ---------------

    void ModoCaptura()
    {
        CambiarColor(materialAlerta);
        agent.isStopped = true;

        // 🔥 Fin inmediato del juego
        SceneManager.LoadScene("GameOver");
    }

    // -------------------------- CAMBIO DE ESTADO ------------------------

    void CambiarEstado(EstadoNPC nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;
        estadoActual = nuevoEstado;
        tiempoSinVer = 0f;

        switch (estadoActual)
        {
            case EstadoNPC.Patrulla:
                CambiarColor(materialNormal);
                agent.speed = speedPatrulla;
                agent.isStopped = false;
                agent.destination = waypoints[currentWaypoint].position;
                break;

            case EstadoNPC.Sospecha:
                CambiarColor(materialSospecha);
                agent.speed = speedPatrulla * 1.2f;
                agent.isStopped = false;
                agent.destination = ultimaPosicionVista;
                break;

            case EstadoNPC.Persecucion:
                CambiarColor(materialAlerta);
                agent.speed = speedPersecusion;
                agent.isStopped = false;
                break;

            case EstadoNPC.Busqueda:
                CambiarColor(materialBusqueda);
                agent.isStopped = true;
                break;

            case EstadoNPC.Captura:
                agent.isStopped = true;
                break;
        }
    }

    // -------------------------- ALARMA ------------------------------

    public void RecibirAlarma(Vector3 punto)
    {
        StopAllCoroutines();
        buscandoDireccion = true;
        CambiarColor(materialBusqueda);
        StartCoroutine(MoverAlPuntoDeAlarma(punto));
    }

    private IEnumerator MoverAlPuntoDeAlarma(Vector3 punto)
    {
        while (Vector3.Distance(transform.position, punto) > 1f)
        {
            Vector3 dir = (punto - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 3);
            agent.SetDestination(punto);
            yield return null;
        }

        ModoBusqueda();
    }

    // ------------------------- VISUAL ---------------------------

    void CambiarColor(Material mat)
    {
        if (indicadorRenderer != null && mat != null)
            indicadorRenderer.material = mat;
    }
}
