using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeEvent : MonoBehaviour
{
    public static event Func<int, string> action;
    void Start()
    {
        DoEvent();
    }

    void DoEvent()
    {
        string abc = action.Invoke(12);
        Debug.Log(abc);
    }
}
