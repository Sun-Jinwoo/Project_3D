using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Movimiento")]
    public float distanciaBajada = 3f;
    public float velocidad = 3f;

    private Vector3 posicionInicial;
    private Vector3 posicionBajada;
    private bool bajando = false;
    private bool subiendo = false;

    void Start()
    {
        posicionInicial = transform.position;
        posicionBajada = posicionInicial - new Vector3(0, distanciaBajada, 0);
    }

    void Update()
    {
        if (bajando)
            transform.position = Vector3.MoveTowards(transform.position, posicionBajada, velocidad * Time.deltaTime);

        if (subiendo)
            transform.position = Vector3.MoveTowards(transform.position, posicionInicial, velocidad * Time.deltaTime);
    }

    public void BajarPuerta()
    {
        bajando = true;
        subiendo = false;
    }

    public void SubirPuerta()
    {
        bajando = false;
        subiendo = true;
    }
}
