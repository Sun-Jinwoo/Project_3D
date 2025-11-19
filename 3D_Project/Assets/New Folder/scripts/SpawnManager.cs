using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints; // Array de puntos de spawn
    public GameObject player; // Referencia al GameObject del jugador

    void Start()
    {
        // Elegir un punto de spawn aleatorio
        int randomIndex = Random.Range(0, spawnPoints.Length);
        player.transform.position = spawnPoints[randomIndex].position;
    }
}