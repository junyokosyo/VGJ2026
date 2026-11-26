using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Battery : MonoBehaviour
{
    [Header("バッテリーのメモリを入れる親オブジェクト")]
    [SerializeField] private Transform _batteryParent;
    [Header("バッテリー1本を表すプレファブ")]
    [SerializeField] private GameObject _batterySegmentPrefab;
    [Header("バッテリーの一本ごとの間隔")]
    [SerializeField] private int _batteryOffset = 9;
    [Header("バッテリー1本目の左側の間隔(+の値を入れてください)")]
    [SerializeField] private int _leftPadding = 40;
    [Header("バッテリー1本が消えるまでの秒数")]
    [SerializeField] private float _batteryDecreaseDuration = 0.3f;


    private Image[] _battery;
    private int _previousFilledCount = 0;
    private const int _batterySegmentCount = 10;
    private void Awake()
    {
        _battery = new Image[_batterySegmentCount];

        var startPos = _batteryParent.position - new Vector3(_leftPadding, 0, 0);

        //バッテリーを本数分生成
        for (int i = 0; i < _batterySegmentCount; ++i)
        {
            //バッテリーの間隔をあけて生成
            var spawnPos = startPos + new Vector3(_batteryOffset * i, 0, 0);
            //生成したオブジェクトのImageを保存
            _battery[i] = Instantiate(_batterySegmentPrefab, spawnPos , Quaternion.identity, _batteryParent).GetComponent<Image>();
        }
    }


    /// <summary>
    /// タイマー側から秒数の割合を引数に入れて毎フレーム更新
    /// </summary>
    public void UpdateBatteryLevel(float percent)
    {
        //バッテリーが10％未満の時にも一本表示させるため繰り上げ
        int intPercent = Mathf.CeilToInt(percent * 10);
        //percentをもとに何本減らすかを持つ変数
        int filledSegmentCount = _batterySegmentCount - intPercent;

        //バッテリーが減った時だけ処理を走らせる
        if (filledSegmentCount != _previousFilledCount)
        {
            DecreaseBattery(filledSegmentCount - _previousFilledCount);
            _previousFilledCount = filledSegmentCount;
            Debug.Log(filledSegmentCount);
        }
    }

    /// <summary>
    /// バッテリーを満タンに戻す（再起動のときに Timer から呼ばれる）
    /// </summary>
    public void ResetBattery()
    {
        //消えるアニメーションの途中のものを止める
        StopAllCoroutines();

        foreach (Image segment in _battery)
        {
            segment.fillAmount = 1f;
            segment.gameObject.SetActive(true);
        }

        _previousFilledCount = 0;
    }

    private void DecreaseBattery(int decreaseCount)
    {
        //バッテリーを減らし終わった時にループを抜け出すためのカウンター
        int batteryCount = _batterySegmentCount - 1;

        for (int i = 0; i <= batteryCount; ++i)
        {
            //バッテリーの後ろからactiveかどうかを調べてactiveのやつを減らす
            if(_battery[batteryCount - i].gameObject.activeSelf)
            {
                StartCoroutine(PlaySegmentDisappearAnimation(_battery[batteryCount - i]));
                --decreaseCount;
            }
            else
            {
                continue;
            }

            //減らした回数が0になったらループから抜け出す
            if (decreaseCount <= 0) break;
        }
    }

    private IEnumerator PlaySegmentDisappearAnimation(Image battery)
    {
        float animTimer = _batteryDecreaseDuration;

        while (animTimer > 0)
        {
            animTimer -= Time.deltaTime;
            battery.fillAmount = animTimer / _batteryDecreaseDuration;

            yield return null;
        }

        battery.gameObject.SetActive(false);
    }
}
