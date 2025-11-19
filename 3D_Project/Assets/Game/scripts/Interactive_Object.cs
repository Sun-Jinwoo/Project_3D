using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Interactive_Object : MonoBehaviour
{
    public float amplitud = 0.5f;
    public float velocidad = 1f;

    public static int contadorDestruidos = 0;

    private TMP_Text textoContador;
    private float posInicialY;

    void Start()
    {
        posInicialY = transform.position.y;

        GameObject textoObj = GameObject.FindWithTag("contador");
        Debug.Log("¿Encontró el objeto con tag Contador? → " + (textoObj != null));

        if (textoObj != null)
        {
            textoContador = textoObj.GetComponent<TMP_Text>();
            Debug.Log("¿TMP_Text encontrado? → " + (textoContador != null));
        }

        ActualizarContador();
    }

    void Update()
    {
        float nuevaY = posInicialY + Mathf.Sin(Time.time * velocidad) * amplitud;

        transform.position = new Vector3(
            transform.position.x,
            nuevaY,
            transform.position.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);

        contadorDestruidos++;

        Debug.Log("Contador actual: " + contadorDestruidos);

        ActualizarContador();

        if (contadorDestruidos >= 10)  // ← AHORA SON 10
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }


    private void ActualizarContador()
    {
        if (textoContador != null)
        {
            textoContador.text = contadorDestruidos.ToString();
        }
    }
}
