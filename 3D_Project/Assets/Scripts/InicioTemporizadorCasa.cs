using UnityEngine;

public class InicioTemporizadorCasa : MonoBehaviour
{
    public float tiempoLimite = 120f; // segundos del temporizador
    bool temporizadorIniciado = false;

    void OnTriggerEnter(Collider other)
    {
        if (temporizadorIniciado) return;

        if (other.CompareTag("Player"))
        {
            temporizadorIniciado = true;
            GameManager.Instance.IniciarTemporizadorCasa(tiempoLimite);
        }
    }
}
