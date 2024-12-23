using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IItems items = collision.GetComponent<IItems>();
        if (items != null)
        {
            items.Collected();
        }
    }
}
