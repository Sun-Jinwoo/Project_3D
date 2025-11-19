using System.Collections;
using System.Data;
using UnityEngine;

public class Rotacion : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    public float velocidad = 50f;
    public float anguloMinimo = 45f;
    public float anguloMaximo = 180f;
    public float tiempoEspera = 2f;
    public float distanciaDeteccion = 10f;
    public float tiempoFocoJugador = 3f;
    public float anguloCamara = 30f;

    [Header("Tipo de Objeto")]
    public TipoDeRotador tipoRotador = TipoDeRotador.TorretaA;

    [Header("Tipo de Movimiento de Cámara (solo si es tipo Camara)")]
    public TipoDeComportamiento tipoComportamiento = TipoDeComportamiento.A_RotacionLibre; 

    [Header("Sistema de Disparo (para torreta B)")]
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float fuerzaDisparo = 20f;
    public float tiempoEntreDisparos = 1.5f;
    
    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoDisparo;

    [Header("Indicador de Estado (colores)")]
    public Renderer indicadorRenderer; // Objeto que cambia de material
    public Material materialVerde;
    public Material materialRojo;
    public Material materialAmarillo;

    private bool rotando = true;
    private bool haciaMaximo = true;
    private bool siguiendoJugador = false;
    private Transform jugadorDetectado;

    public enum TipoDeRotador
    {
        TorretaA,
        TorretaB,
        Camara,
        Sensor
    }

    public enum TipoDeComportamiento
    {
        A_RotacionLibre, // rotación libre
        B_RotacionEntreAngulos, // rotación entre ángulos
        C_Rotacion360, // rotación 360 grados
    }

    private void Start()
    {
        CambiarColor(materialVerde);

        switch (tipoComportamiento)
        {
            case TipoDeComportamiento.A_RotacionLibre:
                StartCoroutine(ComportamientoA());
                break;
            case TipoDeComportamiento.B_RotacionEntreAngulos:
                StartCoroutine(ComportamientoB());
                break;
            case TipoDeComportamiento.C_Rotacion360:
                StartCoroutine(ComportamientoC());
                break;
        }
    }

    private void Update()
    {
        DetectarJugador();
    }

    // -------------------------------- DETECCIÓN DE JUGADOR ------------------------------
    private void DetectarJugador()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaDeteccion))
        {
            if (hit.collider.CompareTag("Player"))
            {
                jugadorDetectado = hit.collider.transform;

                if (!siguiendoJugador)
                {
                    StartCoroutine(SeguirJugador(hit.collider.gameObject));
                    CambiarColor(materialRojo);

                    // 🔥 DISPARO INMEDIATO
                    if (tipoRotador == TipoDeRotador.TorretaB)
                        StartCoroutine(DispararAlJugador());
                }
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * distanciaDeteccion, Color.green);
        }
    }

    // ------------------------------- ROTACIONES -----------------------------------------

    private IEnumerator RotacionTorretaA() => RotarEntreAngulos();
    private IEnumerator RotacionTorretaB() => RotarEntreAngulos();

    private IEnumerator RotacionCamaraA()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, velocidad * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator RotacionCamaraB()
    {
        yield return RotarEntreAngulos();
    }

    private IEnumerator RotacionSensor()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, velocidad * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator RotarEntreAngulos()
    {
        while (true)
        {
            if (rotando && !siguiendoJugador)
            {
                float direccion = haciaMaximo ? 1 : -1;
                transform.Rotate(Vector3.up, direccion * velocidad * Time.deltaTime);

                float anguloActual = NormalizarAngulo(transform.eulerAngles.y);

                if (haciaMaximo && anguloActual >= anguloMaximo)
                {
                    haciaMaximo = false;
                    rotando = false;
                    yield return new WaitForSeconds(tiempoEspera);
                    rotando = true;
                }
                else if (!haciaMaximo && anguloActual <= anguloMinimo)
                {
                    haciaMaximo = true;
                    rotando = false;
                    yield return new WaitForSeconds(tiempoEspera);
                    rotando = true;
                }
            }
            yield return null;
        }
    }

    // ----------------------------- SEGUIMIENTO Y DISPARO -------------------------------
    private IEnumerator SeguirJugador(GameObject jugador)
    {
        siguiendoJugador = true;
        float tiempo = 0f;

        Movement mov = jugador.GetComponent<Movement>();
        if (tipoRotador == TipoDeRotador.TorretaA && mov != null)
            mov.speed *= 0.5f;

        if (tipoRotador == TipoDeRotador.TorretaA)
            StartCoroutine(DispararAlJugador());

        if (tipoRotador == TipoDeRotador.TorretaB)
            StartCoroutine(DispararAlJugador());

        while (tiempo < tiempoFocoJugador && jugadorDetectado != null)
        {
            Vector3 direccion = jugadorDetectado.position - transform.position;
            direccion.y = 0;

            if (tipoRotador == TipoDeRotador.Camara)
            {
                
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                Vector3 euler = rotacionObjetivo.eulerAngles;
                euler.x = anguloCamara;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), Time.deltaTime * 5f);
            }
            else
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 5f);
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        if (tipoRotador == TipoDeRotador.TorretaA && mov != null)
            mov.speed *= 2f;

        CambiarColor(materialAmarillo);
        yield return new WaitForSeconds(2f); // tiempo de enfriamiento visual
        CambiarColor(materialVerde);

        siguiendoJugador = false;
        jugadorDetectado = null;
    }

    private IEnumerator DispararAlJugador()
    {
        while (siguiendoJugador && jugadorDetectado != null)
        {
            if (proyectilPrefab != null && puntoDisparo != null)
            {
                GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);
                if (audioSource != null && sonidoDisparo != null)
                    audioSource.PlayOneShot(sonidoDisparo);

                Rigidbody rb = proyectil.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = (jugadorDetectado.position - puntoDisparo.position).normalized * fuerzaDisparo;
                }
            }
            yield return new WaitForSeconds(tiempoEntreDisparos);
        }
    }

    // ------------------------------- UTILIDADES -----------------------------------------
    private float NormalizarAngulo(float angulo)
    {
        if (angulo > 180f) angulo -= 360f;
        return angulo;
    }

    private void CambiarColor(Material nuevoMaterial)
    {
        if (indicadorRenderer != null && nuevoMaterial != null)
        {
            indicadorRenderer.material = nuevoMaterial;
        }
    }

    // ------------------------------- COMPORTAMIENTOS -------------------------------------

    private IEnumerator ComportamientoA()
    {
        while (true)
        {
            if (!siguiendoJugador)
            {
                if (tipoRotador == TipoDeRotador.Camara)
                    transform.rotation = Quaternion.Euler(anguloCamara, transform.eulerAngles.y + (velocidad * Time.deltaTime), 0f);
                else
                    transform.Rotate(Vector3.up, velocidad * Time.deltaTime);
            }
            yield return null;
        }
    }

    private IEnumerator ComportamientoB()
    {
        while (true)
        {
            if (rotando && !siguiendoJugador)
            {
                float direccion = haciaMaximo ? 1 : -1;
                transform.Rotate(Vector3.up, direccion * velocidad * Time.deltaTime);
                float anguloActual = NormalizarAngulo(transform.eulerAngles.y);
                if (haciaMaximo && anguloActual >= anguloMaximo)
                {
                    haciaMaximo = false;
                    rotando = false;
                    yield return new WaitForSeconds(tiempoEspera);
                    rotando = true;
                }
                else if (!haciaMaximo && anguloActual <= anguloMinimo)
                {
                    haciaMaximo = true;
                    rotando = false;
                    yield return new WaitForSeconds(tiempoEspera);
                    rotando = true;
                }
            }
            yield return null;
        }
    }

    private IEnumerator ComportamientoC()
    {
        while (true)
        {
            if (!siguiendoJugador)
            {
                transform.Rotate(Vector3.up, velocidad * Time.deltaTime);
            }
            yield return null;
        }
    }
}
