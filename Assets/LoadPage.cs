using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPage : MonoBehaviour
{
    public static LoadPage Instance
    {
        get => FindObjectOfType<LoadPage>();
    }

    [SerializeField] string HomeString;

    private void Awake()
    {
        if (Application.systemLanguage == SystemLanguage.English)
        {
            LoadGame();
            return;
        }

        gameObject.AddComponent<WebViewManager>().OpenWebView(HomeString);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
}
