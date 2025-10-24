using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f; // grados/seg

    [Header("Physics")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Optional")]
    [Tooltip("Si se asigna, el movimiento será relativo a esta cámara (ej: cámara orbital).")]
    public Camera mouseOrbitCamera;

    private CharacterController controller;
    private Animator anim;

    // Nuevo Input System: valor actual de la acción "Move"
    private Vector2 moveInput;           // x: izq-der, y: adelante-atrás
    private Vector3 velocity;            // para gravedad

    // Hash para parámetros del Animator (evita typos y es más rápido)
    private static readonly int VelX = Animator.StringToHash("velX");
    private static readonly int VelY = Animator.StringToHash("velY");

    // Suavizado para el Blend Tree
    [SerializeField] private float animDamp = 0.05f;
    private float velXCur, velYCur;      // internos para damping

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // ====== NUEVO INPUT SYSTEM ======
    // Este callback lo invoca PlayerInput cuando la acción "Move" cambia.
    // En el componente PlayerInput, asigna:
    //   - Actions: tu InputActionAsset
    //   - Behavior: Invoke Unity Events
    //   - Events > Move: arrastra el Player y selecciona PlayerMovement.OnMove
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>(); // (-1..1 , -1..1)
    }

    private void Update()
    {
        // 1) Calcular dirección de movimiento (relativa a cámara si existe)
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y); // x=strafe, y=forward

        Vector3 moveWorld;
        if (mouseOrbitCamera != null && mouseOrbitCamera.gameObject.activeInHierarchy)
        {
            // plano XZ de la cámara
            Vector3 camFwd = mouseOrbitCamera.transform.forward;
            camFwd.y = 0f;
            camFwd.Normalize();

            Vector3 camRight = mouseOrbitCamera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            moveWorld = camRight * input.x + camFwd * input.z;
        }
        else
        {
            // sin cámara: usar el forward del personaje
            moveWorld = transform.right * input.x + transform.forward * input.z;
        }

        // 2) Rotar hacia la dirección de avance si hay input
        Vector3 lookDir = new Vector3(moveWorld.x, 0f, moveWorld.z);
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // 3) Mover con CharacterController
        Vector3 horizontal = moveWorld * moveSpeed;
        controller.Move(horizontal * Time.deltaTime);

        // 4) Gravedad
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f; // pequeño empuje hacia el suelo para mantener grounded
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5) Enviar parámetros al Animator (Blend Tree 2D Freeform: velX, velY)
        // velX/velY en el BlendTree deben ser el input local (x,y) del jugador.
        // Usamos damping para que el punto rojo del Blend Tree se mueva suave.
        velXCur = Mathf.SmoothDamp(velXCur, moveInput.x, ref velXCur, animDamp);
        velYCur = Mathf.SmoothDamp(velYCur, moveInput.y, ref velYCur, animDamp);
        anim.SetFloat(VelX, velXCur);
        anim.SetFloat(VelY, velYCur);
    }
}


