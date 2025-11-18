using System.Collections;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float tiempoVida = 7f;
    public float golpe = 10f;
    public GameObject efectoImpacto;
    private float velocidad = 20f;


    public enum TipoProyectil
    {
        Bala,
        Dardo
    }

    public TipoProyectil tipoProyectil = TipoProyectil.Dardo;

    private void Start()
    {
        switch(tipoProyectil)
        {
            case TipoProyectil.Bala:
                velocidad = 50f;

                break;
            case TipoProyectil.Dardo:
                velocidad = 20f;

                break;
        }
        Destroy(gameObject, tiempoVida);

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Aplicar da�o si el objeto tiene un componente de salud
        //Salud salud = collision.gameObject.GetComponent<Movement>();
        if (/*salud != null &&*/ collision.collider.CompareTag("Player"))
        {
            //salud.RecibirDaño(golpe);
            Debug.Log("Proyectil impactó y aplicó " + golpe + " de daño.");
        }
        // Instanciar efecto de impacto
        if (efectoImpacto != null)
        {
            Instantiate(efectoImpacto, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private IEnumerator Bala()
    {
        if (tipoProyectil == TipoProyectil.Bala)
        {
            while (true)
            {
                transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
                yield return null;
            }
        }
    }

    private IEnumerator Dardo()
    {
        if (tipoProyectil == TipoProyectil.Dardo)
        {
            while (true)
            {
                transform.Translate(Vector3.forward * velocidad * Time.deltaTime);

                yield return null;
            }
        }
    }

}
