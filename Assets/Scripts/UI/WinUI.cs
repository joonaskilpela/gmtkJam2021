using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    public GameObject WinPanel;

    GameGrid[] grids;

    private void Start()
    {
        grids = FindObjectsOfType<GameGrid>();
    }

    private void Update()
    {
        // If win panel is active
        if (WinPanel.activeSelf)
        {
            // Load next level if space is pressed
            if (Input.GetKeyDown(KeyCode.Space)) LoadNextLevel();
        }
    }

    private void FixedUpdate()
    {
        // If winpanel is active, dont continue to check grids
        if (WinPanel.activeSelf) return;

        // Check that all grids have reached the flag
        if (grids.Any(g => !g.FlagReached)) return;

        // Activate winpanel
        WinPanel.SetActive(true);
    }

    public void LoadNextLevel()
    {
        // Load the next scene in build index order
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
