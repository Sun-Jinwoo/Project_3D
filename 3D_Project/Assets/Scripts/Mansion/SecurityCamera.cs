using UnityEngine;
using System.Collections;

public class SecurityCamera : MonoBehaviour
{
    [Header("Configuración")]
    public float viewRange = 10f;
    public float viewAngle = 60f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Efectos")]
    public Light spotLight;
    public Color normalColor = Color.green;
    public Color alertColor = Color.red;

    [Header("Rotación Oscilante")]
    public bool oscillateCamera = true;
    public float oscillateSpeed = 1f;           // Velocidad de oscilación
    public float maxAngleLeft = 60f;            // Máximo a la izquierda (grados)
    public float maxAngleRight = -60f;          // Máximo a la derecha (grados)

    private Transform player;
    private bool playerDetected = false;
    private Coroutine oscillateRoutine;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (spotLight != null)
            spotLight.color = normalColor;

        if (oscillateCamera)
            oscillateRoutine = StartCoroutine(OscillateCamera());
    }

    void Update()
    {
        if (player == null) return;
        CheckPlayerInVision();
    }

    void CheckPlayerInVision()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewRange)
        {
            SetDetected(false);
            return;
        }

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2f)
        {
            SetDetected(false);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, viewRange, obstacleLayer))
        {
            if (hit.transform != player)
            {
                SetDetected(false);
                return;
            }
        }

        SetDetected(true);
    }

    void SetDetected(bool detected)
    {
        if (playerDetected == detected) return;

        playerDetected = detected;

        if (spotLight != null)
            spotLight.color = detected ? alertColor : normalColor;

        if (detected)
        {
            Debug.Log("¡Jugador detectado!");
            OnPlayerDetected();
        }
        else
        {
            OnPlayerLost();
        }
    }

    void OnPlayerDetected()
    {
        if (oscillateRoutine != null)
            StopCoroutine(oscillateRoutine);

        StartCoroutine(LookAtPlayer());
    }

    void OnPlayerLost()
    {
        StopAllCoroutines();
        if (oscillateCamera)
            oscillateRoutine = StartCoroutine(OscillateCamera());
    }

    IEnumerator OscillateCamera()
    {
        Quaternion startRotation = transform.rotation;
        float journey = 0f;

        while (true)
        {
            journey = 0f;
            // Ir a la izquierda
            while (journey <= 1f)
            {
                journey += Time.deltaTime * oscillateSpeed;
                float angle = Mathf.LerpAngle(0, maxAngleLeft, journey);
                transform.rotation = startRotation * Quaternion.Euler(0, angle, 0);
                yield return null;
            }

            journey = 0f;
            // Ir a la derecha
            while (journey <= 1f)
            {
                journey += Time.deltaTime * oscillateSpeed;
                float angle = Mathf.LerpAngle(maxAngleLeft, maxAngleRight, journey);
                transform.rotation = startRotation * Quaternion.Euler(0, angle, 0);
                yield return null;
            }
        }
    }

    IEnumerator LookAtPlayer()
    {
        while (playerDetected)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
            yield return null;
        }
    }

    // Gizmos
    void OnDrawGizmosSelected()
    {
        Gizmos.color = playerDetected ? Color.red : Color.yellow;
        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward * viewRange;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward * viewRange;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
        Gizmos.DrawRay(transform.position, forward * viewRange);
    }

    // Trigger opcional (puedes eliminar si no lo usas)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Intruso en el área!");
        }
    }
}