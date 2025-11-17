using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    public float rango = 6f;
    public string playerTag = "Player";
    public AlarmManager alarma;

    void Update()
    {
        Vector3 origen = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origen, transform.forward, out RaycastHit hit, rango))
        {
            if (hit.collider.CompareTag(playerTag))
            {
                alarma.ActivarAlarma(transform.position);
            }
        }

        Debug.DrawRay(origen, transform.forward * rango, Color.red);
    }
}
