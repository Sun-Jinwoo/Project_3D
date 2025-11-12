using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [Header("Movimiento base")]
    public float speedPatrulla = 2f;
    public Transform[] waypoints;
    private int currentWaypoint = 0;
    private NavMeshAgent agent;

    [Header("Raycasts de detección")]
    public float rangoCorto = 6f;
    public float rangoLargo = 12f;
    public float distanciaMinimaAlJugador = 2f;
    public float tiempoPerderJugador = 2f;
    public float tiempoBusqueda = 6f;

    [Header("Detección del jugador")]
    public string playerTag = "Player";
    private Transform jugador;
    private Vector3 ultimaPosicionVista;

    private enum EstadoNPC { Patrulla, Sospecha, Persecucion, Busqueda }
    private EstadoNPC estadoActual = EstadoNPC.Patrulla;

    [Header("Visual")]
    public Renderer indicadorRenderer;
    public Material materialNormal;
    public Material materialSospecha;
    public Material materialBusqueda;
    public Material materialAlerta;

    private float tiempoSinVer = 0f;

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
            case EstadoNPC.Patrulla:
                Patrullar();
                break;
            case EstadoNPC.Sospecha:
                ModoSospecha();
                break;
            case EstadoNPC.Persecucion:
                ModoPersecucion();
                break;
            case EstadoNPC.Busqueda:
                ModoBusqueda();
                break;
        }

        // Visualización raycasts
        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoCorto, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoLargo, Color.yellow);
    }

    // ---------------------- DETECCIÓN DEL JUGADOR ----------------------
    void DetectarJugador()
    {
        Ray rayCorto = new Ray(transform.position + Vector3.up * 0.4f, transform.forward);
        Ray rayLargo = new Ray(transform.position + Vector3.up * 0.4f, transform.forward);
        RaycastHit hit;

        bool veCorto = false;
        bool veLargo = false;

        if (Physics.Raycast(rayCorto, out hit, rangoCorto))
        {
            if (hit.collider.CompareTag(playerTag))
            {
                jugador = hit.collider.transform;
                ultimaPosicionVista = jugador.position;
                veCorto = true;
            }
        }

        if (Physics.Raycast(rayLargo, out hit, rangoLargo))
        {
            if (hit.collider.CompareTag(playerTag))
            {
                jugador = hit.collider.transform;
                ultimaPosicionVista = jugador.position;
                veLargo = true;
            }
        }

        // PRIORIDADES
        if (veCorto)
        {
            CambiarEstado(EstadoNPC.Persecucion);
        }
        else if (veLargo && estadoActual != EstadoNPC.Persecucion)
        {
            CambiarEstado(EstadoNPC.Sospecha);
        }
    }

    // ---------------------- PATRULLA ----------------------
    void Patrullar()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.destination = waypoints[currentWaypoint].position;
        }
    }

    // ---------------------- SOSPECHA ----------------------
    void ModoSospecha()
    {
        agent.speed = speedPatrulla * 1.2f;
        CambiarColor(materialSospecha);
        agent.destination = ultimaPosicionVista;

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            tiempoSinVer += Time.deltaTime;

            // busca por un rato
            if (tiempoSinVer >= tiempoBusqueda)
            {
                tiempoSinVer = 0f;
                CambiarEstado(EstadoNPC.Patrulla);
            }
        }
    }

    // ---------------------- PERSECUCIÓN ----------------------
    void ModoPersecucion()
    {
        if (jugador == null)
        {
            CambiarEstado(EstadoNPC.Busqueda);
            return;
        }

        CambiarColor(materialAlerta);
        agent.speed = speedPatrulla * 1.8f;

        agent.SetDestination(jugador.position);

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaMinimaAlJugador)
        {
            // NPC alcanzó al jugador (aquí podrías implementar daño o interacción)
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }

        // Si no lo ve, empieza conteo para perderlo
        Ray rayCorto = new Ray(transform.position + Vector3.up * 0.4f, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(rayCorto, out hit, rangoCorto))
        {
            if (hit.collider.CompareTag(playerTag))
            {
                tiempoSinVer = 0f;
            }
        }
        else
        {
            tiempoSinVer += Time.deltaTime;
            if (tiempoSinVer >= tiempoPerderJugador)
            {
                tiempoSinVer = 0f;
                ultimaPosicionVista = jugador.position;
                CambiarEstado(EstadoNPC.Busqueda);
            }
        }
    }

    // ---------------------- BÚSQUEDA ----------------------
    void ModoBusqueda()
    {
        CambiarColor(materialBusqueda);
        agent.speed = speedPatrulla;
        agent.destination = ultimaPosicionVista;

        tiempoSinVer += Time.deltaTime;

        // Movimiento leve aleatorio en el área
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Vector3 randomPoint = ultimaPosicionVista + new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
                agent.destination = hit.position;
        }

        if (tiempoSinVer >= tiempoBusqueda)
        {
            tiempoSinVer = 0f;
            CambiarEstado(EstadoNPC.Patrulla);
        }
    }

    // ---------------------- CAMBIO DE ESTADO ----------------------
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
                agent.destination = waypoints[currentWaypoint].position;
                break;

            case EstadoNPC.Sospecha:
                CambiarColor(materialSospecha);
                agent.speed = speedPatrulla * 1.2f;
                agent.destination = ultimaPosicionVista;
                break;

            case EstadoNPC.Persecucion:
                CambiarColor(materialAlerta);
                agent.speed = speedPatrulla * 1.8f;
                break;

            case EstadoNPC.Busqueda:
                CambiarColor(materialBusqueda);
                agent.speed = speedPatrulla;
                agent.destination = ultimaPosicionVista;
                break;
        }
    }

    // ---------------------- UTILIDAD VISUAL ----------------------
    void CambiarColor(Material mat)
    {
        if (indicadorRenderer != null && mat != null)
            indicadorRenderer.material = mat;
    }
}
