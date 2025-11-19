using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    public Image fadeImage;
    public float fadeSpeed = 1.5f;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeOut(string scene)
    {
        float a = 0f;

        while (a < 1f)
        {
            a += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }

        SceneManager.LoadScene(scene);
    }
}
