using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad  {
    private List<GameObject> agents;
    //private List<Genome> genomes;
    //private List<Species> species;
    private Neat neat;
    private INeatInterface impf;
    public SaveLoad() {
        //Generate gen = GameObject.Find("Scripts").GetComponent<Generate>();
        //agents = gen.GetAgents();
       // genomes = new List<Genome>();
        //species = new List<Species>();
        neat = GameObject.Find("Scripts").GetComponent<Neat>();
        impf = neat.GetImplementation();
        agents = impf.GetAgents();

    }

    public void Save() {
        //genomes.Clear();
        //species.Clear();
        //for (int i = 0; i < agents.Count; i++)
        // {

        //  genomes.Add(agents[i].GetComponent<Brain>().GetGenome());
        // }
        if (neat.speciation)
        {
            List<Species> species = neat.GetSpecies();
            SpeciesWrapper sw = new SpeciesWrapper();
            sw.species = species;
            sw.generation = neat.GetGeneration();
            sw.gt = neat.GetTracker();
            Debug.Log(sw.species.Count);
            string json = JsonUtility.ToJson(sw);
            Debug.Log(json);
            //PlayerPrefs.DeleteKey("Genomes");
            PlayerPrefs.SetString("Genomes", json);
            Debug.Log(json);
            Debug.Log(neat.GetGenomes()[0]);
            
        }
        else {
            List<Genome> genomes = new List<Genome>();
            List<Genome> genomes2 = neat.GetGenomes();
            for (int i = 0; i < genomes2.Count; i++) {
                genomes.Add(genomes2[i].Copy());

            }
            Debug.Log(genomes.Count);
            GenomesWrapper gw = new GenomesWrapper();
            gw.genomes = genomes;
            gw.generation = neat.GetGeneration();
            gw.gt = neat.GetTracker();
            string json = JsonUtility.ToJson(gw);
            //PlayerPrefs.DeleteKey("Genomes");
            PlayerPrefs.SetString("Genomes", json);
            Debug.Log(json);
        }
        impf.OnSave();

    }

    public void Load()
    {
        if (neat.speciation)
        {
            string info = PlayerPrefs.GetString("Genomes");
            //species.Clear();
            SpeciesWrapper sw = JsonUtility.FromJson<SpeciesWrapper>(info);
            Debug.Log(sw.species.Count);
            neat.SetSpecies(sw.species);
            neat.SetGeneration(sw.generation);
            neat.SetTracker(sw.gt);
            List<Genome> genomes = new List<Genome>();
            int k = 0;
            for (int i = 0; i < sw.species.Count; i++)
            {
                for (int j = 0; j < sw.species[i].GetGenomes().Count; j++)
                {
                    //Debug.Log(species[i].GetGenomes()[j].GetSpeciesId());
                    // newGenomes.Add(species[i].GetGenomes()[j]);
                    impf.SetGenome(agents[k], sw.species[i].GetGenomes()[j]);
                    impf.ResetAgent(agents[k], k);
                   
                    neat.GetGenomes()[k] = sw.species[i].GetGenomes()[j];
                    impf.SetSpecies(agents[k]);
                    sw.species[i].GetGenomes()[j].SetNeat();

                    for (int l = 0; l < sw.species[i].GetGenomes()[j].GetConnectionGenes().Count; l++)//sample n connections
                    {

                        int inId = sw.species[i].GetGenomes()[j].GetConnectionGenes()[l].GetInNode().GetId();
                        int outId = sw.species[i].GetGenomes()[j].GetConnectionGenes()[l].GetOutNode().GetId();
                        for (int l2 = 0; l2 < sw.species[i].GetGenomes()[j].GetNodeGenes().Count; l2++) {
                            if (inId == sw.species[i].GetGenomes()[j].GetNodeGenes()[l2].GetId()) {
                                sw.species[i].GetGenomes()[j].GetConnectionGenes()[l].SetInNode(sw.species[i].GetGenomes()[j].GetNodeGenes()[l2]);
                            }
                            if (outId == sw.species[i].GetGenomes()[j].GetNodeGenes()[l2].GetId())
                            {
                                sw.species[i].GetGenomes()[j].GetConnectionGenes()[l].SetOutNode(sw.species[i].GetGenomes()[j].GetNodeGenes()[l2]);
                            }
                        }

                    }

                    k++;
                }
            }
            Debug.Log(neat.GetGenomes().Count);
            Debug.Log(neat.GetGenomes()[0]);
            impf.StatsSpecies(sw.species.Count);

            
        }
        else
        {


            string info = PlayerPrefs.GetString("Genomes");
            Debug.Log(info);
            //genomes.Clear();
            GenomesWrapper gw = JsonUtility.FromJson<GenomesWrapper>(info);
            Debug.Log(gw.genomes.Count);
            neat.SetGenomes(gw.genomes);
            neat.SetGeneration(gw.generation);
            neat.SetTracker(gw.gt);
            for (int i = 0; i < agents.Count; i++)
            {

                //agents[i].GetComponent<Brain>().SetGenome(gw.genomes[i]);
                gw.genomes[i].SetNeat();
                impf.SetGenome(agents[i], gw.genomes[i].Copy());
                impf.ResetAgent(agents[i],i);
            }
           
            
        }
       // impf.OnLoad();
        neat.ResetTimer();
        GameObject.Find("Main Camera").GetComponent<CameraScript>().ResetPos();
       
    }

}
