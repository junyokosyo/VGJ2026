using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// シーンの切り替えをフェードつきで行う。
/// 最初に開くシーン（Title）に1つ置けば、シーンをまたいで残る。フェード用の黒い画面は自分で作る。
/// 切り替えるときは SceneController.Load(GameScene.InGame) のように呼ぶ（置いていないシーンから呼んでもフェードなしで切り替わる）
/// </summary>
public class SceneController : MonoBehaviour
{
    private const int FadeSortingOrder = 1000;

    [Header("フェードの色")]
    [SerializeField] private Color _fadeColor = Color.black;

    [Header("暗くなる・明るくなる それぞれの秒数")]
    [SerializeField] private float _fadeDuration = 0.4f;

    private static SceneController _instance;

    private Image _fadeImage;
    private bool _isTransitioning;

    /// <summary>
    /// シーンに置かれている SceneController（なければ null）
    /// </summary>
    public static SceneController Instance => _instance;

    /// <summary>
    /// シーンを切り替えている途中か
    /// </summary>
    public bool IsTransitioning => _isTransitioning;

    /// <summary>
    /// 1つだけ残るようにして、フェード用の画面を作る
    /// </summary>
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        CreateFadeImage();
    }

    /// <summary>
    /// シーンを切り替える。SceneController があればフェードつき、なければそのまま切り替える
    /// </summary>
    public static void Load(GameScene nextScene)
    {
        if (_instance != null)
        {
            _instance.SceneTransition(nextScene, true);
            return;
        }

        SceneManager.LoadScene(nextScene.ToString());
    }

    /// <summary>
    /// シーンを切り替える（切り替えの途中なら何もしない）
    /// </summary>
    public void SceneTransition(GameScene nextScene, bool useFade)
    {
        if (_isTransitioning)
        {
            return;
        }

        StartCoroutine(Transition(nextScene.ToString(), useFade));
    }

    /// <summary>
    /// 暗くする → シーンを読み込む → 明るくする
    /// </summary>
    private IEnumerator Transition(string sceneName, bool useFade)
    {
        _isTransitioning = true;
        _fadeImage.raycastTarget = true;

        if (useFade)
        {
            yield return _fadeImage.DOFade(1f, _fadeDuration).SetUpdate(true).WaitForCompletion();
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }

        if (useFade)
        {
            yield return _fadeImage.DOFade(0f, _fadeDuration).SetUpdate(true).WaitForCompletion();
        }

        _fadeImage.raycastTarget = false;
        _isTransitioning = false;
    }

    /// <summary>
    /// 一番手前に出る、画面いっぱいの透明な画像を作る（シーンをまたいで残るよう、自分の子にする）
    /// </summary>
    private void CreateFadeImage()
    {
        GameObject canvasObject = new GameObject("FadeCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = FadeSortingOrder;

        GameObject imageObject = new GameObject("FadeImage", typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(canvasObject.transform, false);

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        _fadeImage = imageObject.GetComponent<Image>();
        Color color = _fadeColor;
        color.a = 0f;
        _fadeImage.color = color;
        _fadeImage.raycastTarget = false;
    }
}
