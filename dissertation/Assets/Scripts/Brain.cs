
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Brain : MonoBehaviour
{
    public int sensorNumber = 5;
    public float sensorDistance =10.0f;
    public float time = 0.1f;//0.5
    public float torque = 50;//50 or 15
    public bool disableOnLimbFall = false;
    public float maxSpeed = 50.0f;
    public bool coroutine = false;
    public Vector3[] senorPos;
    private GameObject agent;
    private Genome g1;
    private bool ready = false;
    private List<float> outputs2;
    private float[] outputs;
    private int inputSize;
    private int outputSize;
    private int nodes = 0;
    private int limbs;
    private float[] oldVelocities;
    public int id;
    private int gap;
    private List<GameObject> limbObjects;
    private float timeBeforeFall = 0.0f;
    public bool fallen = false;
    private GameObject pf;
    private Neat neat;
    private float floorSensor = 0.0f;
    private Generate gen;
    private float reward = 0f;
    private bool first = true;
    private float[] hits;
    private Vector3 sensorOffset = new Vector3(0,0.35f,0);
    private bool sensors;

    public GameObject harness;

    //private float memory = 0.0f;


    public void Setsensors(bool sensors)
    {
        this.sensors = sensors;

    }
    


    public void SetGenome(Genome genome)
    {
        g1 = genome;

    }
    public Genome GetGenome()
    {
        return g1;

    }
    public void SetId(int id) {
        this.id = id;

    }
    public bool GetFallen() {
        return fallen;
    }
    public void SetFallen(bool fallen)
    {
        this.fallen = fallen;

    }
    public float GetReward() {
        return reward;

    }
    public void ResetReward()
    {
        reward = 0f;

    }
    // Use this for initialization
    void Start()
    {
        senorPos = new Vector3[sensorNumber];
        /*
        senorPos[0] = -Vector3.up;
        senorPos[1] = -Vector3.right;
        senorPos[2] = Vector3.right;
        senorPos[3] = new Vector3(1, -1, 0);
        senorPos[4] = new Vector3(-1, -1, 0);
        
        senorPos[0] = -Vector3.forward;
        senorPos[1] = -Vector3.right;
        senorPos[2] = Vector3.right;
        senorPos[3] = new Vector3(1, 0, -1);
        senorPos[4] = new Vector3(-1, 0, -1);
        */
        senorPos[0] = -Vector3.forward;
        senorPos[1] = Quaternion.Euler(0, -5, 0) * -Vector3.forward;
        senorPos[2] = Quaternion.Euler(0, 5, 0) * -Vector3.forward;
        senorPos[3] = Quaternion.Euler(0, -10, 0) * -Vector3.forward;
        senorPos[4] = Quaternion.Euler(0, 10, 0) * -Vector3.forward;
        senorPos[5] = Quaternion.Euler(0, -15, 0) * -Vector3.forward;
        senorPos[6] = Quaternion.Euler(0, 15, 0) * -Vector3.forward;

        pf = GameObject.Find("Scripts").GetComponent<Generate>().GetPF();
        limbObjects = new List<GameObject>();
      
        FindLimbs(transform);


        hits = new float[sensorNumber];

        //time += Random.Range(-0.001f, 0.001f);//vary time a little so all the agents dont run the network at once
        gen = GameObject.Find("Scripts").GetComponent<Generate>();
        gap = gen.GetGap();
        agent = gameObject;
        //GeneTracker gt = new GeneTracker();
        neat = GameObject.Find("Scripts").GetComponent<Neat>();
       
        inputSize = neat.GetInputSize();
        outputSize = neat.GetOutputSize();  
   
        g1 = neat.InitGenome();
        if (neat.speciation) {
            GameObject.Find("Scripts").GetComponent<ImplementFunctions>().SetSpecies(gameObject);
        }
       

        Debug.Log(g1);
        oldVelocities = new float[limbs * 3 + 3];
        oldVelocities[0] = 0.0f;
        oldVelocities[1] = 0.0f;
        oldVelocities[2] = 0.0f;
       
        for (int i = 0; i < oldVelocities.Length; i++) {
            oldVelocities[i] = 0.0f;
        }
        //Debug.Log( oldVelocities[0]);
        outputs = new float[outputSize];

    }

    // Update is called once per frame
    void Update()
    {
       
        if (harness != null) {
            
            harness.transform.position = Vector3.MoveTowards(harness.transform.position, new Vector3(harness.transform.position.x, harness.transform.position.y, transform.Find("hips").position.z), 50.0f* Time.deltaTime);
            
        }

        if (first) {
            StartCoroutine(WaitBetweenShots());
            first = false;
           
            oldVelocities = new float[limbs * 3 + 3];
            for (int i = 0; i < oldVelocities.Length; i++)
            {
                oldVelocities[i] = 0.0f;
            }
           
        }

       

        if (ready && !fallen)
        {
            if (sensors) {
                RaycastHit hit;
                for (int i = 0; i < senorPos.Length; i++)
                {
                    if (Physics.Raycast(transform.position + sensorOffset, (senorPos[i]), out hit, sensorDistance))//transform.TransformDirection(senorPos[i])

                    {
                        if (gen.showSensorRays)
                        {
                            Debug.DrawRay(transform.position + sensorOffset, (senorPos[i]) * hit.distance, Color.green);
                        }

                        // hit.transform.gameObject;
                        //hits[i] = (sensorDistance - hit.distance) / sensorDistance;
                        hits[i] = hit.distance/sensorDistance;
                        //Debug.Log("Did Hit");
                    }
                    else
                    {
                        if (gen.showSensorRays)
                        {
                            Debug.DrawRay(transform.position + sensorOffset, (senorPos[i]) * sensorDistance, Color.red);
                        }
                        //hits[i] = -1.0f;
                        hits[i] = 1.0f;
                    }
                }
            }
           
           


            float[] angles = new float[limbs * 3];
            float[] velocities = new float[limbs * 3];
            for (int i = 0; i < limbs; i++)
            {
                angles[i * 2] = limbObjects[i].transform.localRotation.eulerAngles.x / 360;
                angles[i * 2 + 1] = limbObjects[i].transform.localRotation.eulerAngles.y / 360;
                angles[i * 2 + 2] = limbObjects[i].transform.localRotation.eulerAngles.z / 360;
                velocities[i * 3] = limbObjects[i].GetComponent<Rigidbody>().velocity.x;
                velocities[i * 3 + 1] = limbObjects[i].GetComponent<Rigidbody>().velocity.y;
                velocities[i * 3 + 2] = limbObjects[i].GetComponent<Rigidbody>().velocity.z;
                
            }

           
            float[] inputs = new float[inputSize];
            inputs[0] = agent.GetComponent<Rigidbody>().velocity.x;
            inputs[1] = agent.GetComponent<Rigidbody>().velocity.y;
            inputs[2] = agent.GetComponent<Rigidbody>().velocity.z;
            inputs[3] = agent.transform.rotation.eulerAngles.x / 360;
            inputs[4] = agent.transform.rotation.eulerAngles.y / 360;
            inputs[5] = agent.transform.rotation.eulerAngles.z / 360;
            inputs[6] = gap*-id-agent.transform.position.x;
            inputs[7] = (inputs[0] - oldVelocities[0]) / time;
            inputs[8] = (inputs[1] - oldVelocities[1]) / time;
            inputs[9] = (inputs[2] - oldVelocities[2]) / time;

            //inputs[7] = (inputs[0] - oldVelocities[0]) / Time.deltaTime;
            //inputs[8] = (inputs[1] - oldVelocities[1]) / Time.deltaTime;
            //inputs[9] = (inputs[2] - oldVelocities[2]) / Time.deltaTime;


            inputs[10] = floorSensor;
            inputs[11] = agent.transform.position.y;
            int start;
            if (sensors)
            {
                for (int i = 0; i < sensorNumber; i++)
                {
                    inputs[12 + i] = hits[i];
                   

                }
                start = 12+sensorNumber;
            }
            else {
                start = 12;
            }
            
            //Debug.Log(inputs[6]);
            
            const int INPUTS_PER_LIMB = 10;
            for (int i = 0; i < limbs; i++)
            {

                inputs[i * INPUTS_PER_LIMB + start + 0] = angles[i * 2];
                inputs[i * INPUTS_PER_LIMB + start + 1] = angles[i * 2 + 1];
                inputs[i * INPUTS_PER_LIMB + start + 2] = angles[i * 2 + 2];
                inputs[i * INPUTS_PER_LIMB + start + 3] = velocities[i * 3];
                inputs[i * INPUTS_PER_LIMB + start + 4] = velocities[i * 3 + 1];
                inputs[i * INPUTS_PER_LIMB + start + 5] = velocities[i * 3 + 2];
               
                inputs[i * INPUTS_PER_LIMB + start + 6] = (velocities[i * 3] - oldVelocities[3 + i * 3]) / time;
                inputs[i * INPUTS_PER_LIMB + start + 7] = (velocities[i * 3 + 1] - oldVelocities[4 + i * 3]) / time;
                inputs[i * INPUTS_PER_LIMB + start + 8] = (velocities[i * 3 + 2] - oldVelocities[5 + i * 3]) / time;

                //inputs[i * INPUTS_PER_LIMB + 18] = (inputs[i + 12] - oldVelocities[3 + i * 3]) / Time.deltaTime;
                //inputs[i * INPUTS_PER_LIMB + 19] = (inputs[i + 13] - oldVelocities[3 + i * 3]) / Time.deltaTime;
                //inputs[i * INPUTS_PER_LIMB + 20] = (inputs[i + 14] - oldVelocities[3 + i * 3]) / Time.deltaTime;

                inputs[i * INPUTS_PER_LIMB + start + 9] = limbObjects[i].GetComponent<Leg>().ReadSensor();
            }

            for (int i = 0; i < inputSize; i++) {

                //inputs[i] = (float)System.Math.Round(inputs[i],1);
                //Debug.Log(inputs[i]+"   "+i);
                // inputs[i] = 0.5f;

            }
            //Debug.Log(inputs[0]);



            //outputs = Run(new float[inputSize] { r1, r2, r3, r4, r1, r2, r3, r4,r3,r4,r1,r2 ,r2,r3,r4,r1,r2,r3,r4,r1,r2,r3,r3,r4,r1,r2,r2});
            //Debug.Log(inputs.Length);
            //outputs = g1.Run(inputs);
            if (coroutine)
            {
                RunCo(inputs);
                if (id == 0) { 

                   // Debug.Log(outputs[0]);
                    //Debug.Log(outputs[1]);
                    //Debug.Log(outputs[2]);
                    //Debug.Log(outputs[5]);
                  
                }
            }
            else {
                outputs = g1.Run(inputs);
               
               
            }
           
            //Debug.Log(outputs[0]);
            //Debug.Log(torque);
            StartCoroutine(WaitBetweenShots());
            oldVelocities[0] = inputs[0];
            oldVelocities[1] = inputs[1];
            oldVelocities[2] = inputs[2];
            for (int i = 0; i < limbs; i++) {
                oldVelocities[3 + i * 3] = velocities[i * 3];
                oldVelocities[4 + i * 3] = velocities[i * 3+1];
                oldVelocities[5 + i * 3] = velocities[i * 3+2];

            }
            if (System.Double.IsNaN(transform.position.x) || System.Double.IsInfinity(transform.position.x)) {
                ResetPos();
                transform.position = new Vector3(id * gap, transform.position.y, transform.position.z);
            }

        }
        //Debug.Log(outputs[0]);
        //Debug.Log(outputs[1]);
        //Debug.Log(outputs[2]);
        //Debug.Log(outputs[3]);
        // Debug.Log(outputs.Length);
        //outputs = g1.GetOutputs();
        //ControlLimits();
        //ControlTorque();
        /*
        if (!fallen)
        {
            //ControlLimits();
            for (int i = 0; i < outputSize; i++)
            {
                
                //outputs[i] = (float)System.Math.Round(outputs[i], 1);

            }
            ControlForce();
            GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, maxSpeed);
        }*/
        // else {
        if (fallen) { 
            reward = -GameObject.Find("Scripts").GetComponent<ImplementFunctions>().deductRewardAmount;

        }
   
        if (gameObject.transform.position.y < -5)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            fallen = true;
        }
        if (gameObject.transform.position.z > 5)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            fallen = true;
        }
        if (!fallen) {
            timeBeforeFall += Time.deltaTime;
        }
       


    }
    public bool first2 = true;
    public float[] limbPos;
    public float strDeduct = 0.05f;
    public float str = 5;
    private void FixedUpdate()
    {
        if (!fallen)
        {
            //ControlLimits();
            for (int i = 0; i < outputSize; i++)
            {

                //outputs[i] = (float)System.Math.Round(outputs[i], 1);

            }
            //ControlForce();
            //ControlHinge();
            ControlConfig();
            //GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, maxSpeed);


            //transform.RotateAround(transform.rotation.eulerAngles, new Vector3(transform.rotation.eulerAngles.x,0,0),100);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, 0, 0), str);
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, 0);
            //transform.RotateAround(transform.position, Vector3.up,1000);
            //transform.position = Vector3.MoveTowards(transform.position, new Vector3(gap * -id, transform.position.y, transform.position.z), 1000);
           
            //Debug.DrawRay(transform.position, newDir, Color.red);

            // Move our position a step closer to the target.
            //transform.rotation = Quaternion.LookRotation(newDir);
            //transform.LookAt(newDir);
        }
        //
       
    }

    public void ControlConfig()
    {
       
        int adder = 0;
        for (int i = 0; i < limbs; i++)
        {
            
          
            ConfigurableJoint cj = limbObjects[i].GetComponent<ConfigurableJoint>();
            float useTorque = torque;
            if (limbObjects[i].GetComponent<Leg>().foot == true)
            {
                //useTorque = 10;
            }
            useTorque = limbObjects[i].GetComponent<Leg>().force;
            JointDrive jd = new JointDrive();
            jd.positionDamper = 10;
            jd.positionSpring = 100;
            jd.maximumForce = 9999999999999;
            
            cj.slerpDrive = jd;
         
            cj.rotationDriveMode = RotationDriveMode.Slerp;

            Vector3 rotX = new Vector3(outputs[adder]* useTorque, 0,0);
            Vector3 rotY = Vector3.zero;
            Vector3 rotZ = Vector3.zero;
            adder++;
            if (cj.angularYLimit.limit != 0)
            {
                rotY = new Vector3(0,outputs[adder] * useTorque, 0);
                adder++;

            }
            if (cj.angularZLimit.limit != 0)
            {
                rotZ = new Vector3(0, 0, outputs[adder] * useTorque);
                adder++;

            }

            cj.targetAngularVelocity = rotX + rotY + rotZ;
            //cj.targetAngularVelocity = new Vector3(outputs[adder] * 100, 0, 0);

        }


    }


    public void ControlLimits() {
        for (int i = 0; i < limbs; i++)
        {

           
            ConfigurableJoint configurableJointComp = limbObjects[i].GetComponent<ConfigurableJoint>();
            SoftJointLimit softJointLimit = new SoftJointLimit();
            //softJointLimit.limit = Mathf.Clamp(outputs[i * 3]*-torque+ configurableJointComp.lowAngularXLimit.limit, 0.0f, 90.0f);// *-180;
            softJointLimit.limit = outputs[i * 3] * 90;
            configurableJointComp.lowAngularXLimit = softJointLimit;


            //softJointLimit.limit = Mathf.Clamp(outputs[i * 3 + 1]*-torque + configurableJointComp.angularZLimit.limit, -90.0f, 0.0f);// * -180;
            softJointLimit.limit = outputs[i * 3 + 1] * 90;
            configurableJointComp.angularZLimit = softJointLimit;

            

            softJointLimit.limit = outputs[i * 3 + 2] * -90;
            //configurableJointComp.angularYLimit = softJointLimit;

          
        }
    }

    public void ControlTorque() {
        for (int i = 0; i < limbs; i++) {
            limbObjects[i].GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * outputs[i * 3] * torque);
            limbObjects[i].GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * outputs[i * 3 + 1] * torque);
            limbObjects[i].GetComponent<Rigidbody>().AddRelativeTorque(Vector3.forward * outputs[i * 3 + 2] * torque);

            
        }
       
    }
    public void ControlHinge() {

        int adder = 0;
        for (int i = 0; i < limbs; i++)
        {
            for (int j = 0; j < limbObjects[i].GetComponents<HingeJoint>().Length; j++) {
                JointMotor jm = new JointMotor();
                if (limbObjects[i].GetComponent<Leg>().foot == true)
                {
                    jm.force = 10;
                    jm.targetVelocity = outputs[adder] * 50;
                }
                else {
                    jm.force = 1000;
                    jm.targetVelocity = outputs[adder] * 500;
                }
               
                limbObjects[i].GetComponents<HingeJoint>()[j].motor = jm;
                limbObjects[i].GetComponents<HingeJoint>()[j].useMotor = true;
                /*
                JointSpring jm = new JointSpring();
                jm.spring = 100;
                jm.targetPosition = outputs[adder] * 90;
                limbObjects[i].GetComponents<HingeJoint>()[j].spring = jm;
                limbObjects[i].GetComponents<HingeJoint>()[j].useSpring = true;
                */
                adder++;
                
            }
           
           
            
            
        }


    }

    public void ControlForce()
    {


        int adder = 0;
        for (int i = 0; i < limbs; i++)
        {
          
            bool xChosen = false;
            bool yChosen = false;
            bool zChosen = false;
            if (limbObjects[i].GetComponent<ConfigurableJoint>().lowAngularXLimit.limit!=0 || limbObjects[i].GetComponent<ConfigurableJoint>().highAngularXLimit.limit != 0)
            {

                //limbObjects[i].GetComponent<Rigidbody>().AddForce(new Vector3(torque * outputs[i * 3], 0, torque * outputs[i * 3 + 2]));
                // GetComponent<Rigidbody>().AddForce(new Vector3(torque * -outputs[i * 3], 0, torque * -outputs[i * 3 + 2]));

                limbObjects[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, torque * outputs[i+adder]));
                //GetComponent<Rigidbody>().AddForce(new Vector3(0,0, torque * -outputs[i +adder] ));
                GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0, 0, torque * -outputs[i + adder]), limbObjects[i].GetComponent<Rigidbody>().transform.position);
                xChosen = true;
            }
            if (limbObjects[i].GetComponent<ConfigurableJoint>().angularZLimit.limit!=0)
            {

                //limbObjects[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, torque * outputs[i * 3 + 2]));
                //GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, torque * -outputs[i * 3 + 2]));
                int inc = 0;
                if (xChosen) {
                    inc++;
                }
                limbObjects[i].GetComponent<Rigidbody>().AddForce(new Vector3(torque * outputs[i +adder+inc] , 0,0 ));
               // GetComponent<Rigidbody>().AddForce(new Vector3(torque * -outputs[i + adder + inc] , 0,0));
                GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(torque * -outputs[i + adder + inc], 0, 0), limbObjects[i].GetComponent<Rigidbody>().transform.position);
                zChosen = true;
            }
            if (limbObjects[i].GetComponent<ConfigurableJoint>().angularYLimit.limit != 0)
            {

                //limbObjects[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, torque * outputs[i * 3 + 2]));
                //GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, torque * -outputs[i * 3 + 2]));
                int inc = 0;
                if (xChosen)
                {
                    inc++;
                }
                limbObjects[i].GetComponent<Rigidbody>().AddForce(new Vector3(0,torque * outputs[i + adder + inc], 0));
                GetComponent<Rigidbody>().AddForce(new Vector3(0,torque * -outputs[i + adder + inc], 0));
                yChosen = true;
            }


            if (xChosen && zChosen) {
                adder++;
            }
            if (xChosen && yChosen)
            {
                adder++;
            }
            //limbObjects[i].transform.rotation = limbObjects[i].transform.parent.rotation;
            //limbObjects[i].transform.localScale = limbObjects[i].transform.lossyScale;
            //limbObjects[i].GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(torque * -outputs[i * 3], torque * -outputs[i * 3 + 1], torque * -outputs[i * 3 + 2]), pos2);
            //limbObjects[i].GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(torque * outputs[i * 3], torque * outputs[i * 3 + 1], torque * outputs[i * 3 + 2]));
        }

    }
   

    IEnumerator WaitBetweenShots()
    {

        ready = false;

        yield return new WaitForSeconds(time);

        ready = true;

    }
    public float GetTimer() {

        return timeBeforeFall;
    }
    public void ResetTimer()
    {
        for (int i = 0; i < limbs; i++)
        {


            ConfigurableJoint cj = limbObjects[i].GetComponent<ConfigurableJoint>();
           

            cj.targetAngularVelocity = new Vector3(0,0,0);
            //cj.targetAngularVelocity = new Vector3(outputs[adder] * 100, 0, 0);

        }
        //oldVelocities[0] = 0.0f;
        // oldVelocities[1] = 0.0f;
        //oldVelocities[2] = 0.0f;
        // for (int i = 0; i < limbs; i++)
        // {
        //   oldVelocities[3 + i * 3] = 0.0f;
        //  oldVelocities[4 + i * 3] = 0.0f;
        //   oldVelocities[5 + i * 3] = 0.0f;

        // }
       
        first = true;
        StopCoroutine("WaitBetweenShots");
        ready = false;
        //StartCoroutine(WaitBetweenShots());
        if (coroutine) {
            StopCoroutine("RunCo");
        }
        
       
        
        //ready = true;
        for (int i = 0; i < outputs.Length; i++) {
            outputs[i] = 0f;
        }
        fallen = false;
        timeBeforeFall = 0.0f;
        if (harness != null) {
            harness.transform.position = new Vector3(harness.transform.position.x, harness.transform.position.y, 0);
            if (harness.GetComponents<SpringJoint>()[0].spring - gen.reduceSpring > 0)
            {
                harness.GetComponents<SpringJoint>()[0].spring -= gen.reduceSpring;
                harness.GetComponents<SpringJoint>()[1].spring -= gen.reduceSpring;
            }
            else {
                harness.GetComponents<SpringJoint>()[0].spring = 0.0f;
                harness.GetComponents<SpringJoint>()[0].connectedBody = null;

                harness.GetComponents<SpringJoint>()[1].spring = 0.0f;
                harness.GetComponents<SpringJoint>()[1].connectedBody = null;
            }
           

        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor" )
        {

            floorSensor = 1.0f;
            //Debug.Log(timeBeforeFall);
           
            if (gen.disableOnFall) {
                fallen = true;
                GetComponent<Rigidbody>().isKinematic = true;
            }
            
        }


    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            floorSensor = 0.0f;
        }
    }
    public void ResetPos() {
        //Debug.Log(pf.transform.localPosition);
        ResetPos2(transform, pf.transform);
       
    }
    private void ResetPos2(Transform t, Transform tpf)
    {
        //t.GetComponent<Rigidbody>().isKinematic = true;
        t.localPosition = tpf.localPosition;
        t.localRotation = tpf.localRotation;
        t.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        t.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < t.childCount; i++) {
            Transform t2 = t.GetChild(i);
            Transform tpf2 = tpf.GetChild(i);
            ResetPos2(t2,tpf2);
        }
       // t.GetComponent<Rigidbody>().isKinematic = false;

    }
    public void ResetLimb()
    {
        for (int i = 0; i < limbs; i++)
        {
            ConfigurableJoint configurableJointComp = limbObjects[i].GetComponent<ConfigurableJoint>();
            SoftJointLimit softJointLimit = new SoftJointLimit();
            softJointLimit.limit = 45.0f;// *-180;
            //softJointLimit.limit = outputs[i * 3] *-90;
            configurableJointComp.lowAngularXLimit = softJointLimit;
            configurableJointComp.angularZLimit = softJointLimit;
            configurableJointComp.highAngularXLimit = softJointLimit;
        }
    }
    private void FindLimbs(Transform t)
    {
        if (t.tag == "leg") {
            limbObjects.Add(t.gameObject);
            limbs++;
        }
       

        for (int i = 0; i < t.childCount; i++)
        {
            FindLimbs(t.GetChild(i));
        }
      

    }

    public void RunCo(float[] inputs)
    {
        StartCoroutine(RunC(inputs));


    }
    IEnumerator RunC(float[] inputs)
    {
        //if (id == 0) {
           // Debug.Log("run");
           // Debug.Log(g1);
       // }
        outputs = g1.Run(inputs);
        if (id == 0) {
            //Debug.Log(inputs[0]);
            //Debug.Log(outputs[0]);
            //Debug.Log(outputs[1]);

        }
        yield return null;

    }

}