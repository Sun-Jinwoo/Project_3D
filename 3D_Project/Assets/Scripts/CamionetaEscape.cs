using UnityEngine;
using UnityEngine.SceneManagement;

public class CamionetaEscape : MonoBehaviour
{
    public string escenaSiguiente = "PantallaFinal";
    public float distanciaInteractuar = 3f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogError("ERROR: No encontré al jugador. TAG Player mal asignado.");
        else
            Debug.Log("Player encontrado correctamente: " + player.name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Presionaste E");

            if (player == null)
            {
                Debug.LogError("PLAYER ES NULL EN UPDATE");
                return;
            }

            float dist = Vector3.Distance(player.position, transform.position);
            Debug.Log("Distancia a camioneta: " + dist);

            if (dist <= distanciaInteractuar)
            {
                Debug.Log("Estas dentro de rango. Intentando escapar...");
                IntentarEscapar();
            }
            else
            {
                Debug.Log("Estas muy lejos de la camioneta.");
            }
        }
    }

    void IntentarEscapar()
    {
        Debug.Log("Items actuales: " + GameManager.Instance.itemsActuales);
        Debug.Log("Items requeridos: " + GameManager.Instance.itemsRequeridos);

        if (GameManager.Instance.itemsActuales >= GameManager.Instance.itemsRequeridos)
        {
            Debug.Log("ESCAPANDO: cargando pantalla final...");

            GameManager.Instance.RegistrarTiempoFinal();
            SceneManager.LoadScene(escenaSiguiente);
        }
        else
        {
            Debug.Log("No tienes suficientes items para escapar.");
        }
    }
}
