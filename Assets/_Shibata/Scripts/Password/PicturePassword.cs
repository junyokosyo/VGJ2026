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

    public void CheckAnswer(string answer)
    {
        var september = answer[0];
        var october = answer[1];
        var november = answer[2];
        var december = answer[3];
    }
}
