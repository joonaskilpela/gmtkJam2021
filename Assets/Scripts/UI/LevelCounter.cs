using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCounter : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;

    private void OnEnable()
    {
        text.text = $"Level {SceneManager.GetActiveScene().buildIndex}";
    }
}
