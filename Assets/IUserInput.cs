using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{
    [Header("===== Output signals =====")]
    public float Dup;   //将wsad转化为signal信号
    public float Dright;
    public float Dmag; //用勾股定理算出Dup和Dright两个值的斜边作为人物移动的综合速度
    public Vector3 Dvec;    //人物移动的方向
    public float Jup;
    public float Jright;


    // 1. pressing signal
    public bool run;
    public bool defense;
    // 2. trigger once signal
    public bool jump;
    protected bool lastJump;  //避免连按
    public bool attack;
    protected bool lastAttack;  //避免连按
    public bool roll;
    public bool lockon; //锁定信号量
    // 3. double trigger

    [Header("===== Others =====")]
    public bool inputEnabled = true; // Flag

    protected float targetDup;
    protected float targetDright;
    protected float velocityDup;
    protected float velocityDright;
    protected Vector2 SquareToCircle(Vector2 input) {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }
}
