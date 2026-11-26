using UnityEngine;

public enum PuzzleType
{
    Lock,
    Picture
}

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

    public AnswerDatabase DataBase => _dataBase;
    public PuzzleType PuzzleType => _puzzleType;
    public string Answer => _answer;

    [SerializeField]
    private AnswerDatabase _dataBase;
    private static PasswordManager instance;
    private PuzzleType _puzzleType = PuzzleType.Lock;
    private PicturePassword _instancePassword = new();
    private string _answer;
    private int _september;

    public void ChangeType(PuzzleType type)
    {
        _puzzleType = type;
    }

    public void AddRockAnswer(string answer)
    {
        _answer = answer;
    }

    public void GenerateAnswers()
    {
        _dataBase.LockPasswordAnswers.SetBirthDay();
    }

    public void InstancePassword()
    {
        _instancePassword.OnInstace();
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
            Destroy(gameObject); // 重複インスタンスを破棄
        }

        GenerateAnswers();
        InstancePassword();
    }
}
