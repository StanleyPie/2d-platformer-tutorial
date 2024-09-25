using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GemScript : MonoBehaviour, IItems
{
    public static event Action<int> onGemCollect;
    public int worth = 10;
    public void Collected()
    {
        onGemCollect.Invoke(worth);
        Destroy(transform.gameObject);
    }

}
