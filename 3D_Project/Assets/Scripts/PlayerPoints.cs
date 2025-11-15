using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.SumarPuntos(1); // 1 punto por objeto
        }
    }
}