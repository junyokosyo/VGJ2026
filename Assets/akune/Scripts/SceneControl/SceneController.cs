using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [Header("シーン遷移時のフェード用オブジェクト")]
    [SerializeField] private Image _fadeImage;
    [Header("フェードが完了する秒数")]
    [SerializeField] private float _fadeDuration = 1.0f;

    private Scene _currentScene = Scene.Title;

    private bool _isTransitioning = false;

    private static SceneController _instance;
    public static SceneController Instance => _instance;

    public bool IsTransitioning => _isTransitioning;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.aKey.wasPressedThisFrame)
        {
            SceneTransition(Scene.InGame, true);
        }
#endif
    }

    public void SceneTransition(Scene nextScene, bool onFade)
    {
        if (_isTransitioning) return;
        if (_currentScene == nextScene) return;

        StartCoroutine(Fade(nextScene, onFade));
    }

    /// <summary>
    /// Fade用コルーチン,第2引数がfalseならfadeせず切り替え
    /// </summary>
    private IEnumerator Fade(Scene nextScene, bool onFade)
    {
        _isTransitioning = true;

        bool useFade = onFade && _fadeImage != null;

        if (useFade) yield return FadeAlpha(0f, 1f);

        SceneManager.LoadScene((int)nextScene);
        _currentScene = nextScene;

        if (useFade) yield return FadeAlpha(1f, 0f);

        _isTransitioning = false;
    }

    private IEnumerator FadeAlpha(float startAlpha, float endAlpha)
    {
        Color color = _fadeImage.color;

        _fadeImage.raycastTarget = true;

        for (float t = 0f; t < _fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, t / _fadeDuration);
            _fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        _fadeImage.color = color;

        _fadeImage.raycastTarget = endAlpha > 0f;
    }
}

public enum Scene
{
    Title, InGame, Result
}
