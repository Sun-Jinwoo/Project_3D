using UnityEngine;

public class CogerObjeto : MonoBehaviour
{
    [Header("Referencias")]
    public Transform handPoint;

    private GameObject pickedObject = null;
    private ConfigurableJoint joint;

    private void Update()
    {
        // Soltar
        if (pickedObject != null && Input.GetKeyDown(KeyCode.Q))
        {
            SoltarObjeto();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (pickedObject == null &&
            other.CompareTag("ObjetoAgarrable") &&
            Input.GetKeyDown(KeyCode.E))
        {
            AgarrarObjeto(other.gameObject);
        }
    }

    private void AgarrarObjeto(GameObject obj)
    {
        pickedObject = obj;
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        // NO hacemos kinematic → mantenemos dinámica pero controlada con joint
        rb.useGravity = false;

        // Crear ConfigurableJoint
        joint = obj.AddComponent<ConfigurableJoint>();
        joint.connectedBody = GetComponent<Rigidbody>(); // Conectar al jugador
        joint.anchor = Vector3.zero;

        // Configuración del joint para que siga exactamente la mano
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        // Posicionar inicialmente en la mano
        obj.transform.position = handPoint.position;
        obj.transform.rotation = handPoint.rotation;

        // Opcional: conectar visualmente como hijo (solo visual, no afecta física)
        obj.transform.SetParent(handPoint);
    }

    private void SoltarObjeto()
    {
        if (pickedObject == null) return;

        Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
        rb.useGravity = true;

        // Transferir velocidad del jugador al soltar
        Rigidbody playerRb = GetComponent<Rigidbody>();
        if (playerRb != null)
            rb.linearVelocity = playerRb.linearVelocity;

        // Destruir el joint
        if (joint != null)
            Destroy(joint);

        // Quitar parent visual
        pickedObject.transform.SetParent(null);

        pickedObject = null;
        joint = null;
    }
}