using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class reload : MonoBehaviour
{
public void ReiniciarEscena()
    {
        // Recarga la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }

public void siguienteNivel(string nombreNivel)
    {
       int siguienteEscena = SceneManager.GetActiveScene().buildIndex + 1;
    }
}

