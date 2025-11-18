using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    public float minX = -200f; // Límite izquierdo en coordenadas del Canvas
    public float maxX = 200f;  // Límite derecho en coordenadas del Canvas
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        // Obtener la posición del cursor en coordenadas del mundo
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out mousePos
        );

        // Mantener la posición Y actual y actualizar solo la X
        Vector3 newPos = rectTransform.anchoredPosition;
        newPos.x = Mathf.Clamp(mousePos.x, minX, maxX);
        rectTransform.anchoredPosition = newPos;
    }
}