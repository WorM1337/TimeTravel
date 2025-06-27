using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UsualDamager : DamageObstacle
{
    public float DamageCulldown = 2f;
    [NonSerialized] public bool IsAbleToDealDamage = true;
    [SerializeField] private float _damage = 15f;

    private Coroutine _culldownDamageProcess;

    public override void DealDamage(IDamageable obj)
    {
        obj.TakeDamage(_damage);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsAbleToDealDamage)
        {
            if (collision.collider.GetComponent<IDamageable>() is IDamageable obj)
            {
                DealDamage(obj);
                _culldownDamageProcess = StartCoroutine(DamageCulldownProcess());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<IDamageable>() is IDamageable obj)
        {
            if(_culldownDamageProcess != null) StopCoroutine(_culldownDamageProcess);
            IsAbleToDealDamage = true;
        }
    }
    public IEnumerator DamageCulldownProcess()
    {
        IsAbleToDealDamage = false;

        yield return new WaitForSeconds(DamageCulldown);

        IsAbleToDealDamage = true;
    }
}
