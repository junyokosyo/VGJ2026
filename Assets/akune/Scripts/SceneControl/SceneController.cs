using System.Collections;
using TMPro;
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

    private static SceneController _instance;
    void Start()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            SceneTransition(Scene.InGame, true);
        }
    }

    public void SceneTransition(Scene nextScene, bool onFade)
    {
        if (_currentScene == nextScene) return;

        StartCoroutine(Fade(nextScene, onFade));
    }

    /// <summary>
    /// Fade用コルーチン,第2引数がfalseならfadeせず切り替え
    /// </summary>
    private IEnumerator Fade(Scene nextScene, bool onFade)
    {
        if (onFade) FadeAlpha(0f, 1f);
        SceneManager.LoadScene((int)nextScene);
        if (onFade) FadeAlpha(1f, 0f);

        yield return null;
    }

    private IEnumerator FadeAlpha(float startAlpha, float endAlpha)
    {
        float fadeTimer = 0.0f;

        while (_fadeImage.color.a > 0)
        {
            var color = _fadeImage.color;
            color.a = fadeTimer / _fadeDuration;

            _fadeImage.color = color;

            fadeTimer -= Time.deltaTime;

            yield return null;
        }

        yield return null;
    }
}

public enum Scene
{
    Title, InGame, Result
}