using UnityEngine;
using UnityEngine.UI;

public class Battery : MonoBehaviour
{
    [SerializeField] private Image _battery;

    /// <summary>
    /// タイマー側から秒数の割合を引数に入れて毎フレーム更新
    /// </summary>
    public void UpdateBatteryLevel(float percent)
    {
        //バッテリーが減る処理
        _battery.fillAmount = percent;
    }
}
