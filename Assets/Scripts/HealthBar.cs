using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private float _maxHealth;
    private float _currentHealth;

    public void SetMaxHealth(float value)
    {
        _maxHealth = value;
        slider.maxValue = _maxHealth;
        _currentHealth = _maxHealth;
        UpdateBar();
    }

    public void SetHealth(float value)
    {
        _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
        UpdateBar();
    }

    private void UpdateBar()
    {
        slider.value = _currentHealth;
    }
}
