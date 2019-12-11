using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wallet{

    public int money;

    public Wallet(int money)
    {
        this.money = money;
    }

    /*
     * come : 増減額
     */
    public void comeMoney(int come)
    {
        money += come;
    }
}
