using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム全体の流れを管理する。
/// 起動 → 30秒で電源オフ → 電源ボタンで再起動（やり直し）→ …をくり返し、調査を終えたらエンディングへ進む。
/// ・やり直し（電池切れ）：進行状況も暗号もそのまま。ロック画面から再開
/// ・リセット（ゲーム終了・タイトルへ戻る）：次に InGame が始まるときに進行状況を消し、暗号を抽選し直す
/// </summary>
public class GameManager : MonoBehaviour
{
    private const string EndingSceneName = "Result";
    private const string TitleSceneName = "Title";

    [Header("参照")]
    [SerializeField] private Timer _timer;

    [Header("ゲーム開始と同時に起動するか（false なら電源ボタン待ちから始まる）")]
    [SerializeField] private bool _isBootOnStart = true;

    [Header("演出の待ち時間（秒）")]
    [SerializeField] private float _bootDelay = 0.5f;
    [SerializeField] private float _powerOffDelay = 1.0f;
    [SerializeField] private float _endingDelay = 1.0f;

    private PhoneState _currentState = PhoneState.PoweredOff;

    /// <summary>
    /// 今のスマホの状態
    /// </summary>
    public PhoneState CurrentState => _currentState;

    /// <summary>
    /// 起動の演出が始まったとき（起動ロゴなどを出す）
    /// </summary>
    public event Action OnBootStarted;

    /// <summary>
    /// 起動が終わり操作できるようになったとき（ロック画面を出す）
    /// </summary>
    public event Action OnBooted;

    /// <summary>
    /// 30秒たって電源が落ちる演出が始まったとき
    /// </summary>
    public event Action OnPowerOffStarted;

    /// <summary>
    /// 電源が落ちきったとき（電源ボタンの画面を出す）
    /// </summary>
    public event Action OnPoweredOff;

    /// <summary>
    /// 調査を終えたとき（引数：到達したエンディング）
    /// </summary>
    public event Action<EndingType> OnGameEnded;

    /// <summary>
    /// 新しいゲームとしてリセットする（進行状況を消し、OnReset で暗号の抽選などを促す）。
    /// ゲーム終了時やタイトルへ戻るときではなく、ここ（次のゲームの開始時）で消す。
    /// 終了時に消すと、Result シーンでエンディングの種類が読めなくなるため
    /// </summary>
    private void Awake()
    {
        GameProgress.Reset();
    }

    /// <summary>
    /// 時間切れを受け取れるようにして、1フレーム待ってから起動する
    /// （ほかのスクリプトが Start でイベント登録を終えるのを待つため）
    /// </summary>
    private IEnumerator Start()
    {
        _timer.OnTimeOver += HandleTimeOver;
        yield return null;

        if (_isBootOnStart)
        {
            Boot();
        }
        else
        {
            OnPoweredOff?.Invoke();
        }
    }

    /// <summary>
    /// シーンを抜けるときに登録を解除する
    /// </summary>
    private void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.OnTimeOver -= HandleTimeOver;
        }
    }

    /// <summary>
    /// スマホを起動する。電源ボタンから呼ぶ。電源が切れているときだけ動く
    /// </summary>
    public void Boot()
    {
        if (_currentState != PhoneState.PoweredOff)
        {
            return;
        }

        StartCoroutine(BootRoutine());
    }

    /// <summary>
    /// 30秒の時間制限を一時停止・再開する。
    /// どの操作中に止めるかは未定なので、決まったら各アプリからこれを呼ぶ
    /// </summary>
    public void SetTimerPaused(bool isPaused)
    {
        if (_currentState != PhoneState.Running)
        {
            return;
        }

        _timer.SetTimerIsActive(!isPaused);
    }

    /// <summary>
    /// 調査を終えてエンディングへ進む（鳥を妻に託す、調査を終える、などから呼ぶ）
    /// </summary>
    public void EndGame()
    {
        if (_currentState == PhoneState.Ended)
        {
            return;
        }

        StopAllCoroutines();
        _currentState = PhoneState.Ended;
        _timer.SetTimerIsActive(false);

        EndingType ending = EndingJudge.Judge();
        GameProgress.SetEnding(ending);
        OnGameEnded?.Invoke(ending);

        StartCoroutine(LoadEndingScene());
    }

    /// <summary>
    /// ゲームを中断してタイトルへ戻る（メニューの「タイトルへ」などから呼ぶ）。
    /// 進行状況は、次にゲームを始めたときにリセットされる
    /// </summary>
    public void ReturnToTitle()
    {
        StopAllCoroutines();
        _currentState = PhoneState.Ended;
        _timer.SetTimerIsActive(false);

        // TODO: SceneController（フェードつき）が完成したら、そちら経由に差し替える
        SceneManager.LoadScene(TitleSceneName);
    }

    /// <summary>
    /// 起動の演出を待ってから、30秒のカウントを最初から始める
    /// </summary>
    private IEnumerator BootRoutine()
    {
        _currentState = PhoneState.Booting;
        GameProgress.CountBoot();
        OnBootStarted?.Invoke();

        yield return new WaitForSeconds(_bootDelay);

        _currentState = PhoneState.Running;
        _timer.ResetTimer();
        _timer.SetTimerIsActive(true);
        OnBooted?.Invoke();
    }

    /// <summary>
    /// 時間切れになったら電源を落とす
    /// </summary>
    private void HandleTimeOver()
    {
        if (_currentState != PhoneState.Running)
        {
            return;
        }

        StartCoroutine(PowerOffRoutine());
    }

    /// <summary>
    /// 電源が落ちる演出を待ってから、電源ボタン待ちにする（進行状況は消さない）
    /// </summary>
    private IEnumerator PowerOffRoutine()
    {
        _currentState = PhoneState.PoweringOff;
        _timer.SetTimerIsActive(false);
        OnPowerOffStarted?.Invoke();

        yield return new WaitForSeconds(_powerOffDelay);

        _currentState = PhoneState.PoweredOff;
        OnPoweredOff?.Invoke();
    }

    /// <summary>
    /// 演出を待ってからエンディングのシーンへ切り替える
    /// </summary>
    private IEnumerator LoadEndingScene()
    {
        yield return new WaitForSeconds(_endingDelay);

        // TODO: SceneController（フェードつき）が完成したら、そちら経由に差し替える
        SceneManager.LoadScene(EndingSceneName);
    }
}
