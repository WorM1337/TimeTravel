using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Player player; // ������ �� Player
    [SerializeField] private Slider healthSlider; // ������ �� Slider ��� ��������

    void Start()
    {
        if (player == null || healthSlider == null)
        {
            Debug.LogError("UIManager: Missing Player or Health Slider reference!");
            return;
        }

        // ������������� �� ������� ��������� ��������
        player.OnHealthChanged += UpdateHealthSlider;

        // �������������� Slider ��������� ���������
        UpdateHealthSlider(player.GetCurrentHealth());
    }

    // ���������� Slider'� ��� ��������� ��������
    private void UpdateHealthSlider(float currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / player.GetMaxHealth(); // ������������ �� 0 �� 1
        }
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealthSlider; // ������������ ��� �����������
        }
    }
}