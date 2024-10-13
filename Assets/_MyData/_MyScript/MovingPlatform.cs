using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    Vector3 nextPosition;

    private void Start()
    {
        nextPosition = pointB.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!transform.gameObject.activeInHierarchy) return;
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!Application.isPlaying) return;

        if (collision.gameObject.CompareTag("Player") && transform.gameObject.activeSelf)
        {
            if (!transform.gameObject.activeInHierarchy) return;
            collision.transform.parent = null;
        }
    }
}
