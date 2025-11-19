using UnityEngine;
using UnityEngine.UI;

public class GlassBreakMinigame : MonoBehaviour
{
    [SerializeField] private RectTransform movingBar; // Barra móvil (indicador)
    [SerializeField] private RectTransform successZone; // Zona de éxito
    [SerializeField] private Slider progressBar; // Barra de progreso
    [SerializeField] private float barSpeed = 200f; // Velocidad de movimiento de la barra móvil
    [SerializeField] private float successZoneSpeed = 100f; // Velocidad de movimiento de la zona de éxito
    [SerializeField] private float progressSpeed = 0.5f; // Velocidad de llenado del progreso
    [SerializeField] private float regressSpeed = 0.2f; // Velocidad de disminución del progreso
    [SerializeField] private float barMinX; // Límite izquierdo de la barra estática
    [SerializeField] private float barMaxX; // Límite derecho de la barra estática

    private float progress = 0f;
    private bool isGameOver = false;
    private float targetX; // Posición objetivo para la zona de éxito
    private float timeSinceLastTarget = 0f;
    private float changeTargetInterval = 1f; // Intervalo para cambiar el objetivo aleatorio

    void Start()
    {
        // Inicializar la barra de progreso
        progressBar.value = 0f;
        // Asegurarse de que la barra móvil esté dentro de los límites al inicio
        movingBar.anchoredPosition = new Vector2(barMinX, movingBar.anchoredPosition.y);
        // Establecer el primer objetivo aleatorio para la zona de éxito
        SetNewRandomTarget();
    }

    void Update()
    {
        if (isGameOver) return;

        // Mover la barra móvil con entrada del jugador
        float input = Input.GetAxisRaw("Horizontal"); // A/D o flechas izquierda/derecha
        Vector2 newPos = movingBar.anchoredPosition + new Vector2(input * barSpeed * Time.deltaTime, 0);
        newPos.x = Mathf.Clamp(newPos.x, barMinX, barMaxX);
        movingBar.anchoredPosition = newPos;

        // Mover la zona de éxito hacia el objetivo aleatorio
        MoveSuccessZone();

        // Verificar si la barra móvil está dentro de la zona de éxito
        bool isInSuccessZone = IsBarInSuccessZone();

        // Actualizar progreso
        if (isInSuccessZone)
        {
            progress += progressSpeed * Time.deltaTime;
        }
        else
        {
            progress -= regressSpeed * Time.deltaTime;
        }

        // Limitar el progreso entre 0 y 1
        progress = Mathf.Clamp01(progress);
        progressBar.value = progress;

        // Verificar si se completó el minijuego
        if (progress >= 1f)
        {
            isGameOver = true;
            Debug.Log("¡Cristal roto! Ganaste.");
            // Aquí puedes activar animación de cristal roto, sonido, etc.
        }
    }

    void MoveSuccessZone()
    {
        // Actualizar el temporizador para cambiar el objetivo
        timeSinceLastTarget += Time.deltaTime;
        if (timeSinceLastTarget >= changeTargetInterval)
        {
            SetNewRandomTarget();
            timeSinceLastTarget = 0f;
        }

        // Mover la zona de éxito hacia el objetivo
        Vector2 currentPos = successZone.anchoredPosition;
        float newX = Mathf.MoveTowards(currentPos.x, targetX, successZoneSpeed * Time.deltaTime);

        // Asegurarse de que la zona de éxito no se salga de los límites
        float halfWidth = successZone.sizeDelta.x / 2;
        newX = Mathf.Clamp(newX, barMinX + halfWidth, barMaxX - halfWidth);
        successZone.anchoredPosition = new Vector2(newX, currentPos.y);
    }

    void SetNewRandomTarget()
    {
        // Calcular los límites considerando el ancho de la zona de éxito
        float halfWidth = successZone.sizeDelta.x / 2;
        float minTarget = barMinX + halfWidth;
        float maxTarget = barMaxX - halfWidth;
        targetX = Random.Range(minTarget, maxTarget);
        // Ajustar el intervalo para que el movimiento sea más o menos frecuente
        changeTargetInterval = Random.Range(0.5f, 1.5f);
    }

    bool IsBarInSuccessZone()
    {
        // Obtener los límites de la zona de éxito (dinámicos)
        float successMinX = successZone.anchoredPosition.x - successZone.sizeDelta.x / 2;
        float successMaxX = successZone.anchoredPosition.x + successZone.sizeDelta.x / 2;

        // Obtener la posición de la barra móvil
        float barX = movingBar.anchoredPosition.x;

        // Verificar si la barra móvil está dentro de los límites de la zona de éxito
        return barX >= successMinX && barX <= successMaxX;
    }

    // Método para reiniciar el minijuego
    public void ResetGame()
    {
        progress = 0f;
        progressBar.value = 0f;
        movingBar.anchoredPosition = new Vector2(barMinX, movingBar.anchoredPosition.y);
        SetNewRandomTarget();
        timeSinceLastTarget = 0f;
        isGameOver = false;
    }
}