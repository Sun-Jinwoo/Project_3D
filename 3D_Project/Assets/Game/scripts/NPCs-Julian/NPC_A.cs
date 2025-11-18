using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    // ---------------------- CAPTURA ----------------------
    private bool jugadorInmovilizado = false;
    private bool puedeCapturar = true;
    private int contadorLiberacion = 0;
    private float tiempoLiberacion = 0f;
    private float tiempoMaxLiberacion = 7f;
    private float cooldownCaptura = 4f;
    private float tiempoNPCQuieto = 3f;

    private MonoBehaviour scriptMovimientoJugador; // guarda el script de movimiento

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
            case EstadoNPC.Captura:
                ModoCaptura();
                break;
        }

        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoCorto, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoLargo, Color.yellow);
    }

    // ---------------------- DETECCIÓN ----------------------
    void DetectarJugador()
    {
        if (!puedeCapturar) return; // no puede capturar durante cooldown

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
                jugador = playerObj.transform;
        }

        if (jugador == null || estadoActual == EstadoNPC.Captura) return;

        Vector3 direccion = (jugador.position - transform.position).normalized;
        float distancia = Vector3.Distance(transform.position, jugador.position);
        float angulo = Vector3.Angle(transform.forward, direccion);
        bool dentroFOV = angulo < (anguloVision / 2f);

        bool lineaDeVista = false;
        if (dentroFOV && Physics.Raycast(transform.position + Vector3.up * 0.4f, direccion, out RaycastHit hit, rangoLargo))
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
        if (jugador == null) return;

        CambiarColor(materialAlerta);
        agent.speed = speedPersecusion;
        agent.SetDestination(jugador.position);

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaMinimaAlJugador && puedeCapturar)
        {
            agent.isStopped = true;
            CambiarEstado(EstadoNPC.Captura);
            return;
        }

        Vector3 direccion = (jugador.position - transform.position).normalized;
        if (!Physics.Raycast(transform.position + Vector3.up * 0.4f, direccion, out RaycastHit hit, rangoCorto) ||
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

    // ---------------------- BÚSQUEDA ----------------------
    void ModoBusqueda()
    {
        CambiarColor(materialBusqueda);
        agent.speed = speedPatrulla;
        tiempoSinVer += Time.deltaTime;

        // Gira de lado a lado simulando búsqueda visual
        float rotacion = buscandoDireccion ? anguloBusqueda : -anguloBusqueda;
        transform.Rotate(Vector3.up * rotacion * Time.deltaTime);
        if (Random.value < 0.01f) buscandoDireccion = !buscandoDireccion;

        if (tiempoSinVer >= tiempoBusqueda)
        {
            tiempoSinVer = 0f;
            CambiarEstado(EstadoNPC.Patrulla);
        }
    }

    // ---------------------- CAPTURA ----------------------
    void ModoCaptura()
    {
        if (jugador == null) return;
        CambiarColor(materialAlerta);

        // Inmoviliza solo el movimiento, no la cámara
        if (!jugadorInmovilizado)
        {
            jugadorInmovilizado = true;
            tiempoLiberacion = 0f;
            contadorLiberacion = 0;

            // Desactiva solo el script de movimiento del jugador
            scriptMovimientoJugador = jugador.GetComponent<MonoBehaviour>();
            if (scriptMovimientoJugador != null)
                scriptMovimientoJugador.enabled = false;

            // Mostrar UI de liberación
            EscapeUI ui = FindObjectOfType<EscapeUI>();
            if (ui != null)
                ui.Mostrar();
        }

        tiempoLiberacion += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            contadorLiberacion++;
            EscapeUI ui = FindObjectOfType<EscapeUI>();
            if (ui != null)
                ui.AgregarProgreso();
        }

        EscapeUI uiCheck = FindObjectOfType<EscapeUI>();
        if (uiCheck != null && uiCheck.Completado() && tiempoLiberacion <= tiempoMaxLiberacion)
        {
            StartCoroutine(LiberarJugador());
        }
        else if (tiempoLiberacion > tiempoMaxLiberacion)
        {
            contadorLiberacion = 0;
            tiempoLiberacion = 0f;
            if (uiCheck != null)
                uiCheck.Reiniciar();
        }
    }

    // ---------------------- LIBERACIÓN DEL JUGADOR ----------------------
    private IEnumerator LiberarJugador()
    {
        // Reactiva el movimiento del jugador
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = true;

        // Oculta UI de liberación
        EscapeUI ui = FindObjectOfType<EscapeUI>();
        if (ui != null)
            ui.Ocultar();

        jugadorInmovilizado = false;
        puedeCapturar = false;
        CambiarColor(materialBusqueda);
        agent.isStopped = true;

        // Empuja ligeramente al jugador hacia atrás al liberarse
        Vector3 direccionEscape = (jugador.position - transform.position).normalized;
        jugador.position += direccionEscape * 2f;

        yield return new WaitForSeconds(tiempoNPCQuieto);

        agent.isStopped = false;
        CambiarEstado(EstadoNPC.Busqueda);

        // Cooldown de captura
        yield return new WaitForSeconds(cooldownCaptura);
        puedeCapturar = true;
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
                agent.isStopped = false;
                agent.speed = speedPatrulla;
                agent.destination = waypoints[currentWaypoint].position;
                break;

            case EstadoNPC.Sospecha:
                CambiarColor(materialSospecha);
                agent.isStopped = false;
                agent.speed = speedPatrulla * 1.2f;
                agent.destination = ultimaPosicionVista;
                break;

            case EstadoNPC.Persecucion:
                CambiarColor(materialAlerta);
                agent.isStopped = false;
                agent.speed = speedPatrulla * 1.8f;
                break;

            case EstadoNPC.Busqueda:
                CambiarColor(materialBusqueda);
                agent.isStopped = true;
                break;

            case EstadoNPC.Captura:
                CambiarColor(materialAlerta);
                agent.isStopped = true;
                break;
        }
    }
    public void RecibirAlarma(Vector3 puntoAlarma)
    {
        StopAllCoroutines();
        buscandoDireccion = true;
        CambiarColor(materialBusqueda);
        StartCoroutine(MoverAlPuntoDeAlarma(puntoAlarma));
    }

    private IEnumerator MoverAlPuntoDeAlarma(Vector3 punto)
    {
        while (Vector3.Distance(transform.position, punto) > 1f)
        {
            Vector3 direccion = (punto - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direccion), Time.deltaTime * 3);
            agent.SetDestination(punto);
            yield return null;
        }

        ModoBusqueda();
    }


    // ---------------------- VISUAL ----------------------
    void CambiarColor(Material mat)
    {
        if (indicadorRenderer != null && mat != null)
            indicadorRenderer.material = mat;
    }
}
