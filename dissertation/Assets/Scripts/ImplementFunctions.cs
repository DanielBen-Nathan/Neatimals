using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class ImplementFunctions : MonoBehaviour , INeatInterface{
    private Generate gen;
    private bool reset = false;
    private Dictionary<int, float[]> colours;
    private string name;

    public bool checkFall = true;
    public float gamma = 0.1f;
    public bool rewardNotFallen =true;
    public float deductRewardAmount = 3.0f;
    public float terminationVelocity = 1.0f;
    public bool viewDead = true;
    // Use this for initialization

    void OnEnable() {
        colours = new Dictionary<int, float[]>();

    }
    void Start () {
        gen = GetComponent<Generate>();
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetGenome(GameObject agent, Genome g) {
        agent.GetComponent<Brain>().SetGenome(g);

    }

    public Genome GetGenome(GameObject agent)
    {
        return agent.GetComponent<Brain>().GetGenome();

    }

    public List<GameObject> GetAgents()
    {
        return GetComponent<Generate>().GetAgents();

    }
    public float FitnessFunction(GameObject agent) {
        float fitness = agent.transform.position.z * -1;
        if (checkFall)
        {
            fitness += agent.GetComponent<Brain>().GetTimer() * gamma;
            
        }
        if (rewardNotFallen) {
            fitness += agent.GetComponent<Brain>().GetReward();


        }
        //agent.GetComponent<Brain>().ResetTimer();
        return fitness;

    }
    public void ResetAgent(GameObject agent, int i) {

        //GameObject.Find("Main Camera").GetComponent<CameraScript>().ResetPos();
        agent.GetComponent<Brain>().ResetReward();
        agent.GetComponent<Rigidbody>().isKinematic = true;
        agent.GetComponent<Brain>().ResetPos();
        agent.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        agent.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
        agent.transform.position = new Vector3(-i * gen.GetGap(), gen.GetPF().transform.position.y, 0f);
        agent.transform.rotation = gen.GetPF().transform.rotation;//new Vector3(90, 0f, gen.agentOrientation);
        //agents[i].GetComponent<Brain>().ResetLimb();
        agent.GetComponent<Rigidbody>().isKinematic = false;
        agent.GetComponent<Brain>().ResetTimer();
        //GameObject.Find("Main Camera").GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);


    }
    public void ResetEnviorment() {
        gen.ResetObstacles();


    }

    public void OnTimeReached() {
       
        //GameObject.Find("Main Camera").GetComponent<CameraScript>().ResetPos();
        
      
       
    }
    public bool Condition() {
        if (viewDead)
        {
            if (!reset)
            {
                GameObject.Find("Main Camera").GetComponent<CameraScript>().ResetPos();
                reset = true;
                List<GameObject> agents = GetComponent<Generate>().GetAgents();
                for (int i = 0; i < agents.Count; i++)
                {
                    //agents[i].GetComponent<Brain>().SetFallen(true);
                }

            }
            reset = GameObject.Find("Main Camera").GetComponent<CameraScript>().GetMoveBack();
            return !reset;
        }
        else {
            GameObject.Find("Main Camera").GetComponent<CameraScript>().SnapReset();
        }
        return true;
        

    }
    public bool EarlyTermination(List<GameObject> agents,float time) {
        if (time < 2.0f) {
            return false;
        }
        for (int i = 0; i < agents.Count; i++) {
            if (!agents[i].GetComponent<Brain>().GetFallen() && agents[i].GetComponent<Rigidbody>().velocity.magnitude> terminationVelocity) {
                return false;
            }

        }
        return true;

    }

    public void SetSpecies( GameObject agent) {
     
        int id = agent.GetComponent<Brain>().GetGenome().GetSpeciesId();
        float[] rgb = new float[3] { 0,0,0};
        if (!colours.TryGetValue(id, out rgb)) {
            colours.Add(id, new float[3] { Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(0f, 1.0f) });
        }

        colours.TryGetValue(id, out rgb);
        //agent.GetComponent<Material>().SetColor();
        Color myColor = new Color(rgb[0], rgb[1], rgb[2], 1);
        //ColorUtility.TryParseHtmlString("#F00", out myColor);
        //myColor.r = id;
        //agent.GetComponent<Renderer>().material.color = myColor;

        MeshRenderer gameObjectRenderer = agent.GetComponent<MeshRenderer>();

        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
        newMaterial.SetFloat("_SpecularHighlights", 0f);
        //newMaterial.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
        //newMaterial.SetFloat("_GLOSSYREFLECTIONS",0f);
        newMaterial.color = myColor;
        gameObjectRenderer.material = newMaterial;
    }

    public void Stats(int gen, float best, float avg) {
        GetComponent<HUD>().SetLastGen(gen, best,avg);

    }

    public void StatsSpecies(int numSpecies)
    {
        GetComponent<HUD>().SetSpeciesNo(numSpecies);

    }
    public void SetGeneration(int generation) {
        GetComponent<HUD>().SetGeneration(generation);
    }
    public void OnSave() {}

    public void OnLoad() { }

    public void Write(int generation, float best, float avg, int cons, int nodes, int species) {
        Neat neat = GetComponent<Neat>();
        StringBuilder csv = new StringBuilder();
       
        if (neat.GetGeneration() == 1) {
            csv.AppendLine("generation, best fitness, average fitness, number of connections, number of nodes, number of species");
            name = System.DateTime.Now.ToString("-yyyy-MM-dd-HH-mm-ss");
        }
        List<GameObject> agents = gen.GetAgents();
       
        csv.AppendLine(generation+ "," +best+","+avg + "," + cons + "," + nodes + "," + species);
          
        
        string path = Path.Combine(Application.persistentDataPath, "data\\data"+ name+ ".csv");
        //File.Delete(path);
        File.AppendAllText(path, csv.ToString());
    }
    
}
