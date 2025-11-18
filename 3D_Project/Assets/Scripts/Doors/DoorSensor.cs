using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    public float rango = 6f;
    public string playerTag = "Player";
    public AlarmManager alarma;
    public DoorController[] puertas;
    private bool puertasActivadas = false;

    void Update()
    {
        Vector3 origen = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origen, transform.forward, out RaycastHit hit, rango))
        {
            if (hit.collider.CompareTag(playerTag))
            {
                ActivarPuertas(hit.point);
            }
        }

        Debug.DrawRay(origen, transform.forward * rango, Color.red);
    }

    private void ActivarPuertas(Vector3 posicionIntrusion)
    {
        if (puertasActivadas) return;

        puertasActivadas = true;

        foreach (var puerta in puertas)
        {
            if (puerta != null)
                puerta.BajarPuerta();
        }

        alarma?.ActivarAlarma(posicionIntrusion); // ? Aquí los NPCs irán exactamente al punto donde el jugador cruzó el láser
    }

    public void DesactivarPuertas()
    {
        puertasActivadas = false;

        foreach (var puerta in puertas)
        {
            if (puerta != null)
                puerta.SubirPuerta();
        }

        alarma?.DesactivarAlarma(); // Esto desactivará la alarma global aunque se llame varias veces
    }
}