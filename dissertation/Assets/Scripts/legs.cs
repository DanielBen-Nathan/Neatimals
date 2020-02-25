using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class legs : MonoBehaviour {
    private Rigidbody rb;
    public float torque;
    private bool ready = true;
    private bool first = true;
    private float right=0f;
    private float up=0f;
    private int time = 5;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
       
        //float turn = Input.GetAxis("Horizontal");
        //Debug.Log(turn);
        if (ready) {
            if (first) {
                right = Random.Range(-1.0f, 1.0f);
                up = Random.Range(-1.0f, 1.0f);
            }
            //Debug.Log(right);
            //rb.AddTorque(transform.right * right * torque);
           // rb.AddTorque(transform.forward * up * torque);
            StartCoroutine(WaitBetweenShots());
        }
       
    }

    IEnumerator WaitBetweenShots()
    {

        first = false;

        yield return new WaitForSeconds(time);

        first = true;



    }
}
