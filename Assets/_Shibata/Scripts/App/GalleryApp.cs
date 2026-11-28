using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gallery アプリ（柴田さんのものを、抽選結果を GameProgress から読む形に変更）。
/// 開くたびに、9〜12月それぞれ抽選された枚数ぶん写真を並べ直す。
/// ・小さい写真は正方形に切り抜いて、角を丸めて並べる（iPhone の写真アプリ風。左上に「18日」のような日付）
/// ・写真を押すと、拡大画面（GalleryViewer）で大きく見られる
/// ・写真の並べ方（デモ v0.27 と同じ考え方）：
///   「話に関わる写真」（AnswerDatabase の confirmedSprites に月ごとに登録。各月2枚まで）を入れ、
///   残りの枠は「素材の写真」（_poolSprites）からランダムに入れる。同じ素材の写真はとなり合わないようにする
/// ・説明文は「話に関わる写真」にだけ付ける
/// ・並びは暗証番号から決めるので、開き直しても・再起動しても同じ。新しいゲームになると変わる
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

    [Header("素材の写真（謎解きに関係ない写真。各月の空いている枠にランダムに入る）")]
    [SerializeField] private List<Sprite> _poolSprites = new();

    [Header("写真を大きく見る画面")]
    [SerializeField] private GalleryViewer _viewer;

    [Header("写真の説明文の予備（9・10・11・12月）。ふだんは AnswerDatabase の Messages の文を使う")]
    [SerializeField] private string[] _captions =
    {
        "公園で一緒に歩いている。",
        "カメラを持つ手が写っている。",
        "ベンチで休んでいる。",
        "ブレていたり、うまく撮れていない写真。",
    };

    [Header("小さい写真の後ろの色（写真がないときに見える）")]
    [SerializeField] private Color _thumbnailBackColor = new Color32(0x1b, 0x1e, 0x26, 0xff);

    [Header("小さい写真の角を丸める絵（9スライスの白い角丸。空なら四角のまま）")]
    [SerializeField] private Sprite _tileSprite;

    [Header("月の見出し（「9月」など）の文字の色。Gallery の背景が白いので、黒っぽい色にする")]
    [SerializeField] private Color _monthLabelColor = new Color32(0x1c, 0x1c, 0x1e, 0xff);

    [Header("小さい写真の左上の日付（文字の大きさ・アウトラインの太さ（ピクセル）・アウトラインの色）")]
    [SerializeField] private float _dayFontSize = 22f;
    [SerializeField, Range(0f, 4f)] private float _dayOutlinePixels = 1.6f;
    [SerializeField] private Color _dayOutlineColor = new Color32(0, 0, 0, 220);

    private const int MaxStoryPhotos = 2;

    private readonly List<Sprite> _photos = new();
    private readonly List<int> _photoMonths = new();
    private readonly List<string> _photoCaptions = new();

    /// <summary>
    /// 開いたら写真を並べ直す
    /// </summary>
    protected override void OnOpened()
    {
        if (_viewer != null)
        {
            _viewer.Close();
        }

        SetImages();
    }

    /// <summary>
    /// 抽選された枚数ぶん、月ごとに写真を並べる
    /// </summary>
    public void SetImages()
    {
        Transform[] parents = { _septemberparent, _octoberparent, _novemberparent, _decemberparent };
        _photos.Clear();
        _photoMonths.Clear();
        _photoCaptions.Clear();

        // 暗証番号から並びを決める（同じゲームのあいだは、何度開いても同じ並びになる）
        System.Random random = new System.Random(GetLayoutSeed());
        List<Sprite> poolBag = new();

        for (int i = 0; i < parents.Length; i++)
        {
            int month = 9 + i;
            int count = GameProgress.Codes.PhotoCounts[i];
            string caption = GetCaption(month, i);

            ClearChildren(parents[i]);

            List<PhotoSlot> slots = MakeMonthSlots(month, count, random, poolBag);
            List<int> days = MakeDays(month, slots.Count);

            for (int j = 0; j < slots.Count; j++)
            {
                PhotoSlot slot = slots[j];
                CreatePhoto(parents[i], slot.Sprite, _photos.Count, days[j]);

                _photos.Add(slot.Sprite);
                _photoMonths.Add(month);
                _photoCaptions.Add(slot.IsStory ? caption : string.Empty);
            }

            FitGridHeight(parents[i], slots.Count);
            ShowMonthLabel(parents[i], month);
        }

        // 写真を入れたあとで並びを計算し直す（月の見出しが写真に隠れないように）
        if (_septemberparent.parent != null && _septemberparent.parent.parent is RectTransform months)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(months);
        }

        if (_viewer != null)
        {
            _viewer.SetPhotos(_photos, _photoMonths, _photoCaptions);
        }
    }

    /// <summary>
    /// 小さい写真を1枚作る。枠（プレハブの Image）の中に写真を入れて、はみ出す分を切る（正方形に切り抜く）。
    /// 角丸の絵があれば、Mask で角を丸く切る（RectMask2D は四角にしか切れない）。左上に日付を出す。
    /// 押すと、拡大画面で index 番目の写真を開く
    /// </summary>
    private void CreatePhoto(Transform parent, Sprite sprite, int index, int day)
    {
        GameObject photo = Instantiate(_gallery, parent);
        Image frame = photo.GetComponent<Image>();
        frame.color = _thumbnailBackColor;

        if (_tileSprite != null)
        {
            frame.sprite = _tileSprite;
            frame.type = Image.Type.Sliced;
            photo.AddComponent<Mask>().showMaskGraphic = true;
        }
        else
        {
            frame.sprite = null;
            photo.AddComponent<RectMask2D>();
        }

        if (sprite != null)
        {
            // 写真の縦横の比のまま、枠いっぱいに広げる（はみ出した分は RectMask2D が切る）
            GameObject inner = new GameObject("Photo", typeof(RectTransform), typeof(Image), typeof(AspectRatioFitter));
            inner.transform.SetParent(photo.transform, false);

            Image image = inner.GetComponent<Image>();
            image.sprite = sprite;
            image.raycastTarget = false;

            AspectRatioFitter fitter = inner.GetComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = sprite.rect.width / sprite.rect.height;
        }

        CreateDayLabel(photo.transform, day);

        if (_viewer == null)
        {
            return;
        }

        // index は、ループの変数ではなく引数（この写真だけの番号）なので、全部の写真が同じ番号になることはない
        Button button = photo.AddComponent<Button>();
        button.targetGraphic = frame;
        button.onClick.AddListener(() => _viewer.Open(index));
    }

    /// <summary>
    /// 月の見出し（写真の並ぶ場所と同じ親の「Label」）に「9月」を出し、高さを確保する
    /// </summary>
    private void ShowMonthLabel(Transform parent, int month)
    {
        Transform label = parent.parent != null ? parent.parent.Find("Label") : null;

        if (label == null || !label.TryGetComponent(out TextMeshProUGUI text))
        {
            return;
        }

        label.gameObject.SetActive(true);
        text.enabled = true;
        text.text = $"{month}月";
        text.color = _monthLabelColor;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.BottomLeft;

        if (!label.TryGetComponent(out LayoutElement element))
        {
            element = label.gameObject.AddComponent<LayoutElement>();
        }

        element.minHeight = Mathf.Max(element.minHeight, 46f);
        element.preferredHeight = Mathf.Max(element.preferredHeight, 46f);
    }

    /// <summary>
    /// 写真の並ぶ場所（GridLayoutGroup）の高さを、写真の段の数に合わせる。
    /// 高さが 0 のままだと、写真が下にはみ出して、次の月の見出しの上にかぶさってしまう
    /// </summary>
    private static void FitGridHeight(Transform parent, int count)
    {
        if (!parent.TryGetComponent(out GridLayoutGroup grid))
        {
            return;
        }

        int columns = grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount ? Mathf.Max(1, grid.constraintCount) : 3;
        int rows = Mathf.CeilToInt(count / (float)columns);
        float height = rows * grid.cellSize.y + Mathf.Max(0, rows - 1) * grid.spacing.y + grid.padding.vertical;

        if (!parent.TryGetComponent(out LayoutElement element))
        {
            element = parent.gameObject.AddComponent<LayoutElement>();
        }

        element.minHeight = height;
        element.preferredHeight = height;
    }

    /// <summary>
    /// 小さい写真の左上に「18日」のような日付を出す（白い文字に黒いアウトライン。明るい写真でも読める）。
    /// アウトラインは、黒い同じ文字を8方向に少しずつずらして後ろに重ねて作る
    /// （TextMeshPro のマテリアルのアウトラインは、フォントや切り抜きの設定によって出ないことがあるため）
    /// </summary>
    private void CreateDayLabel(Transform photo, int day)
    {
        GameObject dayObject = new GameObject("Day", typeof(RectTransform));
        RectTransform rect = (RectTransform)dayObject.transform;
        rect.SetParent(photo, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.offsetMin = new Vector2(10f, -40f);
        rect.offsetMax = new Vector2(-6f, -6f);

        string text = $"{day}日";

        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI / 4f;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _dayOutlinePixels;
            CreateDayText(rect, text, _dayOutlineColor, offset);
        }

        CreateDayText(rect, text, Color.white, Vector2.zero);
    }

    /// <summary>
    /// 日付の文字を1枚作る（offset だけずらす）
    /// </summary>
    private void CreateDayText(RectTransform parent, string text, Color color, Vector2 offset)
    {
        GameObject labelObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        RectTransform rect = (RectTransform)labelObject.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = offset;
        rect.offsetMax = offset;

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = _dayFontSize;
        label.fontStyle = FontStyles.Bold;
        label.color = color;
        label.alignment = TextAlignmentOptions.TopLeft;
        label.raycastTarget = false;
    }

    /// <summary>
    /// その月の写真の日付を、count 個（重ならない・小さい順）決める。
    /// 暗証番号と月から決めるので、開き直しても・再起動しても同じ
    /// </summary>
    private static List<int> MakeDays(int month, int count)
    {
        int daysInMonth = System.DateTime.DaysInMonth(2026, month);
        System.Random random = new System.Random(GetLayoutSeed() + month * 97);
        List<int> all = new();

        for (int day = 1; day <= daysInMonth; day++)
        {
            all.Add(day);
        }

        Shuffle(all, random);
        List<int> days = all.GetRange(0, Mathf.Min(count, all.Count));
        days.Sort();

        while (days.Count < count)
        {
            days.Add(daysInMonth);
        }

        return days;
    }

    /// <summary>
    /// その月の写真の説明文。AnswerDatabase の Messages（月ごとの Story）を使う。
    /// 空なら、このコンポーネントの Captions（デモ v0.27 の文）を使う
    /// </summary>
    private string GetCaption(int month, int monthIndex)
    {
        foreach (Message message in _dataBase.Messages)
        {
            if (message.Month == month && !string.IsNullOrEmpty(message.Story))
            {
                return message.Story;
            }
        }

        return monthIndex < _captions.Length ? _captions[monthIndex] : string.Empty;
    }

    /// <summary>
    /// 写真1枚分（写真と、話に関わる写真かどうか）
    /// </summary>
    private struct PhotoSlot
    {
        public Sprite Sprite;
        public bool IsStory;
    }

    /// <summary>
    /// 1か月分の写真の並びを作る。話に関わる写真（2枚まで）＋素材の写真で count 枚にして、混ぜる。
    /// 素材の写真は、全部を一度ずつ使い切ってから次の周に入る（同じ写真ばかりにならないように）
    /// </summary>
    private List<PhotoSlot> MakeMonthSlots(int month, int count, System.Random random, List<Sprite> poolBag)
    {
        List<PhotoSlot> slots = new();

        foreach (Sprite story in GetStorySprites(month))
        {
            if (slots.Count >= Mathf.Min(count, MaxStoryPhotos))
            {
                break;
            }

            slots.Add(new PhotoSlot { Sprite = story, IsStory = true });
        }

        while (slots.Count < count)
        {
            slots.Add(new PhotoSlot { Sprite = DrawFromPool(random, poolBag), IsStory = false });
        }

        Shuffle(slots, random);
        SeparateSamePhotos(slots);
        return slots;
    }

    /// <summary>
    /// 素材の写真を1枚引く（袋が空になったら、全部を入れ直して混ぜる）。素材が1枚もなければ null
    /// </summary>
    private Sprite DrawFromPool(System.Random random, List<Sprite> poolBag)
    {
        if (_poolSprites.Count == 0)
        {
            return null;
        }

        if (poolBag.Count == 0)
        {
            poolBag.AddRange(_poolSprites);
            Shuffle(poolBag, random);
        }

        Sprite sprite = poolBag[poolBag.Count - 1];
        poolBag.RemoveAt(poolBag.Count - 1);
        return sprite;
    }

    /// <summary>
    /// 同じ写真がとなり合っていたら、後ろの写真と入れ替えて離す（できる範囲で）
    /// </summary>
    private static void SeparateSamePhotos(List<PhotoSlot> slots)
    {
        for (int i = 1; i < slots.Count; i++)
        {
            if (slots[i].Sprite == null || slots[i].Sprite != slots[i - 1].Sprite)
            {
                continue;
            }

            for (int k = i + 1; k < slots.Count; k++)
            {
                if (slots[k].Sprite != slots[i - 1].Sprite)
                {
                    (slots[i], slots[k]) = (slots[k], slots[i]);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// リストの中身を混ぜる
    /// </summary>
    private static void Shuffle<T>(List<T> list, System.Random random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    /// <summary>
    /// 並びを決める元の数（暗証番号から作るので、新しいゲームになるまで変わらない）
    /// </summary>
    private static int GetLayoutSeed()
    {
        PuzzleCodes codes = GameProgress.Codes;
        int seed = int.TryParse(codes.LetterNumber, out int letter) ? letter : 0;

        foreach (int photoCount in codes.PhotoCounts)
        {
            seed = seed * 10 + photoCount;
        }

        foreach (int number in codes.MemoNumbers)
        {
            seed = seed * 10 + number;
        }

        return seed;
    }

    /// <summary>
    /// その月の「話に関わる写真」（AnswerDatabase に登録された写真。空の枠は飛ばす）
    /// </summary>
    private List<Sprite> GetStorySprites(int month)
    {
        List<Sprite> sprites = new();

        foreach (ConfirmedSprite confirmed in _dataBase.confirmedSprites)
        {
            if (confirmed.Month != month)
            {
                continue;
            }

            foreach (Sprite sprite in confirmed.Sprite)
            {
                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }
        }

        return sprites;
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
