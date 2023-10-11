using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] protected HealthBar HealthBar;
    [SerializeField] protected CombatText combatTextPrefab;

    private float hp;
    public bool IsDead => hp <= 0;
    // Start is called before the first frame update
    private string currentAnimName;
    private void Start()
    {
        OnInit();
    }
    public virtual void OnInit()
    {
        hp = 100;
        HealthBar.OnInit(100, transform);
    }

    public virtual void OnDespawn()
    {

    }
    protected virtual void OnDeath()
    {
        ChangeAnim("die");
        Invoke(nameof(OnDespawn),2f);
    }
    protected void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            anim.ResetTrigger(animName);

            currentAnimName = animName;

            anim.SetTrigger(currentAnimName);
        }
    }
    public void OnHit(float damage)
    {
        //GameObject obj = Instantiate(blood, transform.position, transform.rotation);
        //DestroyObject(obj, 0.75f);
        Debug.Log("hit");
        if (!IsDead)
        {
            hp -= damage;
            if (IsDead)
            {
                hp = 0;
                OnDeath();
            }
            HealthBar.SetNewHP(hp);
            Instantiate(combatTextPrefab,transform.position + Vector3.up,Quaternion.identity).OnInit(damage);
        }
    }

    

    


}
