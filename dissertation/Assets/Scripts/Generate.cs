using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate : MonoBehaviour {
    private List<GameObject> agents;

    public GameObject wall;
    public GameObject floor;
    public int gap = 10;//10
    public float width = 8.0f;
    public float length = 200;
    public GameObject agentPF;
    public bool disableOnFall = false;
    public bool disableOnLimbFall= false;
    public float agentOrientation = 90.0f;
    public float torque = 5.0f;
    public float agentTimeInterval = 0.1f;
    public bool coroutine = false;

    public bool obstacles;
    public float minObstacleDistance;
    public float minWallWidth=3.0f;
    public float maxWallWidth = 3.5f;
    public float minWallSep = 10.0f;
    public float maxWallSep = 20.0f;
    public bool resetObstacles;
    public bool showSensorRays;
    //public float minObstacleDistance;


    public bool harnessOn;
    public GameObject harness;
    public float strength = 0.25f;
    public float reduceSpring = 0.01f;
    // Use this for initialization
    void OnEnable () {
        int population = GetComponent<Neat>().population;
        agents = new List<GameObject>();
        //agents.Add(GameObject.Find(name));
        
        for (int i = 0; i < population; i++)
        {
            //Quaternion q = new Quaternion();
            //q.eulerAngles = new Vector3(90.0f, 0, agentOrientation);
            GameObject floorObj = Instantiate(floor, new Vector3(-i*gap, -1, -(length/2)+4), floor.transform.rotation);
            floorObj.transform.localScale = new Vector3(width, floorObj.transform.localScale.y,length);
            GameObject agent = Instantiate(agentPF, new Vector3( - i * gap, agentPF.transform.position.y, 0), agentPF.transform.rotation);
            agent.name = agentPF.name + i;
            agent.GetComponent<Brain>().SetId(i);
            agent.GetComponentInChildren<Brain>().torque=torque;
            agent.GetComponentInChildren<Brain>().time = agentTimeInterval;
            agent.GetComponentInChildren<Brain>().disableOnLimbFall = disableOnLimbFall;
            agent.GetComponentInChildren<Brain>().coroutine = coroutine;
            agent.GetComponentInChildren<Brain>().Setsensors(obstacles);
           
            if (harnessOn) {
                GameObject harnessObj = Instantiate(harness, new Vector3(-i * gap, agent.transform.Find("hips").transform.position.y, 0), agentPF.transform.rotation);
                harnessObj.GetComponents<SpringJoint>()[0].connectedBody = agent.transform.Find("hips").gameObject.GetComponent<Rigidbody>();
                harnessObj.GetComponents<SpringJoint>()[1].connectedBody = agent.transform.Find("hips").gameObject.GetComponent<Rigidbody>();
                //harnessObj.GetComponents<SpringJoint>()[0].anchor.x
                //harnessObj.GetComponent<SpringJoint>().connectedBody = agent.GetComponent<Rigidbody>();
                harnessObj.GetComponents<SpringJoint>()[0].spring = strength;
                harnessObj.GetComponents<SpringJoint>()[1].spring = strength;
                agent.GetComponent<Brain>().harness = harnessObj;
            }
            agents.Add(agent);
        }
        if (obstacles) {
            GenerateObstacles();
        }
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public List<GameObject> GetAgents() {
        return agents;

    }
    public int GetGap()
    {
        return gap;

    }

    public GameObject GetPF()
    {
        return agentPF;

    }
    public void GenerateObstacles() {
        
         float distance = minObstacleDistance;
        List<GameObject> wallObjs = new List<GameObject>();
        distance += Random.Range(1.0f, 5.0f);
        while (distance < length) {

            
            float wallWidth = Random.Range(minWallWidth, maxWallWidth);
            GameObject wallObj = Instantiate(wall, new Vector3(-0 * gap+ Random.Range((-width / 2)+(wallWidth/2), (width / 2) - (wallWidth/2)), 0, -distance), wall.transform.rotation);
            wallObj.transform.localScale=new Vector3(wallWidth, wallObj.transform.localScale.y, wallObj.transform.localScale.z);
            wallObj.name = "wall";
            wallObjs.Add(wallObj);
            distance += Random.Range(minWallSep, maxWallSep);
        }

        for (int i = 1; i < agents.Count; i++) {
            for (int j = 0; j < wallObjs.Count; j++)
            {
                GameObject wallObj = Instantiate(wallObjs[j], new Vector3(-i * gap + wallObjs[j].transform.position.x, 0, wallObjs[j].transform.position.z), wall.transform.rotation);
                wallObj.name = "wall";
            }

        }

    }
    public void ResetObstacles() {
        if (obstacles && resetObstacles) {
            GameObject[] objs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].name == "wall")
                {
                    Destroy(objs[i]);
                }
            }
            GenerateObstacles();

        }
       


    }



}
