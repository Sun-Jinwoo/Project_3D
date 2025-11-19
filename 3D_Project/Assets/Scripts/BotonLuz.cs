using UnityEngine;

public class BotonLuz : MonoBehaviour
{
    public CasaLuz luzConectada;

    public void Activar()
    {
        if (luzConectada != null)
        {
            bool estabaApagada = !luzConectada.encendida;

            luzConectada.Toggle();

            if (estabaApagada)
            {
                Security s = Object.FindFirstObjectByType<Security>();
                if (s != null) s.DespertarAnticipado();
            }
        }
    }
}
