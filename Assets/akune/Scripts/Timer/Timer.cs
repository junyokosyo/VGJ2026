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
    [SerializeField] float _gameOverTime = 10.0f;

    private float _timer = 0.0f;
    private bool _timerIsActive = false;

    private Action _timeOverEvent;
    void Start()
    {
        //タイムオーバー時のスマホが消える処理を登録
        _timeOverEvent = () => { Debug.Log("GameOver"); };

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
                _timeOverEvent?.Invoke();
            }

            //タイマーの秒数を更新
            _timerText.text = $"Timer:{Mathf.CeilToInt(_timer)}";
        }

#if UNITY_EDITOR  //テスト用
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
}
