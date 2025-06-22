using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HealthChanger : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Image _redProgress;
    [SerializeField] private TextMeshProUGUI _textPrecents;

    private float pixelsPerUnitMultiplierCountWhenFull;

    private void Awake()
    {
        pixelsPerUnitMultiplierCountWhenFull = _redProgress.pixelsPerUnitMultiplier;
    }

    void Start()
    {
        _player.OnHealthChanged += UpdateHealth;
    }

    private void UpdateHealth(float currentHealth)
    {
        transform.localScale = new Vector3(currentHealth/_player.GetMaxHealth(), transform.localScale.y, transform.localScale.z);
        _redProgress.pixelsPerUnitMultiplier = pixelsPerUnitMultiplierCountWhenFull * currentHealth / _player.GetMaxHealth();
        _textPrecents.text = Math.Round(currentHealth / _player.GetMaxHealth() * 100.0).ToString() + '%';
    }
}
