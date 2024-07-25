using UnityEngine;

public class LoadPage : MonoBehaviour
{
    [SerializeField] string HomeString;
    [SerializeField] GameObject loadingGo;

    private void Start()
    {
        if(Application.systemLanguage == SystemLanguage.English)
        {
            loadingGo.SetActive(true);
            return;
        }

        gameObject.AddComponent<WebViewManager>().OpenWebView(HomeString, loadingGo);
    }
}
