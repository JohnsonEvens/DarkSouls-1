using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : IUserInput {

    // Variable
    [Header("===== Key settings =====")]
    public string keyUp = "w";  //初始值
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public string keyA = "left shift";
    public string keyB = "space";
    public string keyC;
    public string keyD;

    public string keyJUp;   //控制镜头运动
    public string keyJDown;
    public string keyJRight;
    public string keyJLeft;

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonC = new MyButton();
    public MyButton buttonD = new MyButton();


    [Header("===== Mouse settings =====")]
    public bool mouseEnable = false;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

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

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        buttonA.Tick(Input.GetKey(keyA));
        buttonB.Tick(Input.GetKey(keyB));
        buttonC.Tick(Input.GetKey(keyC));
        buttonD.Tick(Input.GetKey(keyD));

        if (mouseEnable == true) {
            //将视角控制绑定到鼠标
            Jup = Input.GetAxis("Mouse Y") * mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X") * mouseSensitivityX;
        }
        else {
            //将视角控制绑定到键盘
            Jup = (Input.GetKey(keyJUp) ? 1.0f : 0) - (Input.GetKey(keyJDown) ? 1.0f : 0);
            Jright = (Input.GetKey(keyJRight) ? 1.0f : 0) - (Input.GetKey(keyJLeft) ? 1.0f : 0);
        }

        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);

        if (!inputEnabled) {
            targetDup = 0;
            targetDright = 0;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);  //使用SmoothDamp使信号在一定时间内平滑地变为1
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        //使用两个[-1，1]的信号量来指代移动速度，斜方向最大值为根号2不为1，需要进行转换为圆形
        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;  //标量乘上坐标轴得到移动方向

        //run = Input.GetKey(keyA);
        run = (buttonB.IsPressing && !buttonB.IsDelaying) || buttonB.IsExtending;
        //defense = Input.GetKey(keyD);
        defense = buttonD.IsPressing;

        //bool newJump = Input.GetKey(keyB);
        //jump = (newJump != lastJump && newJump == true);    //按下和抬起会触发两次，我们只需要第一次
        //lastJump = newJump;
        jump = buttonB.OnPressed && buttonB.IsExtending;

        //bool newAttack = Input.GetKey(keyC);
        //attack = (newAttack != lastAttack && newAttack == true);    //按下和抬起会触发两次，我们只需要第一次
        //lastAttack = newAttack;
        attack = buttonC.OnPressed;

        roll = buttonB.OnReleased && buttonB.IsDelaying;
    }

    //private Vector2 SquareToCircle(Vector2 input) {
    //    Vector2 output = Vector2.zero;

    //    output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
    //    output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

    //    return output;
    //}
}
