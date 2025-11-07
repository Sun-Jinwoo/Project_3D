using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // Objeto a seguir (el jugador)
    public Vector3 offset = new Vector3(0, 5, -10); // Posición relativa detrás del jugador
    public float smoothSpeed = 0.125f; // Velocidad de suavizado

    private void LateUpdate()
    {
        if (target != null)
        {
            // Calcular la posición deseada detrás del jugador
            Vector3 desiredPosition = target.position + target.TransformDirection(offset);

            // Suavizar el movimiento de la cámara
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // Hacer que la cámara mire al jugador
            transform.LookAt(target);
        }
    }
}
