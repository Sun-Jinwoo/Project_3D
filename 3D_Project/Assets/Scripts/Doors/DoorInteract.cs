using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public DoorSensor sensor; // Asigna el sensor principal
    public string playerTag = "Player";
    public float distanciaInteraccion = 2f;

    private Transform jugador;

    void Update()
    {
        if (jugador == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) jugador = p.transform;
        }

        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaInteraccion)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                sensor.DesactivarPuertas();
            }
        }
    }
}
