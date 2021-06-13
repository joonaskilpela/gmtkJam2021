using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic _inst;

    // Start is called before the first frame update
    void Start()
    {
        if (_inst != null)
        {
            Destroy(gameObject);
            return;
        }

        _inst = this;
        DontDestroyOnLoad(gameObject);
    }
}
