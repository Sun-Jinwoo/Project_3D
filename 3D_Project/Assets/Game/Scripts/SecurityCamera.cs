using UnityEngine;
using System;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private float rotationInterval = 3f; // Tiempo entre rotaciones (segundos)
    [SerializeField] private float maxRotationAngle = 45f; // �ngulo m�ximo de rotaci�n (izquierda y derecha)
    [SerializeField] private float rotationSpeed = 90f; // Velocidad de rotaci�n (grados/segundo)
    [SerializeField] private float detectionRange = 10f; // Rango de detecci�n
    [SerializeField] private float fieldOfViewAngle = 60f; // �ngulo del campo de visi�n (grados)
    [SerializeField] private LayerMask playerLayer; // Capa del jugador
    [SerializeField] private Transform cameraHead; // Transform de la cabeza de la c�mara (para rotar)
    [SerializeField] private Material fovMaterial; // Material para el cono de visi�n (semitransparente)
    [SerializeField] private int fovMeshSegments = 10; // Segmentos para la malla del cono

    private float timer = 0f;
    private Quaternion targetRotation;
    private bool isRotating = false;
    private bool rotateRight = true; // Direcci�n inicial: derecha
    private float initialYAngle; // �ngulo inicial en Y
    private GameObject fovCone; // Objeto que representa el cono de visi�n
    public Action<Vector3> OnPlayerDetected; // Evento para notificar al NPC

    void Start()
    {
        // Establecer rotaci�n inicial
        if (cameraHead == null) cameraHead = transform;
        initialYAngle = cameraHead.eulerAngles.y;
        targetRotation = cameraHead.rotation;

        // Crear el cono de visi�n
        CreateFOVCone();
    }

    void Update()
    {
        // Temporizador para la rotaci�n
        timer += Time.deltaTime;
        if (timer >= rotationInterval && !isRotating)
        {
            StartRotation();
        }

        // Rotar suavemente hacia el objetivo
        if (isRotating)
        {
            cameraHead.rotation = Quaternion.RotateTowards(
                cameraHead.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Comprobar si la rotaci�n est� completa
            if (Quaternion.Angle(cameraHead.rotation, targetRotation) < 0.1f)
            {
                isRotating = false;
                timer = 0f;
            }
        }

        // Actualizar la posici�n y rotaci�n del cono de visi�n
        UpdateFOVCone();

        // Detecci�n del jugador con campo de visi�n
        DetectPlayer();
    }

    void StartRotation()
    {
        // Alternar direcci�n entre izquierda y derecha
        float targetYAngle = initialYAngle + (rotateRight ? maxRotationAngle : -maxRotationAngle);
        targetRotation = Quaternion.Euler(0f, targetYAngle, 0f);
        rotateRight = !rotateRight; // Cambiar direcci�n para la pr�xima rotaci�n
        isRotating = true;
    }

    void DetectPlayer()
    {
        // Encontrar al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Verificar si el jugador est� dentro del rango
        Vector3 directionToPlayer = player.transform.position - cameraHead.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            // Calcular el �ngulo entre la direcci�n de la c�mara y la direcci�n al jugador
            float angleToPlayer = Vector3.Angle(cameraHead.forward, directionToPlayer);

            // Comprobar si el jugador est� dentro del campo de visi�n
            if (angleToPlayer <= fieldOfViewAngle * 0.5f)
            {
                // Verificar si hay obst�culos usando raycast
                Ray ray = new Ray(cameraHead.position, directionToPlayer);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, detectionRange, playerLayer))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        // Invocar evento para el NPC con la posici�n del jugador
                        OnPlayerDetected?.Invoke(hit.point);
                        Debug.Log("Jugador detectado en: " + hit.point);
                    }
                }
            }
        }

        // Visualizar el campo de visi�n en la escena (para depuraci�n)
        Debug.DrawRay(cameraHead.position, Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * cameraHead.forward * detectionRange, Color.red);
        Debug.DrawRay(cameraHead.position, Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * cameraHead.forward * detectionRange, Color.red);
    }

    void CreateFOVCone()
    {
        // Crear un GameObject para el cono de visi�n
        fovCone = new GameObject("FOVCone");
        fovCone.transform.SetParent(cameraHead, false);

        // A�adir MeshFilter y MeshRenderer
        MeshFilter meshFilter = fovCone.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = fovCone.AddComponent<MeshRenderer>();
        meshRenderer.material = fovMaterial;

        // Generar la malla del cono
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Crear v�rtices para el cono
        Vector3[] vertices = new Vector3[fovMeshSegments + 1];
        int[] triangles = new int[fovMeshSegments * 3];

        vertices[0] = Vector3.zero; // V�rtice en la base (posici�n de la c�mara)
        float angleStep = fieldOfViewAngle / fovMeshSegments;

        // Generar v�rtices en el borde del cono
        for (int i = 0; i < fovMeshSegments; i++)
        {
            float angle = -fieldOfViewAngle * 0.5f + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward * detectionRange;
            vertices[i + 1] = direction;
        }

        // Generar tri�ngulos
        for (int i = 0; i < fovMeshSegments - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        // �ltimo tri�ngulo
        triangles[(fovMeshSegments - 1) * 3] = 0;
        triangles[(fovMeshSegments - 1) * 3 + 1] = fovMeshSegments;
        triangles[(fovMeshSegments - 1) * 3 + 2] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void UpdateFOVCone()
    {
        // Asegurarse de que el cono siga la rotaci�n de la c�mara
        if (fovCone != null)
        {
            fovCone.transform.position = cameraHead.position;
            fovCone.transform.rotation = cameraHead.rotation;
        }
    }

    // M�todo para conectar el NPC m�s tarde
    public void SubscribeToPlayerDetection(Action<Vector3> npcCallback)
    {
        OnPlayerDetected += npcCallback;
    }

    void OnDestroy()
    {
        // Destruir el cono de visi�n al destruir la c�mara
        if (fovCone != null)
        {
            Destroy(fovCone);
        }
    }
}