using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para cargar escenas

public class Interactive_Object : MonoBehaviour
{
    [Header("Configuración del movimiento")]
    public float amplitud = 0.5f;
    public float velocidad = 1f;

    [Header("Contador y UI")]
    public static int contadorDestruidos = 0;  // Contador estático para contar las destrucciones
    public TextMesh textoContador;  // Referencia a TextMesh para mostrar el contador

    private float posInicialY;

    void Start()
    {
        posInicialY = transform.position.y;

        // Si el TextMesh no está asignado, lo buscamos en el objeto que lo contiene
        if (textoContador == null)
        {
            textoContador = FindObjectOfType<TextMesh>();
        }

        ActualizarContador();  // Actualiza el texto del contador al inicio
    }

    void Update()
    {
        // Movimiento oscilante
        float nuevaY = posInicialY + Mathf.Sin(Time.time * velocidad) * amplitud;

        transform.position = new Vector3(
            transform.position.x,
            nuevaY,
            transform.position.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cuando el objeto es destruido
        Destroy(gameObject);

        // Incrementar el contador de objetos destruidos
        contadorDestruidos++;

        // Actualizar el texto del contador
        ActualizarContador();

        // Si el contador llega a 12, cargar la siguiente escena
        if (contadorDestruidos >= 11)
        {
            CargarSiguienteEscena();
        }
    }

    private void ActualizarContador()
    {
        // Actualiza el texto del contador en la escena
        if (textoContador != null)
        {
            textoContador.text = "Objetos Destruidos: " + contadorDestruidos;
        }
    }

    private void CargarSiguienteEscena()
    {
        // Cargar la siguiente escena
        // Asegúrate de tener una escena configurada con el índice correcto en Build Settings de Unity
        int siguienteEscenaIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (siguienteEscenaIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene("05_Robo_Zona_Militar");
        }
        else
        {
            Debug.Log("No hay más escenas en la lista de Build Settings.");
        }
    }
}

