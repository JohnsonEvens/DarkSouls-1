using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    private IUserInput pi;
    public float horizontalSpeed = 120.0f;   //水平移动镜头速度
    public float verticalSpeed = 80.0f; //垂直移动镜头速度
    public float cameraDampValue = 0.05f;   //摄像头追人物的延迟速度
    public Image lockDot;
    public bool lockState;

    private GameObject playerHandle;
    private GameObject cameraHandle;
    private float tempEulerX;   //垂直方向镜头角度
    private GameObject model;
    private new GameObject camera;

    private Vector3 cameraDampVelocity;

    [SerializeField]
    private LockTarget lockTarget;


    // Start is called before the first frame update
    void Start() {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20;    //初始值
        ActorController ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        pi = ac.pi; //这里的pi是从ActorController里获取的，而ActorController里pi也是在awake中获取的，awake脚本同时执行，会导致这里pi取到null，故改为start
        camera = Camera.main.gameObject;
        lockDot.enabled = false;
        lockState = false; 

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(lockTarget == null) {
            Vector3 tempModelEuler = model.transform.eulerAngles;   //记录模型的角度，最后再赋值回去，这样人物就不会随着镜头转了

            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);

            tempEulerX -= pi.Jup * verticalSpeed * Time.deltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);  //将旋转视角限制在区间内
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;
        
        }
        else {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;  //转成平面向量
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform.position);   //锁定状态下，让摄像头看着目标脚部坐标
        }

        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position, ref cameraDampVelocity, cameraDampValue);
        //camera.transform.eulerAngles = transform.eulerAngles;
        camera.transform.LookAt(cameraHandle.transform);
        
    }

    void Update() {
        if(lockTarget != null) {
            lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight, 0));
            //超过一定距离，自动解锁
            if(Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10.0f) {
                lockTarget = null;
                lockDot.enabled = false;
                lockState = false;
            }
        }    
    }

    public void LockUnlock() {

        Vector3 modelOrigin1 = model.transform.position; //获取人物当前坐标
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0); //在人物当前坐标的基础上往上高一个单位的坐标点
        Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;  //box中心为人物前方五个单位
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation, LayerMask.GetMask("Enemy"));
        
        if(cols.Length == 0) {
            lockTarget = null;  //没有目标，就解锁
            lockDot.enabled = false;
            lockState = false;
        }
        else {
            foreach (var col in cols) {
                if(lockTarget != null && lockTarget.obj == col.gameObject) {
                    lockTarget = null;  //锁到同一个东西两次，就解锁
                    lockDot.enabled = false;
                    lockState = false;
                    break;
                }
                //print(col.name);
                lockTarget = new LockTarget(col.gameObject, col.bounds.extents.y);  //collider的bounds.extents的y分量是半高
                lockDot.enabled = true;
                lockState = true;
                //lockDot.transform.position = Camera.main.WorldToScreenPoint(lockTarget.transform.position);
                break;
            }
        }
    }

    private class LockTarget {
        public GameObject obj;
        public float halfHeight;

        public LockTarget(GameObject _obj, float _halfHeight) {
            obj = _obj;
            halfHeight = _halfHeight;
        }
    }
}
