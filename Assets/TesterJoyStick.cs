using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterJoyStick : MonoBehaviour
{
    //Start is called before the first frame update
    //void Start() {

    //}

    // Update is called once per frame
    void Update() {
        print(Input.GetAxis("RT"));
        //print(Input.GetAxis("RB"));
        //print(Input.GetAxis("Jright"));
        //print(Input.GetAxis("Jup"));
    }
}
