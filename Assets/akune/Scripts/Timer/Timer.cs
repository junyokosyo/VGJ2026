using System;
using UnityEngine;

/// <summary>
/// 電源が落ちるまでの残り時間だけを管理する（仕様書 3-1：40秒）。
/// 見た目は持たない。電池の表示は Battery がこのクラスの値を見て自分で更新する。
/// 動かす・止める・満タンに戻すのは GameManager が行う
/// </summary>
public class Timer : MonoBehaviour
{
    [Header("電源が落ちるまでの秒数")]
    [SerializeField] private float _duration = 40.0f;

    private float _remainingTime;
    private bool _isActive;

    /// <summary>
    /// 時間切れになったときに1回だけ呼ばれる（GameManager が受け取って電源を落とす）
    /// </summary>
    public event Action OnTimeOver;

    /// <summary>
    /// 電源が落ちるまでの秒数（満タンの時間）
    /// </summary>
    public float Duration => _duration;

    /// <summary>
    /// 残りの秒数
    /// </summary>
    public float RemainingTime => _remainingTime;

    /// <summary>
    /// 残りの割合（満タン 1 → 時間切れ 0）
    /// </summary>
    public float RemainingRate => _duration > 0f ? _remainingTime / _duration : 0f;

    /// <summary>
    /// 満タンから始める
    /// </summary>
    private void Awake()
    {
        _remainingTime = _duration;
    }

    /// <summary>
    /// 動いているあいだだけ残り時間を減らし、0 になったら知らせる
    /// </summary>
    private void Update()
    {
        if (!_isActive || _remainingTime <= 0f)
        {
            return;
        }

        _remainingTime = Mathf.Max(0f, _remainingTime - Time.deltaTime);

        if (_remainingTime <= 0f)
        {
            OnTimeOver?.Invoke();
        }
    }

    /// <summary>
    /// 時間を進めるか止めるか（GameManager が呼ぶ）
    /// </summary>
    public void SetTimerIsActive(bool isActive)
    {
        _isActive = isActive;
    }

    /// <summary>
    /// 残り時間を満タンに戻す（起動のたびに GameManager が呼ぶ）
    /// </summary>
    public void ResetTimer()
    {
        _remainingTime = _duration;
    }
}
