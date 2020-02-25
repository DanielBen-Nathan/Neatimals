using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainRand : MonoBehaviour
{
    private GameObject agent;
    private Genome g1;
    private float time = 2.0f;
    private bool ready = true;
    private float torque = 50;
    private float r1 = 0;
    private float r2 = 0;
    private float r3 = 0;
    private float r4 = 0;
    private List<float> outputs;
    // Use this for initialization
    void Start()
    {
        agent = gameObject;
       

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (ready)
        {
            r1 = Random.Range(-1.0f, 1.0f);
            r2 = Random.Range(-1.0f, 1.0f);
            r3 = Random.Range(-1.0f, 1.0f);
            r4 = Random.Range(-1.0f, 1.0f);


          


        }
        agent.transform.GetChild(0).GetComponent<Rigidbody>().AddTorque(transform.right * r1 * torque);
        agent.transform.GetChild(1).GetComponent<Rigidbody>().AddTorque(transform.right * r2 * torque);
        agent.transform.GetChild(2).GetComponent<Rigidbody>().AddTorque(transform.right * r3 * torque);
        agent.transform.GetChild(3).GetComponent<Rigidbody>().AddTorque(transform.right * r4 * torque);
        StartCoroutine(WaitBetweenShots());
        // Debug.Log(agent.transform.GetChild(0).name);


    }

   

   
    IEnumerator WaitBetweenShots()
    {

        ready = false;

        yield return new WaitForSeconds(time);

        ready = true;



    }

}