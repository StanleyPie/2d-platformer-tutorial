using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour, IItems
{
    public int healAmount = 1;
    public static event Action<int> onHealthCollect;
    public void Collected()
    {
        onHealthCollect.Invoke(healAmount);
        Destroy(gameObject);
    }
}
