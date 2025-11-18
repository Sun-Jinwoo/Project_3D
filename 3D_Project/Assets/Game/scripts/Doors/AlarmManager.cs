using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmManager : MonoBehaviour
{
    [Header("Puertas a activar")]
    public DoorController[] puertas;

    [Header("Luces de alarma")]
    public AlarmLight[] luces;

    [Header("Sonido de alarma")]
    public AudioSource alarmaAudio;

    [Header("NPCs que reaccionarán")]
    public float radioNotificacion = 20f;

    private bool alarmaActiva = false;

    public void ActivarAlarma(Vector3 origen)
    {
        if (alarmaActiva) return;
        alarmaActiva = true;

        // Activar puertas
        foreach (var p in puertas)
            p.BajarPuerta();

        // Activar luces
        foreach (var l in luces)
            l.ActivarAlarma();

        // Sonido
        if (alarmaAudio != null)
        {
            alarmaAudio.loop = true;
            alarmaAudio.Play();
        }

        // Notificar NPCs
        NotificarNPCs(origen);
    }

    public void DesactivarAlarma()
    {
        alarmaActiva = false;

        foreach (var p in puertas)
            p.SubirPuerta();

        foreach (var l in luces)
            l.DesactivarAlarma();

        if (alarmaAudio != null)
            alarmaAudio.Stop();
    }

    void NotificarNPCs(Vector3 origen)
    {
        Collider[] colls = Physics.OverlapSphere(origen, radioNotificacion);

        foreach (var col in colls)
        {
            // Si es NPC_A
            NPC_A npcA = col.GetComponent<NPC_A>();
            if (npcA != null)
            {
                npcA.RecibirAlarma(origen);
            }

            // Si es NPC_B
            NPC_B npcB = col.GetComponent<NPC_B>();
            if (npcB != null)
            {
                npcB.RecibirAlarma(origen);
            }
        }
    }
}
