using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Information;

public class GardenBlock : MonoBehaviour
{

    public int id;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //--- ブロックがクリックされたときの挙動 ---//
    public void onClick()
    {
        Debug.Log("GardenBlock:onClick:" + id);
        GameObject.Find("Main Camera").GetComponent<NogyoMgr>().selectPlant(id);
    }
}
