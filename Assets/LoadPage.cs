using UnityEngine;

public class LoadPage : MonoBehaviour
{
    public static LoadPage Instance
    {
        get => FindObjectOfType<LoadPage>();
    }

    [SerializeField] string HomeString;
    [SerializeField] GameObject loadingGo;

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
        loadingGo.SetActive(true);
    }
}
