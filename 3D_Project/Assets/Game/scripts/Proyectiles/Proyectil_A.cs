using UnityEngine;
using UnityEngine.SceneManagement;

public class Proyectil_A : ProjectileBase
{
    private static int contadorImpactos = 0;

    protected override void OnHitPlayer(GameObject player)
    {
        contadorImpactos++;
        Debug.Log($"Impacto {contadorImpactos}/10");

        if (contadorImpactos >= 10)
            SceneManager.LoadScene("GameOver");
    }
}
