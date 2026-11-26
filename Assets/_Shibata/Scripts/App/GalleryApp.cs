using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gallery アプリ（柴田さんのものを、抽選結果を GameProgress から読む形に変更）。
/// 開くたびに、9〜12月それぞれ抽選された枚数ぶん写真を並べ直す。
/// 写真の拡大表示はあとで足す
/// </summary>
public class GalleryApp : AppBase
{
    [Header("各月の写真を並べる親（9・10・11・12月）")]
    [SerializeField] private Transform _septemberparent;
    [SerializeField] private Transform _octoberparent;
    [SerializeField] private Transform _novemberparent;
    [SerializeField] private Transform _decemberparent;

    [Header("参照")]
    [SerializeField] private AnswerDatabase _dataBase;

    [Header("写真1枚のプレハブ（Image）")]
    [SerializeField] private GameObject _gallery;

    /// <summary>
    /// 開いたら写真を並べ直す
    /// </summary>
    protected override void OnOpened()
    {
        SetImages();
    }

    /// <summary>
    /// 抽選された枚数ぶん、月ごとに写真を並べる
    /// </summary>
    public void SetImages()
    {
        Transform[] parents = { _septemberparent, _octoberparent, _novemberparent, _decemberparent };

        for (int i = 0; i < parents.Length; i++)
        {
            int month = 9 + i;
            int count = GameProgress.Codes.PhotoCounts[i];

            ClearChildren(parents[i]);

            for (int j = 0; j < count; j++)
            {
                GameObject photo = Instantiate(_gallery, parents[i]);
                Sprite sprite = GetSprite(month, j);

                if (sprite != null)
                {
                    photo.GetComponent<Image>().sprite = sprite;
                }
            }
        }
    }

    /// <summary>
    /// その月の j 枚目の写真（登録が足りなければ最後の1枚。1枚もなければ null）
    /// </summary>
    private Sprite GetSprite(int month, int j)
    {
        foreach (ConfirmedSprite confirmed in _dataBase.confirmedSprites)
        {
            if (confirmed.Month == month && confirmed.Sprite.Count > 0)
            {
                return confirmed.Sprite[Mathf.Min(j, confirmed.Sprite.Count - 1)];
            }
        }

        return null;
    }

    /// <summary>
    /// 前に並べた写真を消す（開くたびに増えないように）
    /// </summary>
    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
