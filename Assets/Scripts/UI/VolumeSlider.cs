using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 音量のバー（デモ v0.27 の設定画面と同じ）。枠の中を押した位置・ドラッグした位置が、そのまま音量になる。
/// 値は 0〜1 の 5% 刻み。中のバー（_fill）の右端を、値の位置まで伸ばして表示する。0 のときはバーを隠す
/// </summary>
public class VolumeSlider : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("押せる範囲（枠）と、伸び縮みするバー")]
    [SerializeField] private RectTransform _frame;
    [SerializeField] private RectTransform _fill;

    private float _value = 1f;

    /// <summary>
    /// 押した・ドラッグしたときに、新しい値（5% 刻み）を知らせる
    /// </summary>
    public event Action<float> OnChanged;

    /// <summary>
    /// 表示を変える（保存されている値を出すときなど。OnChanged は呼ばない）
    /// </summary>
    public void SetValue(float value)
    {
        _value = SoundSettings.RoundVolume(value);
        _fill.anchorMax = new Vector2(_value, _fill.anchorMax.y);
        _fill.gameObject.SetActive(_value > 0f);
    }

    /// <summary>
    /// 押したところの値にする
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeTo(eventData);
    }

    /// <summary>
    /// ドラッグしたところの値にする
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        ChangeTo(eventData);
    }

    /// <summary>
    /// 画面上の位置を、枠の左端 0 〜 右端 1 の値にして知らせる
    /// </summary>
    private void ChangeTo(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_frame, eventData.position, eventData.pressEventCamera, out Vector2 local))
        {
            return;
        }

        Rect rect = _frame.rect;
        float value = SoundSettings.RoundVolume((local.x - rect.xMin) / rect.width);

        if (Mathf.Approximately(value, _value))
        {
            return;
        }

        SetValue(value);
        OnChanged?.Invoke(value);
    }
}
