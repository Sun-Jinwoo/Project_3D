using UnityEngine;

public class CogerObjeto : MonoBehaviour
{
    public Transform handPoint; // Cambia a Transform para mayor precisión
    private GameObject pickedObject = null;
    private Rigidbody pickedRigidbody;
    private Vector3 velocityOffset; // Para suavizado opcional (interpolación)

    private void FixedUpdate()
    {
        if (pickedObject != null)
        {
            // Posicionar manualmente en FixedUpdate (sincronizado con física)
            Vector3 targetPosition = handPoint.position;
            pickedRigidbody.MovePosition(targetPosition);
            pickedRigidbody.MoveRotation(handPoint.rotation);

            // Opcional: mantener velocidad cero para evitar deriva
            pickedRigidbody.linearVelocity = Vector3.zero;
            pickedRigidbody.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (pickedObject != null && Input.GetKeyDown(KeyCode.Q))
        {
            SoltarObjeto();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ObjetoAgarrable") && pickedObject == null && Input.GetKeyDown(KeyCode.E))
        {
            AgarrarObjeto(other.gameObject);
        }
    }

    private void AgarrarObjeto(GameObject obj)
    {
        pickedObject = obj;
        pickedRigidbody = obj.GetComponent<Rigidbody>();

        // Desactivar gravedad y física
        pickedRigidbody.useGravity = false;
        pickedRigidbody.isKinematic = true;

        // Limpiar velocidades residuales
        pickedRigidbody.linearVelocity = Vector3.zero;
        pickedRigidbody.angularVelocity = Vector3.zero;

        // NO usar SetParent → evita interferencia física
        // pickedObject.transform.SetParent(handPoint); // ← ELIMINADO

        // Posición inicial
        pickedRigidbody.position = handPoint.position;
        pickedRigidbody.rotation = handPoint.rotation;
    }

    private void SoltarObjeto()
    {
        if (pickedRigidbody != null)
        {
            pickedRigidbody.useGravity = true;
            pickedRigidbody.isKinematic = false;

            // Opcional: aplicar velocidad del personaje al soltar
            Rigidbody playerRb = GetComponentInParent<Rigidbody>();
            if (playerRb != null)
            {
                pickedRigidbody.linearVelocity = playerRb.linearVelocity;
            }
        }

        pickedObject = null;
        pickedRigidbody = null;
    }
}