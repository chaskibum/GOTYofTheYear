using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float lerpSpeed;
    private float _maxHealth;
    private float _currentHealth;
    private float _targetHealth;

    public void SetMaxHealth(float value)
    {
        _maxHealth = value;
        slider.maxValue = _maxHealth;
        _currentHealth = _maxHealth;
        _targetHealth = _maxHealth;
        UpdateBar();
    }

    private void Update()
    {
        if (_targetHealth > 0)
            _currentHealth = Mathf.Lerp(_currentHealth, _targetHealth, Time.deltaTime * lerpSpeed);
        else 
            _currentHealth = _targetHealth;
        UpdateBar();
    }

    public void SetHealth(float value)
    {
        _targetHealth = Mathf.Clamp(value, 0, _maxHealth);
    }

    private void UpdateBar()
    {
        slider.value = _currentHealth;
    }
}
