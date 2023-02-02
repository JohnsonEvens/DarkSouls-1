using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;
    public bool IsExtending = false;    //制作double trigger的信号量，按键第一次松开等待第二次按下所处的状态 
    public bool IsDelaying = false;  //制作long press的信号量，按键按住延迟一会后才出现的状态，检测按键是否处于长按状态

    public float extendingDuration = 0.15f;
    public float delayingDuration = 0.15f;

    private bool curState = false;
    private bool lastState = false;

    private MyTimer extTimer = new MyTimer();   //计时器
    private MyTimer delayTimer = new MyTimer();

    public void Tick(bool input) {
        extTimer.Tick();
        delayTimer.Tick();

        curState = input;

        IsPressing = curState;

        OnPressed = false;
        OnReleased = false;
        if(curState != lastState) {
            if(curState == true) {
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration);   //按钮按下一瞬间启动计时器
            }
            else {
                OnReleased = true;
                StartTimer(extTimer, extendingDuration);    //按钮松开一瞬间启动计时器
            }
        }

        lastState = curState;

        IsExtending = (extTimer.state == MyTimer.STATE.RUN);
        IsDelaying = (delayTimer.state == MyTimer.STATE.RUN);
    }

    private void StartTimer(MyTimer timer, float duration) {
        timer.duration = duration;
        timer.Go();
    }
}
