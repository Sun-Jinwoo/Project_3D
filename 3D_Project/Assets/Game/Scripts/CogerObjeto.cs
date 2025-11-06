using UnityEngine;
using UnityEngine.InputSystem; // ← ESTO ES LO ÚNICO NUEVO

public class CogerObjeto : MonoBehaviour
{
    public GameObject handPoint;
    private GameObject pickedObject = null;

    void Update()
    {
        // SOLTAR con Q
        if (pickedObject != null && Keyboard.current.qKey.isPressed)
        {
            Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            pickedObject.transform.SetParent(null);
            pickedObject = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ObjetoAgarrable"))
        {
            // AGARRAR con E (solo una vez al pulsar)
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                other.transform.position = handPoint.transform.position;
                other.transform.SetParent(handPoint.transform);
                pickedObject = other.gameObject;
            }
        }
    }
}