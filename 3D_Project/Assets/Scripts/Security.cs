using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Security : MonoBehaviour
{
    public SpriteRenderer ViewSpriteRenderer;
    public Transform Model;
    public Transform Head;
    public Transform PatrolStart;
    public Transform PatrolEnd;
    public float MoveSpeed = 4f;
    public float PatrolSpeed = 2f;
    public float TurnSmoothing = 5f;
    public Color DetectedColor;
    public Color NotDetectedColor;
    public float DetectionRadius = 3f;
    public float DetectionAngle = 30f;
    public LayerMask MyLayerMask;

    public float tiempoDespertar = 5f;
    bool despertarActivado = false;
    float tiempoActual;
    public bool npcDespierto = false;

    public Transform player;

    Rigidbody myRigidbody;
    bool detectedPlayer;
    bool movingTowardsPatrolStart;

    TimerManager timer;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (PatrolStart)
        {
            transform.position = PatrolStart.position;
        }

        timer = Object.FindFirstObjectByType<TimerManager>();
    }

    public void IniciarDespertar()
    {
        if (!despertarActivado && !npcDespierto)
        {
            despertarActivado = true;
            tiempoActual = tiempoDespertar;
        }
    }

    public void DespertarAnticipado()
    {
        npcDespierto = true;
        despertarActivado = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameManager.Instance.GameOver = true;
            GameManager.Instance.GameOverPanel.SetActive(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (PatrolStart && PatrolEnd)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(PatrolStart.position + Vector3.up, PatrolEnd.position + Vector3.up);
        }

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(transform.position, DetectionAngle, DetectionRadius, 0f, 1f);
    }

    private void Update()
    {
        if (detectedPlayer)
        {
            ViewSpriteRenderer.color = DetectedColor;
        }
        else
        {
            ViewSpriteRenderer.color = NotDetectedColor;
        }

        if (despertarActivado && !npcDespierto)
        {
            tiempoActual -= Time.deltaTime;

            if (tiempoActual <= 0)
            {
                npcDespierto = true;
                despertarActivado = false;
            }
        }

        if (!npcDespierto)
        {
            return;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GameOver)
            return;

        if (!npcDespierto)
            return;

        if (player == null)
            return;

        detectedPlayer = false;

        // deteccion del jugador
        if (Vector3.Distance(transform.position, player.position) <= DetectionRadius)
        {
            Vector3 positionDelta = player.position - transform.position;

            if (Vector3.Angle(Model.forward, positionDelta) <= DetectionAngle * 0.5f)
            {
                if (!Physics.Raycast(Head.position, positionDelta, positionDelta.magnitude, MyLayerMask))
                {
                    detectedPlayer = true;
                    timer?.ActivarTemporizador();
                }
            }
        }

        // si detecta jugador, lo persigue
        if (detectedPlayer)
        {
            Move(player.position, MoveSpeed);
        }
        else if (PatrolStart && PatrolEnd)
        {
            if (movingTowardsPatrolStart)
            {
                if (Move(PatrolStart.position, PatrolSpeed))
                    movingTowardsPatrolStart = false;
            }
            else
            {
                if (Move(PatrolEnd.position, PatrolSpeed))
                    movingTowardsPatrolStart = true;
            }
        }
    }

    bool Move(Vector3 pos, float speed)
    {
        Vector3 moveDirection = pos - transform.position;
        moveDirection.Normalize();

        myRigidbody.MovePosition(Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime));

        Vector3 lookDirection = moveDirection;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Model.rotation = Quaternion.Lerp(Model.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * TurnSmoothing);
        }

        return Vector3.Distance(transform.position, pos) < 0.1f;
    }
}
