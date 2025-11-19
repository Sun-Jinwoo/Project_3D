using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class PantallaFinalUI : MonoBehaviour
{
    public TextMeshProUGUI dineroTMP;
    public TextMeshProUGUI tiempoTMP;

    public string siguienteEscena = "02_RoboMuseo";

    void Start()
    {
        dineroTMP.text = "Dinero obtenido: $" + GameManager.Instance.dineroTotal;
        tiempoTMP.text = "Tiempo total: " + GameManager.Instance.tiempoFinal.ToString("F2") + "s";
    }

    public void siguienteNivel(string nombreNivel)
    {
       int siguienteEscena = SceneManager.GetActiveScene().buildIndex + 1;
    }
    public void Continuar()
    {
        SceneManager.LoadScene(siguienteEscena);
    }
}
