using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    int currentHealth;

    public HeartUI healthUI;

    SpriteRenderer spriteRenderer;

    public static event Action OnPlayerDied;

    private void Start()
    {
        this.ResetHealth();

        spriteRenderer = GetComponent<SpriteRenderer>();
        GameCtrl.OnReset += this.ResetHealth;
        HealthItem.onHealthCollect += this.Heal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("09");

        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if (enemy)
        {
            TakeDamage(enemy.damage);
            SoundEffectManager.Play("PlayerHit");
        }

        Trap trap = collision.gameObject.GetComponent<Trap>();
        if (trap && trap.damage > 0)
        {
            TakeDamage(trap.damage);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("09");
    //    EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
    //    if (enemy)
    //    {
    //        Debug.LogWarning(collision.ToString());
    //        TakeDamage(enemy.damage);
    //    }

    //    Trap trap = collision.gameObject.GetComponent<Trap>();
    //    if (trap && trap.damage > 0)
    //    {
    //        Debug.LogWarning(collision.ToString());
    //        TakeDamage(trap.damage);
    //    }
    //}
    public void Heal(int amount)
    {
        this.currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthUI.UpdateHearts(currentHealth);
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        StartCoroutine(this.FlashRed());

        if (currentHealth <= 0)
        {
            OnPlayerDied.Invoke();
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }
}
