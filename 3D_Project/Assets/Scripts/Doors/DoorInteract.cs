using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public DoorSensor[] sensores; // Ahora soporta múltiples sensores
    public string playerTag = "Player";
    public float distanciaInteraccion = 4f;

    private Transform jugador;
    private bool yaMostradoPrompt = false;

    void Update()
    {
        // Buscar jugador una sola vez
        if (jugador == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) jugador = p.transform;
            return;
        }

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // Mostrar prompt solo cuando esté cerca (opcional, pero mejora UX)
        if (distancia <= distanciaInteraccion)
        {
            if (!yaMostradoPrompt)
            {
                // Podemos colocar aqui una UI para que indique que al presionar "E" se desactivan las puertas
                Debug.Log("Pulsa E para desactivar la alarma");
                yaMostradoPrompt = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                DesactivarTodo();
            }
        }
        else
        {
            yaMostradoPrompt = false;
        }
    }

    private void DesactivarTodo()
    {
        // Desactiva TODOS los sensores asignados (y la alarma global)
        foreach (var sensor in sensores)
        {
            if (sensor != null)
                sensor.DesactivarPuertas();
        }
    }
}