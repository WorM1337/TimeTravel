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

    public void ChangeAngleOfFilling(float k) // �� 0 �� 1, ��� 1 - ��� ����������� ����������
    {
        _progressFillCircle.fillAmount = k;
    }

}
