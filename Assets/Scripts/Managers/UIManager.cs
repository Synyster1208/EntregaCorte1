using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager obj;
    [Header("Sliders")]
    public Slider TimeSd;
    public Image clock;

    [Header("Colors")]
    public Color Green;
    public Color Yellow;
    public Color Red;

    void Awake()
    {
        obj = this;
    }
    void Start()
    {
        clock.color=Green;
    }

    public void SlidersValue()
    {
       TimeSd.value = GameManager.obj.TimerCdTime;
    }
    void Update()
    {
        SlidersValue();
        SliderColor();
    }

    public void SliderColor()
    {
        if(GameManager.obj.TimerCdTime > 30f)
        clock.color = Green;
        if(GameManager.obj.TimerCdTime > 10f && GameManager.obj.TimerCdTime < 30f)
        clock.color = Yellow;
        if(GameManager.obj.TimerCdTime < 10f)
        clock.color = Red;
    }

    void Destroy()
    {
        obj = null;
    }
}
