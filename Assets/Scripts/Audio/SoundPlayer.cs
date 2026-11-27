using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// BGM と SE を鳴らす係（仕様書 v0.2 の7章・デモ v0.27）。
/// Resources/SoundPlayer のプレハブから、ゲームが始まったときに自動で1つだけ作られ、シーンをまたいで残る（シーンに置かなくてよい）。
/// シーンが切り替わるたびに、そのシーンの GameManager を探して、起動・電源オフ・エンディングに合わせて BGM を鳴らす・止める。
///
/// BGM のきまり：
/// ・タイトル：無音（タイトルに戻ったら静かに消す）
/// ・起動したとき（「もう一度起動する」も）：最初から鳴り始める
/// ・電池0%：余韻なしで、ぷつんと止まる
/// ・True End：止めずに鳴り続ける（Result シーンに移っても切らない）
/// ・Memo End・Normal End：3秒かけて消える
/// ・「スマートフォンを閉じる」：2秒かけて消える（その画面から SoundPlayer.FadeOutBgm(2f) を呼ぶ）
///
/// 音のファイルは、プレハブの枠（Bgm Clip・Tap Clip など）に入れるだけ。空なら鳴らないだけで、エラーにはならない
/// </summary>
public class SoundPlayer : MonoBehaviour
{
    private const string ResourcePath = "SoundPlayer";
    private const string TitleSceneName = "Title";
    private const float TitleFadeSeconds = 0.6f;
    private const float EndingFadeSeconds = 3f;
    private const float PreviewFadeSeconds = 0.5f;

    [Header("音を流す場所（AudioMixer の BGM / SE グループにつないである）")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _seSource;
    [SerializeField] private AudioMixer _mixer;

    [Header("BGM（届いたらここに入れる）")]
    [SerializeField] private AudioClip _bgmClip;

    [Header("SE（届いたらここに入れる）")]
    [SerializeField] private AudioClip _tapClip;
    [SerializeField] private AudioClip _deleteClip;
    [SerializeField] private AudioClip _sendClip;

    private static SoundPlayer _instance;

    private GameManager _gameManager;
    private bool _isPreviewing;

    /// <summary>
    /// ゲームが始まったら、Resources のプレハブから1つだけ作る
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateOnStart()
    {
        SoundPlayer prefab = Resources.Load<SoundPlayer>(ResourcePath);

        if (prefab == null)
        {
            Debug.LogWarning("[SoundPlayer] Resources/SoundPlayer のプレハブがないので、音は鳴りません（Tools → NoSubject → 音の仕組みを作る）");
            return;
        }

        SoundPlayer player = Instantiate(prefab);
        player.name = ResourcePath;
        DontDestroyOnLoad(player.gameObject);
    }

    /// <summary>
    /// 自分を覚えて、シーンが切り替わるのを受け取れるようにする
    /// </summary>
    private void Awake()
    {
        _instance = this;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    /// <summary>
    /// 保存されている音量を Mixer に反映する（Mixer の値は Awake では反映されないことがあるので Start で）
    /// </summary>
    private void Start()
    {
        SoundSettings.Apply(_mixer);
    }

    /// <summary>
    /// 登録を解除する
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnwatchGameManager();
    }

    // ───────── どこからでも呼べる操作 ─────────

    /// <summary>
    /// BGM を最初から鳴らす
    /// </summary>
    public static void PlayBgm()
    {
        if (_instance == null || _instance._bgmClip == null)
        {
            return;
        }

        AudioSource source = _instance._bgmSource;
        source.DOKill();
        source.clip = _instance._bgmClip;
        source.volume = 1f;
        source.Play();
        _instance._isPreviewing = false;
    }

    /// <summary>
    /// BGM を、余韻なしですぐに止める（電池0%）
    /// </summary>
    public static void CutBgm()
    {
        if (_instance == null)
        {
            return;
        }

        _instance._bgmSource.DOKill();
        _instance._bgmSource.Stop();
        _instance._isPreviewing = false;
    }

    /// <summary>
    /// BGM を、指定した秒数で静かに消す（エンディング・「スマートフォンを閉じる」など）
    /// </summary>
    public static void FadeOutBgm(float seconds)
    {
        if (_instance == null || !_instance._bgmSource.isPlaying)
        {
            return;
        }

        AudioSource source = _instance._bgmSource;
        source.DOKill();
        source.DOFade(0f, seconds).OnComplete(source.Stop).SetLink(_instance.gameObject);
        _instance._isPreviewing = false;
    }

    /// <summary>
    /// SE を1回鳴らす
    /// </summary>
    public static void PlaySe(SeKind kind)
    {
        if (_instance == null)
        {
            return;
        }

        AudioClip clip = _instance.GetSeClip(kind);

        if (clip != null)
        {
            _instance._seSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 設定画面で BGM の音量を変えたとき：止まっていれば試しに鳴らす（0 にしたら静かに止める）
    /// </summary>
    public static void PreviewBgm(float volume)
    {
        if (_instance == null)
        {
            return;
        }

        if (volume <= 0f)
        {
            FadeOutBgm(0.25f);
            return;
        }

        if (!_instance._bgmSource.isPlaying)
        {
            PlayBgm();
            _instance._isPreviewing = true;
        }
    }

    /// <summary>
    /// 設定画面を閉じたとき：試しに鳴らしていた BGM だけを静かに止める
    /// </summary>
    public static void EndPreview()
    {
        if (_instance != null && _instance._isPreviewing)
        {
            FadeOutBgm(PreviewFadeSeconds);
        }
    }

    // ───────── シーンと GameManager に合わせる ─────────

    /// <summary>
    /// シーンが切り替わったら：タイトルなら BGM を消す。GameManager があれば、その知らせを受け取る
    /// </summary>
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == TitleSceneName)
        {
            FadeOutBgm(TitleFadeSeconds);
        }

        UnwatchGameManager();
        _gameManager = FindAnyObjectByType<GameManager>();

        if (_gameManager != null)
        {
            _gameManager.OnBootStarted += PlayBgm;
            _gameManager.OnPowerOffStarted += CutBgm;
            _gameManager.OnGameEnded += HandleGameEnded;
        }
    }

    /// <summary>
    /// 前のシーンの GameManager の知らせを受け取るのをやめる
    /// </summary>
    private void UnwatchGameManager()
    {
        if (_gameManager == null)
        {
            return;
        }

        _gameManager.OnBootStarted -= PlayBgm;
        _gameManager.OnPowerOffStarted -= CutBgm;
        _gameManager.OnGameEnded -= HandleGameEnded;
        _gameManager = null;
    }

    /// <summary>
    /// エンディングに入ったとき：True End は鳴らし続け、ほかは3秒で消す
    /// </summary>
    private void HandleGameEnded(EndingType ending)
    {
        if (ending != EndingType.True)
        {
            FadeOutBgm(EndingFadeSeconds);
        }
    }

    /// <summary>
    /// SE の種類に合う音を返す
    /// </summary>
    private AudioClip GetSeClip(SeKind kind)
    {
        switch (kind)
        {
            case SeKind.Delete:
                return _deleteClip;
            case SeKind.Send:
                return _sendClip;
            default:
                return _tapClip;
        }
    }
}
