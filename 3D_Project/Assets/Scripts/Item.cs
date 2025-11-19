using UnityEngine;

public class Item : MonoBehaviour
{
    public int valor = 50;

    public void Recoger()
    {
        GameManager.Instance.SumarDinero(valor);
        Destroy(gameObject);
    }
}
