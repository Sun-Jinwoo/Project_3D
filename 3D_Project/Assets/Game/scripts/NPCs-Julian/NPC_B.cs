using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NPC_B : MonoBehaviour
{
    [Header("Movimiento y detección")]
    public float rangoLargo = 18f;
    public float rangoCorto = 10f;
    public float distanciaOptima = 12f;
    public float anguloVision = 70f;
    public float velocidad = 3.5f;
    public string playerTag = "Player";
    private Transform jugador;
    private NavMeshAgent agent;
    private Vector3 ultimaPosicionVista;

    [Header("Disparo")]
    public GameObject proyectilSeleccionado;
    public Transform firePoint;
    public float fireRate = 0.8f;
    private bool puedeDisparar = true;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoDisparo;

    [Header("Waypoints de patrulla (Emptys)")]
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    [Header("Visual")]
    public Renderer indicadorRenderer;
    public Material materialNormal;
    public Material materialSospecha;
    public Material materialDisparo;
    public Material materialBusqueda;

    private enum EstadoNPC { Patrulla, Sospecha, Acercamiento, Disparo }
    private EstadoNPC estadoActual = EstadoNPC.Patrulla;

    private float tiempoSinVer = 0f;
    private bool jugadorDetectado = false;

    // Contador para GameOver
    private float tiempoCercaDelJugador = 0f;
    public float tiempoParaGameOver = 5f;
    public float distanciaGameOver = 8f;

    // Collider de detección
    private SphereCollider triggerDeteccion;
    public float SphereSize = 0.7f;

    private Animator animator;



    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); //Animador

        CambiarColor(materialNormal);

        triggerDeteccion = gameObject.AddComponent<SphereCollider>();
        triggerDeteccion.isTrigger = true;
        triggerDeteccion.radius = rangoCorto * SphereSize;
        triggerDeteccion.center = Vector3.zero;

        if (waypoints.Length > 0)
        {
            currentWaypoint = 0;
            agent.destination = waypoints[currentWaypoint].position;
        }
    }

    private void Update()
    {
        DetectarJugador();
        VerificarGameOver();
        UpdateAnimation(); // Tambien parte del animador

        switch (estadoActual)
        {
            case EstadoNPC.Patrulla: Patrullar(); break;
            case EstadoNPC.Sospecha: ModoSospecha(); break;
            case EstadoNPC.Acercamiento: ModoAcercamiento(); break;
            case EstadoNPC.Disparo: ModoDisparo(); break;
        }

        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoLargo, Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up * 0.4f, transform.forward * rangoCorto, Color.red);
    }

    // Animator controller
    private void UpdateAnimation()
    {
        if (animator == null) return;

        int animState = 0; // 0 = Idle (quieto), 1 = Caminar (patrulla), 2 = Correr (búsqueda/persecución), 3 = Disparar

        if (estadoActual == EstadoNPC.Disparo)
        {
            animState = 3; // Disparar
        }
        else if (agent.velocity.magnitude > 0.2f) // Está moviéndose
        {
            if (estadoActual == EstadoNPC.Patrulla)
                animState = 1; // Caminar
            else
                animState = 2; // Correr (sospecha o acercamiento)
        }
        // Si no se mueve → Idle (quieto)

        animator.SetInteger("AnimState", animState);
    }

    void DetectarJugador()
    {
        if (jugador == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag(playerTag);
            if (obj != null) jugador = obj.transform;
        }

        if (jugador == null) return;

        Vector3 dir = (jugador.position - transform.position).normalized;
        float distancia = Vector3.Distance(transform.position, jugador.position);
        float angulo = Vector3.Angle(transform.forward, dir);
        bool dentroFOV = angulo < anguloVision / 2f;

        if (dentroFOV && Physics.Raycast(transform.position + Vector3.up * 0.4f, dir, out RaycastHit hit, rangoLargo))
        {
            if (hit.collider.CompareTag(playerTag))
            {
                jugadorDetectado = true;
                ultimaPosicionVista = jugador.position;

                if (distancia <= rangoCorto)
                    CambiarEstado(EstadoNPC.Disparo);
                else
                    CambiarEstado(EstadoNPC.Acercamiento);
            }
        }
        else
        {
            jugadorDetectado = false;
            if (estadoActual == EstadoNPC.Disparo || estadoActual == EstadoNPC.Acercamiento)
            {
                tiempoSinVer += Time.deltaTime;
                if (tiempoSinVer >= 4f)
                    CambiarEstado(EstadoNPC.Sospecha);
            }
        }
    }

    public void RecibirAlarma(Vector3 punto)
    {
        CambiarEstado(EstadoNPC.Sospecha);
        ultimaPosicionVista = punto;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            jugador = other.transform;
            CambiarEstado(EstadoNPC.Acercamiento);
        }
    }

    void VerificarGameOver()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaGameOver)
        {
            tiempoCercaDelJugador += Time.deltaTime;

            if (tiempoCercaDelJugador >= tiempoParaGameOver)
            {
                SceneManager.LoadScene("GameOver");
            }
        }
        else
        {
            tiempoCercaDelJugador = 0f;
        }
    }

    void Patrullar()
    {
        CambiarColor(materialNormal);

        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.destination = waypoints[currentWaypoint].position;
        }
    }

    void ModoSospecha()
    {
        CambiarColor(materialSospecha);
        agent.isStopped = false;
        agent.speed = velocidad; // ← Para que corra en modo busqueda tambien
        agent.destination = ultimaPosicionVista;

        // Rotación de búsqueda 
        transform.Rotate(Vector3.up * Mathf.Sin(Time.time * 2f) * 40f * Time.deltaTime);

        tiempoSinVer += Time.deltaTime;
        if (tiempoSinVer >= 6f)
            CambiarEstado(EstadoNPC.Patrulla);
    }

    void ModoAcercamiento()
    {
        CambiarColor(materialBusqueda);
        if (jugador == null) return;

        agent.isStopped = false;
        agent.speed = velocidad;
        agent.SetDestination(jugador.position);

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaOptima)
            CambiarEstado(EstadoNPC.Disparo);
    }

    void ModoDisparo()
    {
        if (jugador == null) return;

        CambiarColor(materialDisparo);
        agent.isStopped = true;

        Vector3 mirar = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
        transform.LookAt(mirar);

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia > rangoLargo)
        {
            CambiarEstado(EstadoNPC.Acercamiento);
            return;
        }

        if (puedeDisparar)
            StartCoroutine(Disparar());
    }

    IEnumerator Disparar()
    {
        puedeDisparar = false;

        if (proyectilSeleccionado != null && firePoint != null)
        {
            GameObject proyectil = Instantiate(proyectilSeleccionado, firePoint.position, firePoint.rotation);
            if (audioSource != null && sonidoDisparo != null)
                audioSource.PlayOneShot(sonidoDisparo);
            Rigidbody rb = proyectil.GetComponent<Rigidbody>();

            if (rb != null)
                rb.linearVelocity = firePoint.forward * 25f;

            // Animacion de disparo
            animator?.SetTrigger("Fire");
        }

        yield return new WaitForSeconds(fireRate);
        puedeDisparar = true;
    }

    void CambiarEstado(EstadoNPC nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;

        estadoActual = nuevoEstado;
        tiempoSinVer = 0f;
    }

    void CambiarColor(Material mat)
    {
        if (indicadorRenderer != null && mat != null)
            indicadorRenderer.material = mat;
    }
}