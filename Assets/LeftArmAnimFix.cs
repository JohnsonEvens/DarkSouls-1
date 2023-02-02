using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{
    private Animator anim;

    public Vector3 a;

    void Awake() {
        anim = GetComponent<Animator>();
    }
    void OnAnimatorIK(int layerIndex) {
        if(anim.GetBool("defense") == false) {
            Transform leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm); //获取骨头的形变组件
            leftLowerArm.localEulerAngles += 0.75f * a;
            anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
        }
        
    }
}
