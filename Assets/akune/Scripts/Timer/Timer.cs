using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Timer : MonoBehaviour
{
    [Header("バッテリー残量のイメージ")]
    [SerializeField] Battery _battery;

    [Header("タイマーのテキスト")]
    [SerializeField] TextMeshProUGUI _timerText;

    [Header("スマホが消える秒数")]
    [SerializeField] float _gameOverTime = 30.0f;

    private float _timer = 0.0f;
    private bool _timerIsActive = false;

    /// <summary>
    /// 時間切れになったときに1回だけ呼ばれる（GameManager が受け取って電源を落とす）
    /// </summary>
    public event Action OnTimeOver;

    void Start()
    {
        //インスペクターで設定されたタイムに設定
        _timer = _gameOverTime;
    }

    void Update()
    {
        //アクティブかつ0以上ならカウント
        if (_timerIsActive && _timer > 0)
        {
            _timer -= Time.deltaTime;
            //秒数の割合
            var percent = _timer / _gameOverTime;

            _battery.UpdateBatteryLevel(percent);

            //タイムオーバーでイベント発火
            if (_timer <= 0)
            {
                _timer = 0;
                OnTimeOver?.Invoke();
            }

            //タイマーの秒数を更新
            //テキストは仕様上表示しないので、設定されているときだけ更新する（デバッグ用）
            if (_timerText != null)
            {
                _timerText.text = $"Timer:{Mathf.CeilToInt(_timer)}";
            }
        }

#if UNITY_EDITOR  //テスト用
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SetTimerIsActive(true);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SetTimerIsActive(false);
        }
#endif
    }

    public void SetTimerIsActive(bool value)
    {
        _timerIsActive = value;
    }

    /// <summary>
    /// 残り時間とバッテリー表示を満タンに戻す（再起動のときに GameManager が呼ぶ）
    /// </summary>
    public void ResetTimer()
    {
        _timer = _gameOverTime;
        _battery.ResetBattery();
    }
}
