using System;
using UnityEngine;

public class PasswordChecker : MonoBehaviour
{
    /// <summary>
    /// 自分の回答をリセット時
    /// </summary>
    public static event Action OnReset;
    public IState Locker => _locker;
    public IState Memo => _memo;
    public IState Messaging => _messaging;

    private IState _locker = new LockerPassword();
    private IState _memo = new MemoPassword();
    private IState _messaging = new MessagingPassward();
    private IState _nowCurrent;

    /// <summary>
    /// 今のStateの切替
    /// </summary>
    /// <param name="next">次のState</param>
    public void ChangeState(AppId id)
    {
        switch (id)
        {
            case AppId.Memo:
                _nowCurrent = _memo;
                break;
            case AppId.Messaging:
                _nowCurrent = _messaging;
                break;
            case AppId.BirdModel:
                _nowCurrent = _locker;
                break;
        }
    }

    /// <summary>
    /// 自分の回答が正しいか
    /// </summary>
    /// <param name="text">自分の回答</param>
    public void CheckAnswer(string text)
    {
        if (_nowCurrent.IsCorrect(text))
        {
            Debug.Log("クリア");
        }
        else
        {
            OnReset?.Invoke();
        }

        //switch (PasswordManager.Instance.PuzzleType)
        //{
        //    case PuzzleType.Gallery:
        //        if (PasswordManager.Instance.MemoPassword.IsCorrect(text))
        //        {
        //            UnityEngine.Debug.Log("クリア");
        //        }
        //        else
        //        {
        //            OnReset?.Invoke();
        //        }
        //        break;
        //}
    }
}