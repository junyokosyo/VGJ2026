using UnityEngine;

public class PicturePassword
{
    /// <summary>
    /// 正しい答え
    /// </summary>
    public string Correct => _correct;

    /// <summary>
    /// ９月の枚数
    /// </summary>
    private int _september;
    /// <summary>
    /// １０月の枚数
    /// </summary>
    private int _october;
    /// <summary>
    /// １１月の枚数
    /// </summary>
    private int _november;
    /// <summary>
    /// 12月の枚数
    /// </summary>
    private int _december;
    private string _correct;

    /// <summary>
    /// 画像の数の抽選
    /// </summary>
    public void OnInstace()
    {
        _september = Random.Range(3, 10);
        _october = Random.Range(3, 10);
        _november = Random.Range(3, 10);
        _december = Random.Range(3, 10);

        _correct = _october.ToString() +
            _september.ToString() +
            _december.ToString() +
            _november.ToString();

        Debug.Log(_correct);
    }

    /// <summary>
    /// 回答が合っているか
    /// </summary>
    /// <param name="answer">回答</param>
    /// <returns>全部一致だったらtrue</returns>
    public bool CheckAnswer(string answer) => answer == _correct;
}
