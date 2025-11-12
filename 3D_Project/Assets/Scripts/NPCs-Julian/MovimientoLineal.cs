using UnityEngine;

public class MovimientoLineal : MonoBehaviour
{
    public float speed = 3f;       
    public float distance = 10f;   

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingToTarget = true;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.right * distance;
    }

    void Update()
    {
        // Direcion de movimiento
        Vector3 destination = movingToTarget ? targetPos : startPos;

        // rotacion suave hacia el destino
        //transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(destination - transform.position), speed * Time.deltaTime * 10);

        // If reached destination, flip direction
        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            movingToTarget = !movingToTarget;
        }

        // Rotacion de vuelta
        if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(destination - transform.position)) < 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }
    }
}

