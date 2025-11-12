using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    [Header("Movimiento base")]
    public float speed = 2f;
    public float stoppingDistance = 0.3f;
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    [Header("Raycasts de detección")]
    public float rangoCorto = 7f;
    public float rangoLargo = 10f;
    public float anguloBusquedaMin = -45f;
    public float anguloBusquedaMax = 45f;
    public float velocidadGiroBusqueda = 3f;
    public float velocidadDeBusqueda = 4f;
    public float tiempoBusqueda = 4f;

    [Header("Detección del jugador")]
    public string Player = "Player";
    private Transform jugador;
    private Vector3 puntoSospecha;
    private bool enInvestigacion = false;
    private bool enSospecha = false;
    private bool enPersecucion = false;

    [Header("Persecución")]
    public float tiempoQuietoAntesDePerseguir = 1.5f;
    public float tiempoExtraDePersecucion = 2f;

    [Header("Sospecha")]
    public float tiempoQuietoAntesDeSospechar = 1f;
    public float velocidadDeSospecha = 3f;

    [Header("Visual")]
    public Renderer indicadorRenderer;
    public Material materialNormal;
    public Material materialSospecha;
    public Material materialBusqueda;
    public Material materialAlerta;

    private void Start()
    {
        CambiarColor(materialNormal);
        if (waypoints.Length > 0)
            transform.position = waypoints[0].position;
    }

    private void Update()
    {
        if (!enInvestigacion && !enSospecha && !enPersecucion)
        {
            Patrullar();
            DetectarJugador();
        }
        else if (enPersecucion)
        {
            PerseguirJugador();
        }

        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoCorto, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoLargo, Color.yellow);
    }

    // ---------------------- COMPORTAMIENTO DE PATRULLA ----------------------
    void Patrullar()
    {
        if (waypoints.Length == 0) return;

        Transform objetivo = waypoints[currentWaypoint];
        Vector3 direccion = (objetivo.position - transform.position).normalized;
        direccion.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direccion), Time.deltaTime * 3f);
        MoverEnTerreno(transform.forward, speed);

        if (Vector3.Distance(transform.position, objetivo.position) < stoppingDistance)
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    // ---------------------- DETECCIÓN DEL JUGADOR ----------------------
    void DetectarJugador()
    {
        Ray rayoLargo = new Ray(transform.position, transform.forward * rangoLargo);
        Ray rayoCorto = new Ray(transform.position + Vector3.up * 0.4f, transform.forward * rangoCorto);
        RaycastHit hit;

        bool detectaCorto = false;
        bool detectaLargo = false;

        // Rango corto → persecución directa (prioritaria)
        if (Physics.Raycast(rayoCorto, out hit, rangoCorto))
        {
            if (hit.collider.CompareTag(Player))
            {
                jugador = hit.collider.transform;
                detectaCorto = true;
            }
        }

        // Rango largo → sospecha o seguimiento si no hay persecución
        if (Physics.Raycast(rayoLargo, out hit, rangoLargo))
        {
            if (hit.collider.CompareTag(Player))
            {
                jugador = hit.collider.transform;
                detectaLargo = true;
            }
        }

        // PRIORIDADES:
        if (detectaCorto)
        {
            // Si el corto lo detecta, ignora cualquier sospecha y persigue directamente
            if (!enPersecucion)
                StartCoroutine(IniciarPersecucion());
        }
        else if (detectaLargo && !enSospecha && !enPersecucion)
        {
            StartCoroutine(EntrarEnSospecha());
        }
    }

    // ---------------------- ESTADOS DE COMPORTAMIENTO ----------------------

    // 🟡 Sospecha (raycast largo)
    private IEnumerator EntrarEnSospecha()
    {
        enSospecha = true;
        CambiarColor(materialSospecha);

        // 1. Quieto brevemente antes de moverse
        yield return new WaitForSeconds(tiempoQuietoAntesDeSospechar);

        // 2. Mientras detecte al jugador con el rayo largo, se moverá hacia él
        float tiempoSinVer = 0f;
        while (true)
        {
            Ray rayoLargo = new Ray(transform.position + Vector3.up * 0.4f, transform.forward);
            RaycastHit hit;
            bool veJugador = false;

            if (Physics.Raycast(rayoLargo, out hit, rangoLargo))
            {
                if (hit.collider.CompareTag(Player))
                {
                    veJugador = true;
                    jugador = hit.collider.transform;
                    puntoSospecha = jugador.position;
                }
            }

            // Si durante este tiempo lo detecta por el rayo corto, pasar a persecución
            Ray rayoCorto = new Ray(transform.position + Vector3.up * 0.4f, transform.forward);
            if (Physics.Raycast(rayoCorto, out hit, rangoCorto) && hit.collider.CompareTag(Player))
            {
                StartCoroutine(IniciarPersecucion());
                enSospecha = false;
                yield break;
            }

            if (veJugador && jugador != null)
            {
                // Seguir al jugador mientras se lo vea
                Vector3 dir = (jugador.position - transform.position).normalized;
                dir.y = 0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 3f);
                MoverEnTerreno(transform.forward, velocidadDeSospecha);
                tiempoSinVer = 0f;
            }
            else
            {
                // Si no lo ve más, ir al último punto donde fue visto
                tiempoSinVer += Time.deltaTime;
                Vector3 dir = (puntoSospecha - transform.position).normalized;
                dir.y = 0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 3f);
                MoverEnTerreno(transform.forward, velocidadDeSospecha);

                if (tiempoSinVer > 2f)
                {
                    break; // deja de sospechar y pasa a búsqueda
                }
            }

            yield return null;
        }

        // 3. Al llegar o no ver más al jugador, iniciar búsqueda
        enSospecha = false;
        StartCoroutine(BuscarJugador());
    }

    // 🔍 Investigación (rotación entre ángulos)
    private IEnumerator BuscarJugador()
    {
        enInvestigacion = true;
        CambiarColor(materialBusqueda);

        float tiempo = 0f;
        bool haciaMax = true;

        while (tiempo < tiempoBusqueda)
        {
            DetectarJugador();

            float direccionGiro = haciaMax ? 1f : -1f;
            transform.Rotate(Vector3.up * direccionGiro * velocidadGiroBusqueda * Time.deltaTime);

            float anguloY = NormalizarAngulo(transform.eulerAngles.y);
            if (anguloY >= anguloBusquedaMax) haciaMax = false;
            if (anguloY <= anguloBusquedaMin) haciaMax = true;

            tiempo += Time.deltaTime;
            yield return null;
        }

        enInvestigacion = false;
        CambiarColor(materialNormal);
    }

    // 🔴 Persecución directa
    private IEnumerator IniciarPersecucion()
    {
        enPersecucion = true;
        CambiarColor(materialAlerta);

        // Pausa inicial (reacción)
        yield return new WaitForSeconds(tiempoQuietoAntesDePerseguir);

        float tiempoSinVer = 0f;

        while (enPersecucion)
        {
            Ray rayoCorto = new Ray(transform.position + Vector3.up * 0.4f, transform.forward);
            RaycastHit hit;
            bool veJugador = false;

            if (Physics.Raycast(rayoCorto, out hit, rangoCorto))
            {
                if (hit.collider.CompareTag(Player))
                {
                    veJugador = true;
                    jugador = hit.collider.transform;
                }
            }

            if (veJugador && jugador != null)
            {
                Vector3 dir = (jugador.position - transform.position).normalized;
                dir.y = 0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
                MoverEnTerreno(transform.forward, speed * 1.6f);
                tiempoSinVer = 0f;
            }
            else
            {
                tiempoSinVer += Time.deltaTime;
                MoverEnTerreno(transform.forward, speed * 1.2f);

                if (tiempoSinVer >= tiempoExtraDePersecucion)
                {
                    enPersecucion = false;
                    StartCoroutine(BuscarJugador());
                }
            }

            yield return null;
        }
    }

    private void PerseguirJugador() { }

    // ---------------------- UTILIDADES ----------------------
    private void CambiarColor(Material mat)
    {
        if (indicadorRenderer != null && mat != null)
            indicadorRenderer.material = mat;
    }

    private float NormalizarAngulo(float angulo)
    {
        if (angulo > 180f) angulo -= 360f;
        return angulo;
    }

    // ✅ Mantener al NPC sobre el terreno
    private void MoverEnTerreno(Vector3 direccion, float velocidad)
    {
        Vector3 movimiento = direccion * velocidad * Time.deltaTime;
        transform.position += movimiento;

        RaycastHit suelo;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out suelo, 3f))
        {
            if (suelo.collider.CompareTag("Ground"))
            {
                Vector3 pos = transform.position;
                pos.y = suelo.point.y;
                transform.position = pos;
            }
        }
    }
}
