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
    /// <summary>
    /// ギャラリー
    /// </summary>
    [SerializeField]
    private GameObject _gallery;

    /// <summary>
    /// ランダムで選ばれた枚数分生成
    /// </summary>
    public void SetImages()
    {
        int september = PasswordManager.Instance.GalleryPassword.September;
        int october = PasswordManager.Instance.GalleryPassword.October;
        int november = PasswordManager.Instance.GalleryPassword.November;
        int december = PasswordManager.Instance.GalleryPassword.December;
        int[] months = { september, october, november, december };
        Transform parent = null;

        for(int i = 0;i < months.Length; i++)
        {
            switch (i)
            {
                case 0:
                    parent = _septemberparent;
                    break;
                case 1:
                    parent = _octoberparent;
                    break;
                case 2:
                    parent = _novemberparent;
                    break;
                case 3:
                    parent = _decemberparent;
                    break;
            }

            for (int j = 1; j <= months[i]; j++)
            {
                GameObject gallery = Instantiate(_gallery,parent);
                Image image = gallery.GetComponent<Image>();
            }
        }
    }
}
