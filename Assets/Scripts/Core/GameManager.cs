using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// ゲーム全体の流れを管理する（仕様書 v0.2）。
/// 起動 → 40秒で電源オフ →「もう一度起動する」で再起動（やり直し）→ …をくり返し、Messaging の返事でエンディングへ進む。
/// ・やり直し（電池切れ）：進行状況も暗証番号もそのまま。ホーム画面から再開し、再起動の回数が1増える
/// ・新しいゲーム（Title から InGame に入ったとき）：進行状況を消し、暗証番号を抽選し直す
/// ・「最初からやり直す」「はじめから遊ぶ」：Title へ戻る（次に InGame に入ったときに新しいゲームになる）
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Timer _timer;

    [Header("シーンを開いたらすぐ起動するか（Title から来たときは、スマホが上がりきった状態で始まる）")]
    [SerializeField] private bool _isBootOnStart = true;

    [Header("演出の待ち時間（秒）")]
    [SerializeField] private float _bootDelay = 0.5f;
    [SerializeField] private float _powerOffDelay = 0.64f;
    [SerializeField] private float _endingDelay = 1.0f;

    private PhoneState _currentState = PhoneState.PoweredOff;
    private bool _isPausedByRequest;
    private bool _isPausedByApplication;

    /// <summary>
    /// 今のスマホの状態
    /// </summary>
    public PhoneState CurrentState => _currentState;

    /// <summary>
    /// 起動の演出が始まったとき
    /// </summary>
    public event Action OnBootStarted;

    /// <summary>
    /// 起動が終わり操作できるようになったとき（ホーム画面を出す）
    /// </summary>
    public event Action OnBooted;

    /// <summary>
    /// 40秒たって電源が落ちる演出が始まったとき（ブラウン管の演出・BGM をぷつんと止める）
    /// </summary>
    public event Action OnPowerOffStarted;

    /// <summary>
    /// 電源が落ちきったとき（電池切れの画面を出す）
    /// </summary>
    public event Action OnPoweredOff;

    /// <summary>
    /// エンディングに入ったとき（引数：エンディングの種類）
    /// </summary>
    public event Action<EndingType> OnGameEnded;

    /// <summary>
    /// 保存データを読み込む（なければ新しいゲームになる）
    /// </summary>
    private void Awake()
    {
        GameProgress.Load();
    }

    /// <summary>
    /// 時間切れを受け取れるようにして、1フレーム待ってから起動する
    /// （ほかのスクリプトが Start でイベント登録を終えるのを待つため。暗証番号の抽選も、登録が終わってから頼む）
    /// </summary>
    private IEnumerator Start()
    {
        _timer.OnTimeOver += HandleTimeOver;
        yield return null;

        if (GameProgress.ConsumeNewGameRequest())
        {
            GameProgress.NewGame();
        }

        if (_isBootOnStart)
        {
            StartFromTitle();
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
    /// アプリが裏に回ったら時間を止める（仕様書 3-1「ページが裏に回っているあいだ」）。
    /// WebGL ではタブが裏に回ると Unity 自体の更新が止まるので、そちらでも時間は進まない
    /// </summary>
    private void OnApplicationPause(bool isPaused)
    {
        _isPausedByApplication = isPaused;
        ApplyTimerState();
    }

    /// <summary>
    /// 最初の起動（Title から来たとき・シーンを直接開いたとき）。再起動の回数は増えない
    /// </summary>
    public void StartFromTitle()
    {
        if (_currentState != PhoneState.PoweredOff)
        {
            return;
        }

        StartCoroutine(BootRoutine());
    }

    /// <summary>
    /// 電池切れの画面の「もう一度起動する」から呼ぶ。再起動の回数が1増える
    /// </summary>
    public void Boot()
    {
        if (_currentState != PhoneState.PoweredOff)
        {
            return;
        }

        GameProgress.AddReboot();
        StartCoroutine(BootRoutine());
    }

    /// <summary>
    /// 40秒の時間制限を一時停止・再開する（制作ツールの「時間制限を止める」など）
    /// </summary>
    public void SetTimerPaused(bool isPaused)
    {
        _isPausedByRequest = isPaused;
        ApplyTimerState();
    }

    /// <summary>
    /// エンディングへ進む。時間はここで止まる（仕様書 3-1「スマホの外の場面」）。
    /// Normal End は「何も分からなかった。」を選んだとき、True / Memo End は番号を送ったあと（GameProgress.SetSentNumber の後）に呼ぶ
    /// </summary>
    public void EndGame(EndingType ending)
    {
        if (_currentState == PhoneState.Ended || ending == EndingType.None)
        {
            return;
        }

        StopAllCoroutines();
        _currentState = PhoneState.Ended;
        ApplyTimerState();

        GameProgress.SetEnding(ending);
        GameProgress.SetFlag(ProgressFlag.Ended);
        OnGameEnded?.Invoke(ending);

        StartCoroutine(LoadSceneAfterDelay(GameScene.Result, _endingDelay));
    }

    /// <summary>
    /// 「最初からやり直す」→ 確認で「やり直す」、記録画面の「はじめから遊ぶ」から呼ぶ。
    /// Title へ戻る（進行状況の消去と暗証番号の抽選は、次に InGame に入ったときに行われる）
    /// </summary>
    public void ReturnToTitle()
    {
        StopAllCoroutines();
        _currentState = PhoneState.Ended;
        ApplyTimerState();

        SceneController.Load(GameScene.Title);
    }

    /// <summary>
    /// 起動の演出を待ってから、40秒のカウントを最初から始める
    /// </summary>
    private IEnumerator BootRoutine()
    {
        _currentState = PhoneState.Booting;
        OnBootStarted?.Invoke();

        yield return new WaitForSeconds(_bootDelay);

        _currentState = PhoneState.Running;
        _timer.ResetTimer();
        ApplyTimerState();
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
    /// 電源が落ちる演出（ブラウン管）を待ってから、電池切れの画面にする（進行状況は消さない）
    /// </summary>
    private IEnumerator PowerOffRoutine()
    {
        _currentState = PhoneState.PoweringOff;
        ApplyTimerState();
        OnPowerOffStarted?.Invoke();

        yield return new WaitForSeconds(_powerOffDelay);

        _currentState = PhoneState.PoweredOff;
        OnPoweredOff?.Invoke();
    }

    /// <summary>
    /// 状態と一時停止の理由から、タイマーを動かすか決める
    /// </summary>
    private void ApplyTimerState()
    {
        bool isActive = _currentState == PhoneState.Running && !_isPausedByRequest && !_isPausedByApplication;
        _timer.SetTimerIsActive(isActive);
    }

    /// <summary>
    /// 指定した秒数を待ってからシーンを切り替える
    /// </summary>
    private IEnumerator LoadSceneAfterDelay(GameScene scene, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneController.Load(scene);
    }
}
