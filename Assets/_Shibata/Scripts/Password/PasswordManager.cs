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

    public string LockAnswer => _lockAnswer;
    public string PictureAnswer => _pictureAnswer;
    public AnswerDatabase DataBase => _dataBase;
    public PuzzleType PuzzleType => _puzzleType;

        [SerializeField]
    private AnswerDatabase _dataBase;
    private static PasswordManager instance;
    private string _lockAnswer;
    private string _pictureAnswer;
    private PuzzleType _puzzleType = PuzzleType.Lock;

    public void ChangeType(PuzzleType type)
    {
        _puzzleType = type;
    }

    public void AddRockAnswer(string answer)
    {
        Debug.Log(answer);
        _lockAnswer = answer;
    }

    public void SetPictureAnswer(string answer)
    {
        _pictureAnswer = answer;
    }

    public void GenerateAnswers()
    {
        _dataBase.LockPasswordAnswers.SetBirthDay();
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
    }
}
