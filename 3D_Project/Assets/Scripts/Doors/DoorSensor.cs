using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    [Header("Detección")]
    public float rango = 6f;
    public string playerTag = "Player";

    [Header("Control puertas")]
    public DoorController[] puertas; // Asigna todas las puertas en el inspector
    public AlarmLight alarma;       // Objeto que representa la alarma visual

    private bool puertasActivadas = false;

    void Update()
    {
        DetectarJugador();
    }

    void DetectarJugador()
    {
        Vector3 origen = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origen, transform.forward, out RaycastHit hit, rango))
        {
            if (hit.collider.CompareTag(playerTag) && !puertasActivadas)
            {
                ActivarPuertas();
            }
        }

        Debug.DrawRay(origen, transform.forward * rango, Color.red);
    }

    void ActivarPuertas()
    {
        puertasActivadas = true;

        foreach (var puerta in puertas)
            puerta.BajarPuerta();

        if (alarma != null)
            alarma.ActivarAlarma();
    }

    public void DesactivarPuertas()
    {
        puertasActivadas = false;

        foreach (var puerta in puertas)
            puerta.SubirPuerta();

        if (alarma != null)
            alarma.DesactivarAlarma();
    }
}
