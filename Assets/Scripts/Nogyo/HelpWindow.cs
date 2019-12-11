using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class HelpWindow : MonoBehaviour
{
    public bool active;

    void Start()
    {
        active = false;
        Activate(false);
    }


    public void Activate(bool activate)
    {
        gameObject.GetComponent<Image>().enabled = activate;
        gameObject.transform.GetChild(0).gameObject.SetActive(activate);

        this.active = activate;
    }


}
