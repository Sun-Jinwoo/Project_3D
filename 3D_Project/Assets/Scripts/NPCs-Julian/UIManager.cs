using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject WinScreen;

    public void ShowWinScreen()
    {
         WinScreen.SetActive(true);
    }
}
