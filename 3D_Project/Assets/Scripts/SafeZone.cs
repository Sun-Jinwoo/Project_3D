using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("SafeZone: GameManager.Instance es null!");
            return;
        }

        // ← Victoria SOLO en CazaFinal (después del timer)
        if (GameManager.Instance.estadoActual == GameManager.EstadoJuego.CazaFinal)
        {
            Debug.Log("🎉 ¡ZONA SEGURA ALCANZADA! VICTORIA!");
            GameManager.Instance.Victoria();
        }
        else
        {
            Debug.Log("⚠️ Zona segura no válida aún. Espera la alerta.");
        }
    }
}