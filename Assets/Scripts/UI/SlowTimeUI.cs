using UnityEngine;
using UnityEngine.UI;

public class SlowTimeUI : MonoBehaviour
{
    [SerializeField] private Image _hourGlassIcon;
    [SerializeField] private Image _progressFillCircle;

    public void ChangeHourGlassColor(Color color)
    {
        _hourGlassIcon.color = color;
    }

    public void ChangeAngleOfFilling(float k) // От 0 до 1, где 1 - это заполненная окружность
    {
        _progressFillCircle.fillAmount = k;
    }

}
