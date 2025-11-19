using UnityEngine;

public class CasaLuz : MonoBehaviour
{
    public Light luz;

    public bool encendida;

    void Start()
{
    luz.enabled = false;
}

    public void Toggle()
    {
        encendida = !encendida;
        if (luz != null)
            luz.enabled = encendida;

    }

    
}
