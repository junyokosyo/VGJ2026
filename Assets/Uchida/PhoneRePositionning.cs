using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// オブジェクトを徐々に上昇させ、
/// 上昇完了時にゆっくり停止させるクラス。
/// </summary>

public class PhoneRePositionning : MonoBehaviour
{
    [SerializeField] private PhoneSwipe phoneSwipe;
    [Header("上昇設定")]
    [SerializeField] private float _moveHeight = 3f;
    [SerializeField] private float _moveDuration = 2f;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private int _moveCount;

    /// <summary>
    /// 初期位置と目標位置を設定し、上昇処理を開始する。
    /// </summary>
    private void Start()
    {
        _startPosition = transform.position;

        _targetPosition = _startPosition + Vector3.up * _moveHeight;
    }

    private void Update()
    {
        if (phoneSwipe.IsSwiping && _moveCount <= 1)
        {
            StartCoroutine(MoveUp());
        }
    }

    /// <summary>
    /// オブジェクトを上昇させ、徐々に減速して停止させる。
    /// </summary>
    private IEnumerator MoveUp()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / _moveDuration);

            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            transform.position = Vector3.Lerp(_startPosition, _targetPosition, smoothProgress);

            yield return null;
        }
        transform.position = _targetPosition;
        _moveCount++;
    }
}
