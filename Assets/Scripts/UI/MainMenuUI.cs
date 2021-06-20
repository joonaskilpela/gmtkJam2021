using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        // Load first scene
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
#if !UNITY_WEBGL
        // Quit the application
        Application.Quit();
#endif
    }
}
