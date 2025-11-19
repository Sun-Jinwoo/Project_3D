using UnityEngine;
using UnityEngine.UI;

public class EscapeUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image barraProgreso;
    public GameObject panelEscape;

    private float progresoActual = 0f;
    private float progresoMaximo = 10f;
    private bool activo = false;

    void Start()
    {
        if (panelEscape != null)
            panelEscape.SetActive(false);
    }

    public void Mostrar()
    {
        activo = true;
        progresoActual = 0f;
        if (panelEscape != null)
            panelEscape.SetActive(true);
        ActualizarBarra();
    }

    public void Ocultar()
    {
        activo = false;
        if (panelEscape != null)
            panelEscape.SetActive(false);
    }

    public void AgregarProgreso()
    {
        if (!activo) return;
        progresoActual++;
        ActualizarBarra();
    }

    public void Reiniciar()
    {
        progresoActual = 0f;
        ActualizarBarra();
    }

    private void ActualizarBarra()
    {
        if (barraProgreso != null)
            barraProgreso.fillAmount = progresoActual / progresoMaximo;
    }

    public bool Completado()
    {
        return progresoActual >= progresoMaximo;
    }
}
