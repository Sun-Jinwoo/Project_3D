using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform player;

    void Start()
    {
        if (spawnPoint != null && player != null)
            player.position = spawnPoint.position;
    }
}
