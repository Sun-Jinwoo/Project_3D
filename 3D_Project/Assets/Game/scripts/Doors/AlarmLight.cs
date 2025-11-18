using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    [Header("Alarm Material")]
    public Renderer alarmaRenderer;
    public Color colorBase = Color.white;
    public Color colorAlarma = Color.red;
    public float velocidadCambio = 3f;

    private bool alarmaActiva = false;
    private Material mat;

    void Start()
    {
        if (alarmaRenderer != null)
            mat = alarmaRenderer.material;
    }

    void Update()
    {
        if (alarmaActiva && mat != null)
        {
            float t = (Mathf.Sin(Time.time * velocidadCambio) + 1) / 2f;
            mat.color = Color.Lerp(colorBase, colorAlarma, t);
        }
    }

    public void ActivarAlarma()
    {
        alarmaActiva = true;
    }

    public void DesactivarAlarma()
    {
        alarmaActiva = false;
        if (mat != null)
            mat.color = colorBase;
    }
}
