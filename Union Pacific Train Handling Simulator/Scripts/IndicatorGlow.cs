using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using System;

public class IndicatorGlow : MonoBehaviour
{
    public Sprite indicatorGlow0;
    public Sprite indicatorGlow25;
    public Sprite indicatorGlow50;
    public Sprite indicatorGlow75;
    public Sprite indicatorGlow100;
    private SpriteRenderer spriteRenderer;
    private bool glow = false;
    public int duration = 20;   // frames
    private int timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = duration;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (glow)
        {
            timer--;

            if((timer < 15) && (timer >= 10)) { Fade(75); }

            else if ((timer < 10) && (timer >= 5)) { Fade(50); }

            else if ((timer < 5) && (timer >= 0)) { Fade(25); }

            else if (timer == 0) { Fade(0); }
        }
    }

    public void Glow()
    {
        spriteRenderer.sprite = indicatorGlow100;
        glow = true;
        timer = duration;
    }

    void Fade(int level)
    {
        switch(level)
        {
            case 75:
                spriteRenderer.sprite = indicatorGlow75;
                break;

            case 50:
                spriteRenderer.sprite = indicatorGlow50;
                break;

            case 25:
                spriteRenderer.sprite = indicatorGlow25;
                break;

            default:
                spriteRenderer.sprite = indicatorGlow0;
                glow = false;
                timer = duration;
                break;

        }
    }
}
