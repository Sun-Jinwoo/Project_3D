using UnityEngine;

public class Puerta : MonoBehaviour
{
    public Transform doorModel;
    public float openAngle = -90f;
    public float speed = 3f;

    bool isOpen;
    bool isMoving;

    void Start()
    {
        if (doorModel == null) doorModel = transform;
    }

    public void Abrir()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(MoveDoor());
    }

    System.Collections.IEnumerator MoveDoor()
    {
        isMoving = true;

        Quaternion startRot = doorModel.localRotation;
        Quaternion endRot = Quaternion.Euler(0f, isOpen ? openAngle : 0f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            doorModel.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        isMoving = false;
    }
}
