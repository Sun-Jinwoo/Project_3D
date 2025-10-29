using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public Camera thirdPersonCamera; // Cámara en tercera persona
    public Camera firstPersonCamera; // Cámara en primera persona
    public Camera mouseOrbitCamera; // Nueva cámara orbital
    public Transform playerBody; // Cuerpo del jugador para sincronizar la posición relativa

    private float xRotation = 0f; // Control de la rotación vertical de la cámara en primera persona
    public float orbitSensitivity = 500f; // Sensibilidad de la cámara orbital
    public float orbitDistance = 5f; // Distancia de la cámara orbital
    private float orbitXRotation = 0f; // Rotación vertical de la cámara orbital
    private float orbitYRotation = 0f; // Rotación horizontal de la cámara orbital

    private void Start()
    {
        // Activar inicialmente la cámara en tercera persona
        ActivateCamera(thirdPersonCamera, false);
    }

    private void Update()
    {
        //// Cambiar entre cámaras con las teclas T, Y y U
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    SmoothSwitchToCamera(thirdPersonCamera);
        //}
        //else if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    SmoothSwitchToCamera(firstPersonCamera);
        //}
        //else if (Input.GetKeyDown(KeyCode.U))
        //{
        //    SmoothSwitchToCamera(mouseOrbitCamera);
        //}

        //// Controlar la cámara orbital si está activa
        //if (mouseOrbitCamera.gameObject.activeSelf)
        //{
        //    ControlMouseOrbitCamera();
        //}

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard.tKey.wasPressedThisFrame)
            SmoothSwitchToCamera(thirdPersonCamera);
        else if (keyboard.yKey.wasPressedThisFrame)
            SmoothSwitchToCamera(firstPersonCamera);
        else if (keyboard.uKey.wasPressedThisFrame)
            SmoothSwitchToCamera(mouseOrbitCamera);

        if (mouseOrbitCamera.gameObject.activeSelf)
        {
            ControlMouseOrbitCamera(mouse);
        }
    }

    private void ControlMouseOrbitCamera(Mouse mouse)
    {
        if (mouse == null) return;

        float mouseX = mouse.delta.x.ReadValue() * orbitSensitivity * Time.deltaTime;
        float mouseY = mouse.delta.y.ReadValue() * orbitSensitivity * Time.deltaTime;

        orbitYRotation += mouseX;
        orbitXRotation -= mouseY;
        orbitXRotation = Mathf.Clamp(orbitXRotation, -35f, 60f);

        Quaternion rotation = Quaternion.Euler(orbitXRotation, orbitYRotation, 0f);
        Vector3 offset = rotation * new Vector3(0, 1.5f, -orbitDistance);
        mouseOrbitCamera.transform.position = playerBody.position + offset;
        mouseOrbitCamera.transform.LookAt(playerBody);
    }

    private void SmoothSwitchToCamera(Camera targetCamera)
    {
        StartCoroutine(SmoothSwitchCoroutine(targetCamera));
    }

    private IEnumerator SmoothSwitchCoroutine(Camera targetCamera)
    {
        Camera currentCamera = thirdPersonCamera.gameObject.activeSelf ? thirdPersonCamera :
                            firstPersonCamera.gameObject.activeSelf ? firstPersonCamera :
                            mouseOrbitCamera;

        Vector3 startPosition = currentCamera.transform.position;
        Quaternion startRotation = currentCamera.transform.rotation;

        Vector3 targetPosition = targetCamera.transform.position;
        if (targetCamera == mouseOrbitCamera)
        {
            targetPosition.y += 1.0f; // Ajustar la altura al cambiar a la cámara orbital
        }

        Quaternion targetRotation = targetCamera.transform.rotation;

        float transitionTime = 0.5f; // Duración de la transición
        float elapsedTime = 0f;

        // Mantener ambas cámaras activas durante la transición
        targetCamera.gameObject.SetActive(true);

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;

            currentCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            currentCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        ActivateCamera(targetCamera, true);
    }

    private void ActivateCamera(Camera targetCamera, bool finalize)
    {

        // Ajustar la posición en Y si la cámara en primera persona está activa
        if (targetCamera == firstPersonCamera && finalize)
        {
            firstPersonCamera.transform.SetParent(playerBody); // Asegurar que sea hija del jugador
            firstPersonCamera.transform.localPosition = new Vector3(0, 0.54f, 0); // Ajustar la posición relativa
            firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }


        // Ajustar la posición en Y si la cámara orbital está activa
        if (targetCamera == mouseOrbitCamera && finalize)
        {
            // Elevar la cámara orbital al nivel deseado
            Vector3 targetPosition = mouseOrbitCamera.transform.position;
            targetPosition.y += 1.0f; // Ajusta este valor para subir la cámara
            mouseOrbitCamera.transform.position = targetPosition;
        }

        // Activar y desactivar cámaras
        thirdPersonCamera.gameObject.SetActive(targetCamera == thirdPersonCamera);
        firstPersonCamera.gameObject.SetActive(targetCamera == firstPersonCamera);
        mouseOrbitCamera.gameObject.SetActive(targetCamera == mouseOrbitCamera);

        Debug.Log($"{targetCamera.name} activada.");
    }

    private void ControlMouseOrbitCamera()
    {
        // Obtener movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * orbitSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * orbitSensitivity * Time.deltaTime;

        // Ajustar las rotaciones orbitales basadas en el movimiento del mouse
        orbitYRotation += mouseX;
        orbitXRotation -= mouseY;

        // Limitar el ángulo vertical para evitar que la cámara se invierta
        orbitXRotation = Mathf.Clamp(orbitXRotation, -35f, 60f);

        // Calcular la rotación de la cámara basada en los valores acumulados
        Quaternion rotation = Quaternion.Euler(orbitXRotation, orbitYRotation, 0f);

        // Calcular la posición de la cámara usando la rotación y la distancia al jugador
        Vector3 offset = rotation * new Vector3(0, 1.5f, -orbitDistance); // Subir 1.5 en Y
        mouseOrbitCamera.transform.position = playerBody.position + offset;

        // Apuntar la cámara hacia el jugador
        mouseOrbitCamera.transform.LookAt(playerBody);
    }

}
