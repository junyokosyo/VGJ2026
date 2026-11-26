using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneController : MonoBehaviour
{
    //[Header("シーン遷移時のフェード用オブジェクト")]
    //[SerializeField] private Image _fadeImage;
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

    public void SceneTransition(Scene nextScene)
    {
        if (_currentScene == nextScene) return;
        SceneManager.LoadScene((int)nextScene);
    }
}

public enum Scene
{
    Title, InGame, Result
}