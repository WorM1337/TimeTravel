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

    public void ChangeAngleOfFilling(float k) // От 0 до 1, где 1 - это заполненная окружность
    {
        _progressFillCircle.fillAmount = k;
    }
}
