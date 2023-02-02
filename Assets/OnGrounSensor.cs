using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGrounSensor : MonoBehaviour
{
    public CapsuleCollider capcol;
    public float offset = 0.1f; //把碰撞胶囊变得细长

    private Vector3 point1;
    private Vector3 point2;
    private float radius;

    // Start is called before the first frame update
    void Awake()
    {
        radius = capcol.radius - 0.05f;
    }

    void FixedUpdate()
    {
        point1 = transform.position + transform.up * (radius - offset);    //计算胶囊的两个点
        point2 = transform.position + transform.up * (capcol.height - offset) - transform.up * radius;

        Collider[] outputCols = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground"));
        if(outputCols.Length != 0) {
            SendMessageUpwards("IsGround");
        }
        else {
            SendMessageUpwards("IsNotGround");
        }
    }
}
