using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlindPanel : MonoBehaviour
{
    float alfa;
    bool turn;

    atBlind func = null;

    public delegate void atBlind(); 

    // Start is called before the first frame update
    void Start()
    {
        turn = false;
        alfa = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, alfa);

        if (alfa < 0f)
        {
            Destroy(gameObject);
        }else if(alfa < 1f && !turn)
        {
            alfa += 0.05f;
        }else if (alfa < 1f && turn)
        {
            alfa -= 0.05f;
        }
        else
        {
            turn = true;
            alfa -= 0.05f;

            if (func != null) func();
        }
    }


    public void initfornogyo(atBlind func)
    {
        this.func = func;

    }
}
