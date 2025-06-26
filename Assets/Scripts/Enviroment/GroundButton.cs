using System;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class GroundButton : MonoBehaviour
{
    [SerializeField] private event Action _onButtonActive;
    [SerializeField] private event Action _onButtonPerformed;
    [SerializeField] private event Action _onButtonCanceled;

    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _onButtonPerformed?.Invoke();
        //_activeAnim.SetBool("");
        // Анимация нажатия


    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        _onButtonActive?.Invoke();
        //_activeAnim.SetBool("");
        // Анимация активности
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _onButtonCanceled?.Invoke();
        //_activeAnim.SetBool("");
        // Анимация отпускания
    }
}
