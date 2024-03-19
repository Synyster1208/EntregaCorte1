using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clocks : MonoBehaviour
{
   
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Player"));
        {
            GameManager.obj.TimerCdTime++;
            Destroy(gameObject, 0.2f);
        }
    }

}
