using UnityEngine;

public enum PuzzleType
{
    Lock,
    Gallery
}

/// <summary>
/// シングルトンパターン
/// </summary>
public class PasswordManager : MonoBehaviour
{
    public static PasswordManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<PasswordManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<PasswordManager>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// 誕生月などのデータ取得
    /// </summary>
    public AnswerDatabase DataBase => _dataBase;
    /// <summary>
    /// ギャラリーでのなぞ解きの回答
    /// </summary>
    public MemoPassword MemoPassword => _memoPassword;
    /// <summary>
    /// 現在どの謎を解いているか
    /// </summary>
    public PuzzleType PuzzleType => _puzzleType;
    /// <summary>
    /// 正しい答え
    /// </summary>
    public string Correct => _correct;

    [SerializeField]
    private AnswerDatabase _dataBase;
    [SerializeField]
    private PasswordChecker _passwordChecker;
    private static PasswordManager instance;
    private PuzzleType _puzzleType;
    private MemoPassword _memoPassword;
    private string _correct;

    /// <summary>
    /// 現在解いてる謎の切替
    /// </summary>
    /// <param name="type">切り替えたいタイプ</param>
    public void ChangeType(AppId type)
    {
        _passwordChecker.ChangeState(type);
    }

    /// <summary>
    /// 正しい回答をセット
    /// </summary>
    /// <param name="correct">正しい答え</param>
    public void SetCorrect(string correct)
    {
        _correct = correct;
    }

    /// <summary>
    /// 誕生日と月のランダム抽選
    /// </summary>
    public void GenerateAnswers()
    {
        _dataBase.LockPasswordAnswers.SetBirthDay();
    }

    /// <summary>
    /// ギャラリーのパスワードのランダム抽選
    /// </summary>
    public void InstancePassword()
    {
        _memoPassword.OnInstace();
    }

    void Awake()
    { 
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //_puzzleType = PuzzleType.Gallery;
        //GenerateAnswers();
        InstancePassword();
    }
}
