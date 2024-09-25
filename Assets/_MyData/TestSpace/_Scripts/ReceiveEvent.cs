using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveEvent : MonoBehaviour
{
    public void Awake()
    {
        HomeEvent.action += AddMe;
    }

    protected string AddMe(int intNum)
    {
        intNum += 23;
        return intNum.ToString();
    }
}
