using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndUI : MonoBehaviour
{
    public void BackToMainMenu()
    {
        // Load the first scene
        SceneManager.LoadScene(0);
    }
}
