using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverGameManager : SingletonMonoBehaviour<DeliverGameManager>
{
    public float speed { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        speed = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
