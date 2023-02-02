using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour {

    public GameObject model;
    public CameraController camcon;
    public IUserInput pi;
    public float walkSpeed = 2.2f;
    public float runMultiplier = 2.5f;  //跑步速度增加倍率
    public float jumpVelocity = 3.0f;   //跳跃垂直速度控制
    public float rollVelocity = 2.5f;   //翻滚冲量

    [Space(10)] //插入10个空格
    [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;  //摩擦力预设
    public PhysicMaterial frictionZero;

    //[SerializeField]    //将private属性显示在编译器上
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 plannaVec;
    private Vector3 thrustVec;
    private bool canAttack; //当前是否能攻击

    private bool lockPlanar = false;    //跳跃起来后锁住平面位移向量，这样跳跃过程能维持起跳瞬间的速度，形成平面移动
    private bool trackDirection = false;    //设置为true时，应该去追踪plannaVec的方向

    private CapsuleCollider col;    //取得碰撞胶囊

    private float lerpTarget;   //攻击时使用插值线性改变attack层权重

    private Vector3 deltaPos;   //记录每次FixedUpdate后坐标应该移动多少

    // Start is called before the first frame update
    void Awake() {   //awake>enable>start awake阶段先把要用的组件抓好，进入start阶段就不会缺失
        IUserInput[]  inputs = GetComponents<IUserInput>();   //因为就在本身上的对象，GetComponent前不加东西
        foreach (var input in inputs) {
            if(input.enabled == true) {
                pi = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update() {

        if (pi.lockon) {
            camcon.LockUnlock();
        }

        if (camcon.lockState == false) {
            float targetRunMulti = ((pi.run) ? 2.0f : 1.0f);    //使用插值润滑从走到跑的动作衔接
            anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), targetRunMulti, 0.1f));
        }
        else {
            Vector3 localDVec = transform.InverseTransformVector(pi.Dvec);  //Dvec是global的，要转换
            anim.SetFloat("forward", localDVec.z * ((pi.run) ? 2.0f : 1.0f));
            anim.SetFloat("right", localDVec.x * ((pi.run) ? 2.0f : 1.0f));
        }

        anim.SetBool("defense", pi.defense);

        //if (rigid.velocity.magnitude > 1.0f) {  //velocity.magnitude返回标量速度大小
        //    anim.SetTrigger("roll");
        //}
        if (pi.roll || rigid.velocity.magnitude > 7f) {
            anim.SetTrigger("roll");
            canAttack = false;
        }

        if (pi.jump) {
            anim.SetTrigger("jump");
            canAttack = false;
        }

        if (pi.attack && CheckState("ground") && canAttack) {
            anim.SetTrigger("attack");
        }
        if (camcon.lockState == false) {
            if (pi.Dmag > 0.1f) {  //规避松开按键人物面向重置为前的bug
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);    //使用插值优化人物方向改变
            }
            if (lockPlanar == false) {
                plannaVec = pi.Dmag * model.transform.forward * walkSpeed * ((pi.run) ? runMultiplier : 1.0f);
            }
        }
        else {
            if (trackDirection == false) {
                model.transform.forward = transform.forward;
            }
            else {
                model.transform.forward = plannaVec.normalized;
            }

            if(lockPlanar == false) {
                plannaVec = pi.Dvec * walkSpeed * ((pi.run) ? runMultiplier : 1.0f);
            }
        }
    }
    void FixedUpdate() { //物理引擎每秒五十次，速度不同不能放在Update里,Update是每帧执行一次，FixedUpdate是固定每0.02秒执行一次（unity默认值）
        rigid.position += deltaPos; //更新位置前，先把OnAnimatorMove传来的位移加进去，下面变更位置是在此基础上

        //rigid.position += movingVec * Time.fixedDeltaTime;
        rigid.velocity = new Vector3(plannaVec.x, rigid.velocity.y, plannaVec.z) + thrustVec; //两种写法，要么修改位置，要速度乘时间，要么直接修改速度
        thrustVec = Vector3.zero;

        deltaPos = Vector3.zero;
    }

    private bool CheckState(string stateName, string layerName = "Base Layer") {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }

    /// <summary>
    /// 接受message函数区
    /// </summary>
    public void OnJumpEnter() {
        pi.inputEnabled = false;    //跳跃起来后锁住，禁止输入
        lockPlanar = true;  //跳跃过程中维持起跳瞬间的速度
        thrustVec = new Vector3(0, jumpVelocity, 0);
        trackDirection = true;
    }
/*    public void OnJumpExit() {
        pi.inputEnabled = true; //跳跃结束，解锁
        lockPlanar = false;
    }*/
    public void IsGround() {
        anim.SetBool("isGround", true);
    }
    public void IsNotGround() {
        anim.SetBool("isGround", false);
    }
    public void OnGroundEnter() {
        pi.inputEnabled = true; //跳跃结束，解锁
        lockPlanar = false;
        canAttack = true;
        col.material = frictionOne;
        trackDirection = false;
    }
    public void OnGroundExit() {
        col.material = frictionZero;
    }
    public void OnFallEnter() {
        pi.inputEnabled = false;    //不止跳跃，直接下落后也要锁住，禁止输入
        lockPlanar = true;  //下落过程中维持下落瞬间的水平速度，做抛物线坠落
    }
    public void OnRollEnter() {
        pi.inputEnabled = false;    //翻滚后锁住，禁止输入
        lockPlanar = true;  //翻滚过程中维持翻滚开始瞬间的速度
        thrustVec = new Vector3(0, rollVelocity, 0);
        trackDirection = true;
    }
    public void OnJabEnter() {
        pi.inputEnabled = false;    //后跳后锁住，禁止输入
        lockPlanar = true;  //后跳过程中维持后跳开始瞬间的速度
    }
    public void OnJabUpdate() {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity"); //以曲线形势修改后跳的冲量
    }
    public void OnAttack1hAEnter() {
        pi.inputEnabled = false;    //攻击后锁住，禁止输入
        lerpTarget = 1.0f;
    }
    public void OnAttack1hAUpdate() {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        //攻击时使用插值线性改变attack层权重
        //float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));
        //currentWeight = Mathf.Lerp(currentWeight, lerpTarget, 0.1f);
        //anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
    public void OnAttackIdleEnter() {
        pi.inputEnabled = true;
        lerpTarget = 0f;
    }
    public void OnAttackIdleUpdate() {
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
    public void OnUpdateRM(object _deltaPos) {
        //OnUpdateRM和FixedUpdate不是同步的，所以需要先累加着，等到FixedUpdate
        //只想作用于第三段攻击
        if(CheckState("attack1hC", "attack")) {
            deltaPos += (Vector3)_deltaPos; //这里需要拆箱unboxing，因为传来的是打包好的object类型
        }
    }
}
