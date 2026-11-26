public class MessagingPassward : IState
{
    private string _correct;
    /// <summary>
    /// メモに書いてあるタイトル番号の順番
    /// </summary>
    private int _order;

    public bool IsCorrect(string answer)
    {
        return answer == _correct;
    }
}
