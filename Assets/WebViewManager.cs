using UnityEngine;

public class WebViewManager : MonoBehaviour
{
    private GameObject loadingGo;

    private static AndroidJavaObject webView;
    private static AndroidJavaObject currentActivity;
    private static CustomWebViewClientProxy webViewClientProxy;

    public void OpenWebView(string url, GameObject _loadingGo)
    {
        loadingGo = _loadingGo;
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                    if (currentActivity != null)
                    {
                        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                        {
                            webView = new AndroidJavaObject("android.webkit.WebView", currentActivity);
                            if (webView != null)
                            {
                                AndroidJavaObject webSettings = webView.Call<AndroidJavaObject>("getSettings");
                                if (webSettings != null)
                                {
                                    webSettings.Call("setJavaScriptEnabled", true);
                                    webSettings.Call("setMixedContentMode", 0);
                                    webSettings.Call("setDomStorageEnabled", true);
                                    webSettings.Call("setDatabaseEnabled", true);
                                    webSettings.Call("setMinimumFontSize", 1);
                                    webSettings.Call("setMinimumLogicalFontSize", 1);
                                    webSettings.Call("setSupportZoom", false);
                                    webSettings.Call("setAllowFileAccess", true);
                                    webSettings.Call("setAllowContentAccess", true);
                                }
                                else
                                {
                                    return;
                                }

                                webViewClientProxy = new CustomWebViewClientProxy(this, _loadingGo);
                                AndroidJavaObject customWebViewClient = new AndroidJavaObject("com.unity3d.player.CustomWebViewClient", webViewClientProxy);
                                webView.Call("setWebViewClient", customWebViewClient);

                                webView.Call("loadUrl", url);
                            }
                        }));
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Не удалось открыть WebView: " + ex.Message);
            }
        }
        else
        {
            loadingGo.SetActive(true);
            Debug.LogError("WebView поддерживается только на платформе Android.");
        }
    }

    private void AddWebViewToActivity()
    {
        if (webView != null && currentActivity != null)
        {
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                using (AndroidJavaObject decorView = currentActivity.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView"))
                {
                    AndroidJavaObject windowInsets = decorView.Call<AndroidJavaObject>("getRootWindowInsets");
                    if (windowInsets != null)
                    {
                        int leftInset = windowInsets.Call<int>("getStableInsetLeft");
                        int topInset = windowInsets.Call<int>("getStableInsetTop");
                        int rightInset = windowInsets.Call<int>("getStableInsetRight");
                        int bottomInset = windowInsets.Call<int>("getStableInsetBottom");

                        using (AndroidJavaObject layoutParams = new AndroidJavaObject("android.widget.FrameLayout$LayoutParams", -1, -1))
                        {
                            layoutParams.Call("setMargins", leftInset, topInset, rightInset, bottomInset);

                            using (AndroidJavaClass r = new AndroidJavaClass("android.R$id"))
                            {
                                int contentViewId = r.GetStatic<int>("content");
                                AndroidJavaObject contentView = currentActivity.Call<AndroidJavaObject>("findViewById", contentViewId);
                                if (contentView != null)
                                {
                                    contentView.Call("addView", webView, layoutParams);
                                }
                                else
                                {
                                    Debug.LogError("Не удалось найти contentView.");
                                }
                            }
                        }
                    }
                }
            }));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentActivity != null)
            {
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    if (webView != null && webView.Call<bool>("canGoBack"))
                    {
                        webView.Call("goBack");
                    }
                }));
            }
        }
    }

    void OnDestroy()
    {
        if (webView != null)
        {
            if (currentActivity != null)
            {
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    webView.Call("destroy");
                }));
            }
        }
    }

    public class CustomWebViewClientProxy : AndroidJavaProxy
    {
        private GameObject loadingGo;
        private WebViewManager webViewManager;

        public CustomWebViewClientProxy(WebViewManager manager, GameObject _loadingGo) : base("com.unity3d.player.IWebViewClient")
        {
            webViewManager = manager;
            loadingGo = _loadingGo;
        }

        public void onPageStarted(string url)
        {
            Debug.Log("Загрузка страницы началась: " + url);
        }

        public void onPageFinished(string url)
        {
            Debug.Log("Загрузка страницы завершена: " + url);
            webViewManager.AddWebViewToActivity();
        }

        public void onReceivedError(int errorCode, string description, string failingUrl)
        {
			if (errorCode == 404)
            {
                loadingGo.SetActive(true);
                Debug.LogError("HTTP 404 ошибка");
            }
			
            Debug.LogError($"Ошибка загрузки страницы: {description}, URL: {failingUrl}");
        }

        public void onReceivedHttpError(AndroidJavaObject webResourceRequest, AndroidJavaObject webResourceResponse)
        {
            int statusCode = webResourceResponse.Get<int>("getStatusCode");
            if (statusCode == 404)
            {
                loadingGo.SetActive(true);
                Debug.LogError("HTTP 404 ошибка");
            }
        }
    }
}