using UnityEngine;

public class CasaLuz : MonoBehaviour
{
    public Light luz;
    public bool encendida = false;

    void Start()
    {
        if (luz) luz.enabled = false;
    }

    public void Encender()
    {
        encendida = true;
        luz.enabled = true;
    }
}
