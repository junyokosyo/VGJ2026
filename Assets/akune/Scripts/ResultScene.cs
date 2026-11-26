using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour
{
    [SerializeField] private ClearCounter _clearCounter;
    [SerializeField] private TextMeshProUGUI _clearCountText;
    void Start()
    {
        ShowResult();
    }


    private void ShowResult()
    {
        _clearCountText.text = "clearCout:" + _clearCounter.GetClearCount().ToString();
    }
}
