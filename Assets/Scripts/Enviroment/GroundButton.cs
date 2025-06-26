using System;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider2D))]
public class GroundButton : MonoBehaviour
{
    [SerializeField] private UnityEvent _onButtonActive;
    [SerializeField] private UnityEvent _onButtonPerformed;
    [SerializeField] private UnityEvent _onButtonCanceled;

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
