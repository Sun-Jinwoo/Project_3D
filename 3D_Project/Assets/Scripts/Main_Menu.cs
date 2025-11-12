using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;

    public void OpenOptionsPanel()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    public void PlayGame()
    {
        SceneManager.LoadScene("02_Robo_Museo");
    }
}
