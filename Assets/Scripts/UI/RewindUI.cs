using UnityEngine;
using UnityEngine.UI;

public class RewindUI : MonoBehaviour
{
    [SerializeField] private Image _rewindIcon;
    [SerializeField] private Image _progressFillCircle;

    public void ChangeRewindIconColor(Color color)
    {
        _rewindIcon.color = color;
    }

    public void ChangeAngleOfFilling(float k) // �� 0 �� 1, ��� 1 - ��� ����������� ����������
    {
        _progressFillCircle.fillAmount = k;
    }
}
