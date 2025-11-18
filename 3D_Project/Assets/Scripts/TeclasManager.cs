using TMPro;
using UnityEngine;

public class TeclasManager : MonoBehaviour
{
    public TextMeshProUGUI mensaje;
    private int paso = 0;

    public float tiempoAntesDeDesvanecer = 5f;
    public float velocidadDesvanecer = 1f;

    bool iniciado;

    void Start()
    {
        if (mensaje == null)
        {
            mensaje = GetComponent<TextMeshProUGUI>();
        }

        mensaje.text = "Usa WASD para moverte";
        mensaje.alpha = 1f;
    }

    void Update()
    {
        if (paso == 0 && (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")))
        {
            paso++;
            mensaje.text = "Presiona E para abrir puertas";
        }
        else if (paso == 1 && Input.GetKeyDown(KeyCode.E))
        {
            paso++;
            mensaje.text = "Presiona F para agarrar un Objeto";
        }
        else if (paso == 2 && Input.GetKey(KeyCode.F))
        {
            paso++;
            mensaje.text = "Mantén CTRL para agacharte";
        }
        else if (paso == 3 && Input.GetKey(KeyCode.LeftControl))
        {
            paso++;
            mensaje.text = "¡Buen trabajo! Ahora explora la casa.";
            IniciarDesvanecido();     
        }
    }

    public void IniciarDesvanecido()
    {
        if (!iniciado)
        {
            iniciado = true;
            StartCoroutine(Desvanecer());
        }
    }

    System.Collections.IEnumerator Desvanecer()
    {
        yield return new WaitForSeconds(tiempoAntesDeDesvanecer);

        float a = 1f;

        while (a > 0f)
        {
            a -= Time.deltaTime * velocidadDesvanecer;
            mensaje.alpha = a;
            yield return null;
        }

        mensaje.gameObject.SetActive(false);
    }
}
