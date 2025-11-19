using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    [SerializeField] private LayerMask CapaDetectable;
    public float rotacion = 0.1f;
    public float Congelado = 2f;
    public float RetomarOperacion = 3f;

    private float posicionInicialY;


    void Update()
    {
        transform.Rotate(new Vector3(0f, rotacion * Time.deltaTime, 0f));

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //Congelar al jugador
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.green);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            {
                StartCoroutine(CongelarJugador(hit.collider.gameObject));
                Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
            }
        }
    }

    private IEnumerator CongelarJugador(GameObject jugador)
    {
        var movimiento = jugador.GetComponent<Movement>();
        if (movimiento != null)
        {
            movimiento.enabled = false;
            yield return new WaitForSeconds(Congelado);
            movimiento.enabled = true;
        }
    }

    private IEnumerator TiempoEspera(GameObject jugador)
    {
        yield return new WaitForSeconds(RetomarOperacion);
    }
}
