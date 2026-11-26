using UnityEngine;

public class LockerPassword : IState
{
    private string _correct;
    private int _randomNumber;

    public void InstanceRandom()
    {
        _randomNumber = Random.Range(1111, 10000);
        _correct = _randomNumber.ToString();
        Debug.Log(_correct);
    }

    public bool IsCorrect(string answer) => answer == _correct;
}
