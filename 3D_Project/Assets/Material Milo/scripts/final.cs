using UnityEngine;

public class final : MonoBehaviour
{

public GameObject cartelVictoria;
public bool juegoTerminado = false;
void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Has llegado al final del juego!");//alarma de consola
            juegoTerminado = true;
            cartelVictoria.SetActive(true); // Muestra el cartel de victoria
        }
    }
}
