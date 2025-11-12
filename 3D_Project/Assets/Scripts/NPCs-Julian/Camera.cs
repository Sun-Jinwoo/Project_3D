using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        SegundaPersona();
    }

    void SegundaPersona()
    {
        Vector3 position = transform.position;
        position.x = player.transform.position.x;
        position.y = player.transform.position.y + 1.5f;
    }
}
