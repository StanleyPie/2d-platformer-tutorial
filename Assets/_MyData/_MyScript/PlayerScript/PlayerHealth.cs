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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAI enemy = collision.GetComponent<EnemyAI>();
        if (enemy)
        {
            TakeDamage(enemy.damage);
        }
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
