using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Player player; // Ссылка на Player
    [SerializeField] private Slider healthSlider; // Ссылка на Slider для здоровья

    void Start()
    {
        if (player == null || healthSlider == null)
        {
            Debug.LogError("UIManager: Missing Player or Health Slider reference!");
            return;
        }

        // Подписываемся на событие изменения здоровья
        player.OnHealthChanged += UpdateHealthSlider;

        // Инициализируем Slider начальным значением
        UpdateHealthSlider(player.GetCurrentHealth());
    }

    // Обновление Slider'а при изменении здоровья
    private void UpdateHealthSlider(float currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / player.GetMaxHealth(); // Нормализация от 0 до 1
        }
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealthSlider; // Отписываемся при уничтожении
        }
    }
}