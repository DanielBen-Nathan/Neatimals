using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour {
    private float time = 0.0f;
    public float maxTime = 10.0f;
    public float t = 10.0f;
    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * -t);
        /* Debug.Log(time);
         if (time >= 1)
         {
             // GetComponent<Rigidbody>().AddRelativeTorque(Vector3.forward * 100);
             ConfigurableJoint configurableJointComp = GetComponent<ConfigurableJoint>();
             SoftJointLimit softJointLimit = new SoftJointLimit();
             //softJointLimit.limit =  outputs[i * 3]*-90+ configurableJointComp.lowAngularXLimit.limit;// *-180;
             float x = Random.Range(-150.0f, 150.0f); ;
             softJointLimit.limit = x;

             configurableJointComp.highAngularXLimit = softJointLimit;
             SoftJointLimit softJointLimit2 = new SoftJointLimit();
             softJointLimit2.limit = x+1;
             configurableJointComp.lowAngularXLimit = softJointLimit2;

             //GetComponent<ConfigurableJoint>().targetAngularVelocity = new Vector3(-1000, -1000, -1000);
             //Debug.Log(GetComponent<ConfigurableJoint>().targetAngularVelocity);
             //Debug.Log(GetComponent<Rigidbody>().velocity);
             time = 0.0f;
         }
         time += Time.fixedDeltaTime;*/
    }
}
