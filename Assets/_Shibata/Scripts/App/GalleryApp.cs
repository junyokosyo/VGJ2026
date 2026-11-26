using UnityEngine;
using UnityEngine.UI;

public class GalleryApp : AppBase
{
    /// <summary>
    /// 写真たちの親オブジェクト
    /// </summary>
    [SerializeField]
    private Transform _septemberparent;
    [SerializeField]
    private Transform _octoberparent;
    [SerializeField]
    private Transform _novemberparent;
    [SerializeField]
    private Transform _decemberparent;
    [SerializeField]
    private AnswerDatabase _dataBase;
    /// <summary>
    /// ギャラリー
    /// </summary>
    [SerializeField]
    private GameObject _gallery;
    
    void Start()
    {
        SetImages();
    }

    /// <summary>
    /// ランダムで選ばれた枚数分生成
    /// </summary>
    public void SetImages()
    {
        int september = PasswordManager.Instance.MemoPassword.September;
        int october = PasswordManager.Instance.MemoPassword.October;
        int november = PasswordManager.Instance.MemoPassword.November;
        int december = PasswordManager.Instance.MemoPassword.December;
        int[] months = { september, october, november, december };
        Transform parent = null;
        int month = 0;

        for(int i = 0;i < months.Length; i++)
        {
            switch (i)
            {
                case 0:
                    month = 9;
                    parent = _septemberparent;
                    break;
                case 1:
                    month = 10;
                    parent = _octoberparent;
                    break;
                case 2:
                    month = 11;
                    parent = _novemberparent;
                    break;
                case 3:
                    month = 12;
                    parent = _decemberparent;
                    break;
            }

            for (int j = 1; j <= months[i]; j++)
            {
                GameObject gallery = Instantiate(_gallery,parent);
                Image image = gallery.GetComponent<Image>();

                foreach(var confirmed in _dataBase.confirmedSprites)
                {
                    if (confirmed.Month == month)
                    {
                        if (j-1 < confirmed.Sprite.Count)
                        {
                            image.sprite = confirmed.Sprite[j-1];
                        }
                    }
                }
            }
        }
    }
}
