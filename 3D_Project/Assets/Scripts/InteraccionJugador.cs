using UnityEngine;

public class InteraccionJugador : MonoBehaviour
{
    public Transform spine;
    public float interactDistance = 2f;

    void Update()
    {
        Debug.DrawRay(spine.position, spine.forward * interactDistance, Color.red, 0.2f);


        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(spine.position, spine.forward);
            if (Physics.Raycast(r, out RaycastHit hit, interactDistance))
            {
                Puerta p = hit.collider.GetComponent<Puerta>();
                if (p != null)
                {
                    p.Abrir();
                    Security s = Object.FindFirstObjectByType<Security>();
                    if (s != null) s.IniciarDespertar();
                    return;
                }

                CasaLuz luz = hit.collider.GetComponent<CasaLuz>();
                if (luz != null)
                {
                    luz.Encender();
                    Security s = Object.FindFirstObjectByType<Security>();
                    if (s != null) s.DespertarAnticipado();
                    return;
                }
            }
        }
    }
}
