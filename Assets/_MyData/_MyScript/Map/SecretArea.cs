using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretArea : MonoBehaviour
{
    public float fadeDuration = 1f;

    SpriteRenderer spriteRenderer;
    Color hiddenColor;

    Coroutine currentCoroutine;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hiddenColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (this.currentCoroutine != null)
            {
                StopCoroutine(this.currentCoroutine);
            }
            currentCoroutine = StartCoroutine(this.FadeSprite(true)); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!gameObject.activeInHierarchy) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (this.currentCoroutine != null)
            {
                StopCoroutine(this.currentCoroutine);
            }
            currentCoroutine = StartCoroutine(this.FadeSprite(false));
        }
    }

    IEnumerator FadeSprite(bool fadeOut)
    {
        Color startColor = spriteRenderer.color;
        Color endColor = fadeOut ? new Color(hiddenColor.r, hiddenColor.g, hiddenColor.b, 0f) : hiddenColor;

        float timeFading = 0f;

        while (timeFading <  fadeDuration)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, timeFading/ fadeDuration);
            timeFading += Time.deltaTime;
            yield return null;
        }
    }

}
