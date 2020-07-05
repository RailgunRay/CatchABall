using UnityEngine;

public class GraphicsManager : MonoBehaviour {

    public static GraphicsManager instance;
    public UserPrefsCollection pencilCollection;
    public UserPrefsCollection officeCollection;

    [HideInInspector]
    public UserPrefsCollection currentlySelectedUserPrefsCollection;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        currentlySelectedUserPrefsCollection = pencilCollection;
    }
}


