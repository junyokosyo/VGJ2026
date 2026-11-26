using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステータスバーの電池の表示（デモと同じ：10% から始まり、40秒で 0% になる。4秒で 1% ずつ減る）。
/// 時間は持たない。Timer の残りの割合を見て、% の文字と10本のバーを更新するだけ。
/// バーの Image に絵を入れれば、そのまま絵が使われる（減ったバーは薄くなる）
/// </summary>
public class Battery : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Timer _timer;

    [Header("表示")]
    [SerializeField] private TextMeshProUGUI _percentText;
    [SerializeField] private Image[] _bars;

    [Header("満タンのときに出す %（バーの本数と同じにする）")]
    [SerializeField] private int _startPercent = 10;

    [Header("減ったバーの濃さと、薄くなるまでの秒数")]
    [SerializeField] private float _spentAlpha = 0.14f;
    [SerializeField] private float _fadeDuration = 0.35f;

    private int _shownPercent = -1;

    /// <summary>
    /// 今表示している %
    /// </summary>
    public int Percent => _shownPercent;

    /// <summary>
    /// 最初の表示をすぐに出す（アニメーションなし）
    /// </summary>
    private void Start()
    {
        if (_timer == null)
        {
            Debug.LogWarning($"[Battery] {name} に Timer がつながっていません");
            enabled = false;
            return;
        }

        Refresh(CalculatePercent(), false);
    }

    /// <summary>
    /// % が変わったときだけ表示を更新する（再起動で満タンに戻ったときも、ここで戻る）
    /// </summary>
    private void Update()
    {
        int percent = CalculatePercent();

        if (percent != _shownPercent)
        {
            Refresh(percent, true);
        }
    }

    /// <summary>
    /// 残りの割合を % にする。0% になるのは時間切れのときだけにしたいので切り上げる
    /// </summary>
    private int CalculatePercent()
    {
        return Mathf.CeilToInt(_timer.RemainingRate * _startPercent);
    }

    /// <summary>
    /// % の文字とバーの濃さを変える
    /// </summary>
    private void Refresh(int percent, bool isAnimated)
    {
        _shownPercent = percent;

        if (_percentText != null)
        {
            _percentText.text = $"{percent}%";
        }

        for (int i = 0; i < _bars.Length; i++)
        {
            float alpha = i < percent ? 1f : _spentAlpha;
            Image bar = _bars[i];
            bar.DOKill();

            if (isAnimated)
            {
                bar.DOFade(alpha, _fadeDuration).SetLink(bar.gameObject);
            }
            else
            {
                Color color = bar.color;
                color.a = alpha;
                bar.color = color;
            }
        }
    }
}
