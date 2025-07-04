using UnityEngine;
using TMPro;

public class Plate : MonoBehaviour, IInteractable
{
    [SerializeField] private string plateText = "This is a Plate!";
    [SerializeField] private TextMeshProUGUI uiText;

    private bool isPlayerNearby = false;
    private static Plate activePlate = null;

    private void Awake()
    {
        if (uiText != null)
        {
            uiText.enabled = false;
            uiText.text = "";
        }

    }

    public void Press()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                if (activePlate != null && activePlate != this)
                {
                    activePlate.HideText();
                }
                activePlate = this;
                player.CurrentInteractable = this;
                ShowText();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                if (activePlate == this)
                {
                    activePlate = null;
                    player.CurrentInteractable = null;
                    HideText();
                }
            }
        }
    }

    private void ShowText()
    {
        if (uiText != null)
        {
            uiText.text = plateText;
            uiText.enabled = true;
        }
    }

    private void HideText()
    {
        if (uiText != null)
        {
            uiText.enabled = false;
            uiText.text = "";
        }
    }
}
