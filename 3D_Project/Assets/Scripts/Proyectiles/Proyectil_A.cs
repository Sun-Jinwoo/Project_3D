using UnityEngine;
using UnityEngine.SceneManagement;

public class Proyectil_A : ProjectileBase
{
    private static int contadorImpactos = 0;

    protected override void OnHitPlayer(GameObject player)
    {
        contadorImpactos++;
        Debug.Log($"Impacto {contadorImpactos}/5");

        if (contadorImpactos >= 5)
            SceneManager.LoadScene("GameOver");
    }
}
