using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Sign : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string _messageSignText;

    [SerializeField] private GameObject _signWindow;

    private void ShowMessage()
    {
        var text = _signWindow.GetComponentInChildren<TextMeshProUGUI>();

        text.text = _messageSignText;

        _signWindow.SetActive(true);
    }

    private void HideMessage()
    {
        _signWindow.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            ShowMessage();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            HideMessage();
        }
    }
}
