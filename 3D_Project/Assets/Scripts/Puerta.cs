using UnityEngine;

public class Puerta : MonoBehaviour
{
    public Transform doorModel;
    public float openAngle = 90f;
    public float speed = 3f;
    public float interactDistance = 2f;

    bool isOpen;
    bool isMoving;

    Transform player;

    void Start()
    {
        player = Object.FindFirstObjectByType<Ninja>().transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (!isMoving)
            {
                isOpen = !isOpen;
                StopAllCoroutines();
                StartCoroutine(MoveDoor());
            }
        }
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

    internal void Abrir()
    {
        throw new System.NotImplementedException();
    }
}
