using UnityEngine;
using UnityEngine.EventSystems;

public class PasswordInput : MonoBehaviour,IPointerDownHandler
{
    [SerializeField]
    private AnswerDatabase _answerDatabase;
    private string _input;

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void ResetText()
    {
        _input = "";
    }
}
