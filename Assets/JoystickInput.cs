﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInput : IUserInput 
{
    [Header("===== Joystick settings =====")]
    public string axisX = "axisX";
    public string axisY = "axisY";
    public string axisJright = "axis4";
    public string axisJup = "axis5";
    public string btnA = "btn0";
    public string btnB = "btn1";
    public string btnX = "btn2";
    public string btnY = "btn3";
    public string btnLB = "btn4";
    public string btnRB = "btn5";
    public string btnJstick = "btn9";

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonX = new MyButton();
    public MyButton buttonY = new MyButton();
    public MyButton buttonLB = new MyButton();
    public MyButton buttonRB = new MyButton();
    public MyButton buttonJstick = new MyButton();


    //[Header("===== Output signals =====")]
    //public float Dup;   //将wsad转化为signal信号
    //public float Dright;
    //public float Dmag; //用勾股定理算出Dup和Dright两个值的斜边作为人物移动的综合速度
    //public Vector3 Dvec;    //人物移动的方向
    //public float Jup;
    //public float Jright;


    //// 1. pressing signal
    //public bool run;
    //// 2. trigger once signal
    //public bool jump;
    //private bool lastJump;  //避免连按
    //public bool attack;
    //private bool lastAttack;  //避免连按
    //// 3. double trigger

    //[Header("===== Others =====")]
    //public bool inputEnabled = true; // Flag

    //private float targetDup;
    //private float targetDright;
    //private float velocityDup;
    //private float velocityDright;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        buttonA.Tick(Input.GetButton(btnA));
        buttonB.Tick(Input.GetButton(btnB));
        buttonX.Tick(Input.GetButton(btnX));
        buttonY.Tick(Input.GetButton(btnY));
        buttonLB.Tick(Input.GetButton(btnLB));
        buttonRB.Tick(Input.GetButton(btnRB));
        buttonJstick.Tick(Input.GetButton(btnJstick));

        //print(buttonA.IsExtending && buttonA.OnPressed);

        Jup = Input.GetAxis(axisJup);
        Jright = Input.GetAxis(axisJright);

        targetDup = Input.GetAxis(axisY);
        targetDright = Input.GetAxis(axisX);

        if (!inputEnabled) {
            targetDup = 0;
            targetDright = 0;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);  //使用SmoothDamp使信号在一定时间内平滑地变为1
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        //使用两个[-1，1]的信号量来指代移动速度，斜方向最大值为根号2不为1，需要进行转换为圆形
        Vector2 tempDAxis = new Vector2(Dright, Dup);
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;  //标量乘上坐标轴得到移动方向

        //run = Input.GetButton(btnA);
        run = (buttonA.IsPressing && !buttonA.IsDelaying) || buttonA.IsExtending;
        //defense = Input.GetButton(btnLB);
        defense = buttonLB.IsPressing;

        //bool newJump = Input.GetButton(btnB);
        //jump = (newJump != lastJump && newJump == true);    //按下和抬起会触发两次，我们只需要第一次
        //lastJump = newJump;
        jump = buttonA.OnPressed && buttonA.IsExtending;

        //bool newAttack = Input.GetButton(btnX);
        //attack = (newAttack != lastAttack && newAttack == true);    //按下和抬起会触发两次，我们只需要第一次
        //lastAttack = newAttack;
        attack = buttonX.OnPressed;

        roll = buttonA.OnReleased && buttonA.IsDelaying;

        lockon = buttonJstick.OnPressed;
    }

    //private Vector2 SquareToCircle(Vector2 input) {
    //    Vector2 output = Vector2.zero;

    //    output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
    //    output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

    //    return output;
    //}
}
