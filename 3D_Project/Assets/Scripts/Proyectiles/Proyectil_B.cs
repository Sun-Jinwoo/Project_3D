using UnityEngine;

public class Proyectil_B : ProjectileBase
{
    protected override void OnHitPlayer(GameObject player)
    {
        MonoBehaviour movimiento = player.GetComponent<MonoBehaviour>();
        if (movimiento != null)
        {
            movimiento.enabled = false;
            player.GetComponent<MonoBehaviour>().StartCoroutine(ReactivarMovimiento(player, 2f));
        }
    }

    private System.Collections.IEnumerator ReactivarMovimiento(GameObject player, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        MonoBehaviour movimiento = player.GetComponent<MonoBehaviour>();
        if (movimiento != null)
            movimiento.enabled = true;
    }
}

