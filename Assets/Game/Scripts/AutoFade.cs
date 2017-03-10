using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AutoFade : MonoBehaviour
{
    private static AutoFade instance = null;
    public Material mat;
    private Mesh mesh;
    private string levelName = "";
    private int levelIndex = 0;

    private float fadeAlpha = 0;
    private Color fadeColor = Color.black;
    private bool isFading = false;

    private static AutoFade Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (new GameObject("AutoFade")).AddComponent<AutoFade>();
            }
            return instance;
        }
    }
    public static bool Fading
    {
        get { return Instance.isFading; }
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
        mat = new Material(Shader.Find("UI/Default"));
        mesh = new Mesh();
    }

    public void OnGUI()
    {
        if (this.isFading)
        {
            this.fadeColor.a = this.fadeAlpha;
            GUI.color = this.fadeColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }

    private IEnumerator Fade(float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            t = Mathf.Clamp01(t + Time.deltaTime / aFadeOutTime);
            fadeAlpha = t;
            Debug.Log("fade : " + t);
        }
        if (levelName != "")
            SceneManager.LoadScene(levelName);
        else
            SceneManager.LoadScene(levelIndex);

        while (t > 0.0f)
        {
            yield return new WaitForEndOfFrame();
            t = Mathf.Clamp01(t - Time.deltaTime / aFadeInTime);
            fadeAlpha = t;
            Debug.Log("fade : " + t);
        }
        isFading = false;
    }
    private void StartFade(float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        fadeColor = aColor;
        fadeAlpha = 0;

        isFading = true;
        StartCoroutine(Fade(aFadeOutTime, aFadeInTime, aColor));
    }

    public static void LoadScene(string aLevelName, float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        if (Fading) return;
        Instance.levelName = aLevelName;
        Instance.StartFade(aFadeOutTime, aFadeInTime, aColor);
    }

    public static void LoadScene(int aLevelIndex, float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        if (Fading) return;
        Instance.levelName = "";
        Instance.levelIndex = aLevelIndex;
        Instance.StartFade(aFadeOutTime, aFadeInTime, aColor);
    }
}

