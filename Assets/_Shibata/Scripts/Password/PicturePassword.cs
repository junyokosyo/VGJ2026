using UnityEngine;

public class PicturePassword
{
    private int _september;
    private int _october;
    private int _november;
    private int _december;

    public void OnInstace()
    {
        _september = Random.Range(3, 10);
        _october = Random.Range(3, 10);
        _november = Random.Range(3, 10);
        _december = Random.Range(3, 10);
    }

    /// <summary>
    /// 回答が合っているか
    /// </summary>
    /// <param name="answer">回答</param>
    /// <returns>全部一致だったらtrue</returns>
    public bool CheckAnswer(string answer)
    {
        var september = answer[0] - '0';
        var october = answer[1] - '0';
        var november = answer[2] - '0';
        var december = answer[3] - '0';

        return september == _september
                && october == _october
                && november == _november
                && december == _december;
    }
}
