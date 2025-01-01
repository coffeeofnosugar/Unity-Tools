using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField, BoxGroup("Conf")] private float upBarSpeed = .2f;
    [SerializeField, BoxGroup("Conf")] private float downBarSpeed = .8f;
    [SerializeField, BoxGroup("Conf")] private string stringFormat = "{0}/{1}";
    
    [Space(20)]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform upBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    
    private DOGetter<float> _valueGetter;
    private DOSetter<float> _valueSetter;
    [SerializeField] private float _currentValue;
    [SerializeField] private float _maxValue;
    
    public void Initialized()
    {
        _valueGetter = () => _currentValue;
        _valueSetter = (newValue) =>
        {
            _currentValue = newValue;
            upBar.sizeDelta = new Vector2(newValue / _maxValue * background.rect.width, 0f);
            _textMeshPro.text = string.Format(stringFormat, newValue.ToString("F0"), _maxValue.ToString("F0"));
        };
    }

    public void UpdateBar(float newValue, float maxValue, bool immediately = true)
    {
        _maxValue = maxValue;
        UpdateBar(newValue, immediately);
    }

    public void UpdateBar(float newValue, bool immediately = false)
    {
        if (immediately)
        {
            _valueSetter(newValue);
        }
        else
        {
            _textMeshPro.GetComponent<RectTransform>().DOShakePosition(upBarSpeed, 5f);
            DOTween.To(_valueGetter, _valueSetter, newValue, upBarSpeed).OnComplete(() =>
            {
                bottomBar.DOSizeDelta(new Vector2(newValue / _maxValue * background.rect.width, 0f), downBarSpeed);
            });
        }
    }
}