using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager obj;

    [Header("Values")]
    public float CdTime;
    public float TimerCdTime;
    

    void Awake()
    {
        obj = this;
    }
    
    void Start()
    {
        TimerCdTime = CdTime;
    }


    
    void Update()
    {
        TimerCdTime -= Time.deltaTime;
        if(TimerCdTime <= 0)
          ResetScene();
    }

    public void ResetScene()
    {
      SceneManager.LoadScene("GameOver");
    }

    void OnDestroy()
    {
        obj = null;
    }
}
