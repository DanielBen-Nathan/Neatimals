using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neat : MonoBehaviour {//set up initial genes and perform operations on population
    private List<GameObject> agents;
    private List<Genome> genomes;
    private float time = 0.0f;
    private float timer2 = 0.0f;
    private GeneTracker gt;
    private Genome genome;
    private List<ConnectionGene> connections;
    private List<NodeGene> nodes;
    private INeatInterface impf;
    private int generation = 1;
    private List<Species> species;

    public int maxTime = 30;
    public float checkEarlyTermination = 3.0f;
    public int population = 500;//750 or 500
    public int tournamentSize = 10;//5
    public int survivors = 25;//100 or 50
    public float crossoverProbability = 1.0f;
    public int inputSize = 46;//82 or 46
    public int outputSize = 12;//24 or 12
    public int startConnections = 1;
    public float lr = 0.1f;
    public float minLr = 0.01f;
    public float decayRate = 0.97f;
    public float weightPerpMutationProb = 0.9f;//0.9
    public float weightMutationProb = 0.8f;//0.8
    public float connectionMutationProb = 0.3f;//0.5
    public float nodeMutationProb = 0.2f;//0.5
    public float enableConnectionProb = 0.25f;//0.5
    public float disableConnectionProb = 0.25f;//0.5
    public float weightRange = 1.0f;
    public NodeGene.ACTIVATION_FUNCTION internalActiationFunction = NodeGene.ACTIVATION_FUNCTION.SIGMOID;
    public NodeGene.ACTIVATION_FUNCTION outputActiationFunction = NodeGene.ACTIVATION_FUNCTION.SIGMOID;
    public int maxNeurones = 50;
    public int maxConnections = 250;
    public bool cloneBestToAll = false;

    public bool speciation;
    public float coeffDisjoint = 0.05f;
    public float coeffWeightDiff = 1.0f;
    public float threshold = 0.1f;
    public float outerSpeciesCrossoverProb = 0.0f;

   

    void OnEnable() {//set up inital nodes and connections
        gt = new GeneTracker();
        genome = new Genome(gt);
        nodes = new List<NodeGene>();
        species = new List<Species>();
        for (int i = 0; i < inputSize; i++)
        {
            NodeGene nodeIn = new NodeGene(NodeGene.NODETYPE.INPUT, gt.NewId(), 0, false, NodeGene.ACTIVATION_FUNCTION.NULL);
            nodes.Add(nodeIn);
            genome.AddNodeGene(nodeIn.Copy());

        }
        for (int i = 0; i < outputSize; i++)
        {
            NodeGene nodeOut = new NodeGene(NodeGene.NODETYPE.OUTPUT, gt.NewId(), 1, true, outputActiationFunction);
            nodes.Add(nodeOut);
            genome.AddNodeGene(nodeOut.Copy());

        }


         connections = new List<ConnectionGene>();
      

        for (int i = 0; i < inputSize;i++) {

            for (int i2 = 0; i2 < outputSize; i2++)
            {
                //Debug.Log(genome.GetNodeGenes()[i]);
                ConnectionGene con = new ConnectionGene(genome.GetNodeGenes()[i], genome.GetNodeGenes()[inputSize + i2], gt.Innovate());
                connections.Add(con.Copy());
            }
        }
        for (int i = 0; i < connections.Count; i++) {
            //Debug.Log(connections[i]);
        }
        

    }


    // Use this for initialization
    void Start () {
        //Debug.Log(gt.NewId());
        // gen = gameObject.GetComponent<Generate>();
        //agents = gen.GetAgents();
        impf = GetComponent<INeatInterface>();
        agents = impf.GetAgents();
        genomes = new List<Genome>();
        
        //for (int i = 0; i < agents.Count; i++)//get genomes and set fitness
        //{
          //  agents[i].GetComponent<Brain>().SetId(i);
        //}

        }
	
	// Update is called once per frame
	void Update () {
        if (time >= maxTime && GetComponent<ImplementFunctions>().Condition()) {
            //Debug.Log(agents[0].GetComponent<Brain>().GetGenome());
            //genomes.Clear();
            //genomes = new List<Genome>();
            //species.Clear();
            GetComponent<ImplementFunctions>().OnTimeReached();
            float sumFitness = 0;
            for (int i = 0; i < agents.Count; i++)//get genomes and set fitness
            {
                //agents[i].GetComponent<Brain>().SetFitness(agents[i].transform.position.z*-1);
                // genomes.Add( agents[i].GetComponent<Brain>().GetGenome());
               // genomes.Add(impf.GetGenome(agents[i]));
               // if (speciation) {
                    //InsertGenomeIntoSpecies(impf.GetGenome(agents[i]));

               // }
                //float fitness = agents[i].transform.position.z * -1;
                //if (checkFall) {
                //fitness += agents[i].GetComponent<Brain>().GetTimer()*gamma;
                //agents[i].GetComponent<Brain>().ResetTimer();
                //}
                float fitness = GetComponent<ImplementFunctions>().FitnessFunction(agents[i]);
                genomes[i].SetFitness(fitness);
               
                sumFitness += fitness;
            }
            float avgFitness = sumFitness / agents.Count;
            
            genomes.Sort((x, y) => x.GetFitness().CompareTo(y.GetFitness()));
            genomes.Reverse();

           
            Debug.Log("generation: " + generation + "   average fitness: " + avgFitness + "    best fitness: " + genomes[0].GetFitness() + "\n" + genomes[0]);
            Debug.Log("second best fitness: " + genomes[1].GetFitness() + "\n" + genomes[1]);
            impf.Stats(generation+1,genomes[0].GetFitness(), avgFitness);
            impf.Write(generation, genomes[0].GetFitness(), avgFitness,genomes[0].GetConnectionGenes().Count, genomes[0].GetNodeGenes().Count,species.Count);
            if (speciation)//calculate shared fitness for each genome in each species
            {
                for (int i = 0; i < species.Count; i++)
                {
                    for (int j = 0; j < species[i].GetGenomes().Count; j++)
                    {
                        Genome genome = species[i].GetGenomes()[j];
                        genome.SetFitness(species[i].CalcSharedFitness(genome));
                    }
                }
                genomes.Sort((x, y) => x.GetFitness().CompareTo(y.GetFitness()));
                genomes.Reverse();
            }
           
            // Debug.Log("generation: " + generation + "   average fitness: " + avgFitness + "    best fitness: " + genomes[0].GetFitness() + "\n" + genomes[0]);

            //Debug.Log("SPEC generation: " + generation + "   average fitness: " + avgFitness + "    best fitness: " + genomes[0].GetFitness() + "\n" + genomes[0]);

            //for (int i = 0; i < agents.Count; i++)
            //{
            //Debug.Log(genomes[i].GetFitness());
            // }

            List<Genome> selected = TournamentSelection();
           
            List<Genome> newGenomes;
            if (speciation)
            {
                newGenomes = new List<Genome>();
                CrossoverMutationSpecies(selected);
                for (int i = 0; i < survivors; i++)
                {
                    // Genome g = new Genome(gt);
                    //  g.
                    InsertGenomeIntoSpecies(genomes[i], species);
                    //Debug.Log(genomes[i].GetSpeciesId());
                    //Debug.Log(genomes[i]);
                }
                for (int i = 0; i < species.Count; i++)
                {
                    for (int j = 0; j < species[i].GetGenomes().Count; j++)
                    {
                        //Debug.Log(species[i].GetGenomes()[j].GetSpeciesId());
                        newGenomes.Add(species[i].GetGenomes()[j]);
                    }
                }
                //Debug.Log("new " + newGenomes[0]);
            }
            else {
                newGenomes = CrossoverMutation(selected);
                for (int i = 0; i < survivors; i++)
                {
                    // Genome g = new Genome(gt);
                    //  g.
                    newGenomes.Add(genomes[i]);

                }
            }

            
           
            //Debug.Log(gt.NewId());
            if (cloneBestToAll) {
               // newGenomes.Clear();
               
                for (int i = 0; i < agents.Count ; i++)//crossover and mutation
                {
                    //Debug.Log(i);
                    newGenomes[i] = genomes[0].Copy();
                    speciation = false;
                }
            }

            if (speciation)
            {
                Debug.Log("number of species: " + species.Count + "  first in species list size: " + species[0].GetGenomes().Count);
                Debug.Log(newGenomes.Count);
                impf.StatsSpecies(species.Count);
            }

            Debug.Log("new "+newGenomes[0]);
            for (int i = 0; i < agents.Count; i++) { 
               //Debug.Log(newGenomes[i]);
            }
            impf.ResetEnviorment();
            for (int i = 0; i < agents.Count; i++) {//reset position
                //agents[i].GetComponent<Brain>().SetGenome(newGenomes[i]);
                impf.ResetAgent(agents[i], i);
                impf.SetGenome(agents[i], newGenomes[i]);
                if (speciation) {
                    impf.SetSpecies(agents[i]);
                }
              
               

            }
            genomes = newGenomes;
            time = 0.0f;
            generation++;
            lr *= decayRate;
            if (lr < minLr) {
                lr = minLr;
            }
            
        }

        if (timer2 >= checkEarlyTermination) {
            //Debug.Log("check ");
            if (impf.EarlyTermination(agents,time)) {
                time = maxTime;
               
               // Debug.Log("early termination");
            }
           // Debug.Log(impf.EarlyTermination(agents));
            timer2 = 0.0f;
        }

        //Debug.Log(time);
        time += Time.deltaTime;
        timer2 += Time.deltaTime;

    }

    public List<Genome> GetGenomes() {
        return genomes;

    }
    public void SetGenomes(List<Genome> genomes)
    {
        this.genomes = genomes;

    }
    public List<Species> GetSpecies()
    {
        return species;

    }
    public void SetSpecies(List<Species> species)
    {
        this.species = species;

    }
    public int GetGeneration()
    {
        return generation;

    }
    public void SetGeneration(int generation)
    {
        this.generation = generation-1;
        impf.SetGeneration(generation);

    }
    public INeatInterface GetImplementation() {

        return impf;
    }
    public void ResetTimer() {
        time = 0.0f;
        generation++;
    }
    public float GetTimer()
    {
        return time;
    }
    public void CrossoverMutationSpecies(List<Genome> selected) {
      
       
       
        for (int i = 0; i < selected.Count; i++)//group selected into species
         {

            int id = selected[i].GetSpeciesId();
            //Genome genome = selected[Random.Range(0, selected.Count)];
            //Debug.Log(selected[i].GetSpeciesId());
            //Debug.Log("225");
            for (int j = 0; j < species.Count; j++) {
                if (id == species[j].GetId()) {
                    //Debug.Log("id: "+id+"   species: " + species[j].GetId()+"size: "+species[j].GetGenomes().Count);
                    species[j].GetSelectedGenomes().Add(selected[i]);
                   // Debug.Log("225");
                    break;
                   
                }

            }

         }

       
        Crossover crossover = new Crossover();
        int sum2 = 0;
        for (int i = 0; i < species.Count; i++) {
            if (!species[i].CheckExtinct()) {
                for (int j = 0; j < species[i].GetSelectedGenomes().Count; j++)
                {
                    Genome parent1 = species[i].GetSelectedGenomes()[Random.Range(0, species[i].GetSelectedGenomes().Count)];
                    Genome child = null;
                    if (Random.Range(0.0f, 1.0f) <= crossoverProbability)
                    {
                        Genome parent2 = species[i].GetSelectedGenomes()[Random.Range(0, species[i].GetSelectedGenomes().Count)];
                        child = crossover.CrossoverGenes(parent1, parent2, gt);
                    }
                    else
                    {
                        child = parent1.Copy();
                    }

                    //Debug.Log(child);
                    child.Mutate();
                    child.NextGen();
                    InsertGenomeIntoSpeciesChild(child, species);
                    //newGenomes.Add(child);
                    sum2++;
                }

            }
            
        }
       // Debug.Log("new" + sum2);
       // Debug.Log("count s" + species.Count);
       // Debug.Log("count s" + species[species.Count-1].GetChildren().Count);
        for (int i = 0; i < species.Count; i++) {
            //species[i].RemoveGen(generation-1);
           
            
             species[i].SetChildrenToSpecies();
            species[i].GetSelectedGenomes().Clear();


        }
        //Debug.Log("count s2" + species.Count);
       // Debug.Log("count s" + species[species.Count-1].GetGenomes().Count);
        for (int i = 0; i < species.Count; i++)
        {
           // Debug.Log("ss count" + species[i].GetGenomes().Count);
            //species[i].RemoveGen(generation-1);
            if (species[i].CheckExtinct())
            {
                species.RemoveAt(i);
                i--;

            }
          

        }
        // Debug.Log(newGenomes.Count);
        //species = newSpecies;
        //Debug.Log("s count"+species.Count);
       // Debug.Log("s1 count" + species[0].GetGenomes().Count);
     
       // Debug.Log(newGenomes.Count);
        //return newGenomes;
    }

    public List<Genome> CrossoverMutation(List<Genome> selected) {
        Crossover crossover = new Crossover();
        List<Genome> newGenomes = new List<Genome>();
        for (int i = 0; i < agents.Count - survivors; i++)//crossover and mutation
        {
            Genome parent1 = selected[Random.Range(0, selected.Count )];

            //Genome child = parent1.Copy();//crossover.CrossoverGenes(parent1, parent2, gt);
            Genome child = null;
            if (Random.Range(0.0f, 1.0f) <= crossoverProbability)
            {
                Genome parent2 = selected[Random.Range(0, selected.Count )];
                child = crossover.CrossoverGenes(parent1, parent2, gt);
            }
            else
            {
                child = parent1.Copy();
            }

            //Debug.Log(child);
            child.Mutate();
            newGenomes.Add(child);
            //Debug.Log(newGenomes[i]);
            //Debug.Log(child);
            
        }

        return newGenomes;

    }
    public List<Genome> TournamentSelection() {
        List<Genome> selected = new List<Genome>();

        List<Genome> tournament = new List<Genome>();
        for (int i = 0; i < agents.Count - survivors; i++)
        {
            tournament.Clear();

            for (int i2 = 0; i2 < tournamentSize; i2++)
            {
                tournament.Add(genomes[Random.Range(0, agents.Count)]);
            }
            tournament.Sort((x, y) => x.GetFitness().CompareTo(y.GetFitness()));
            tournament.Reverse();
           
            selected.Add(tournament[0]);
            //Debug.Log(tournament.Count);
        }
        return selected;
    }
    public GeneTracker GetTracker() {

        return gt;
    }
    public void SetTracker(GeneTracker gt)
    {

        this.gt = gt;
    }
    public Genome GetDeafultGenome()
    {
        Genome g = new Genome(gt);
        for (int i = 0; i < genome.GetNodeGenes().Count; i++) {
            g.AddNodeGene(genome.GetNodeGenes()[i]);

        }
        return g;
    }
   
    public int GetInputSize()
    {

        return inputSize;
    }
    public int GetOutputSize()
    {

        return outputSize;
    }
    public Genome InitGenome() {
        Genome g1 = new Genome(gt);
       


      
        for (int i = 0; i < inputSize + outputSize; i++)
        {
            NodeGene node = GetDeafultGenome().GetNodeGenes()[i].Copy();
            if (node.GetNodeType() == NodeGene.NODETYPE.OUTPUT || node.GetNodeType() == NodeGene.NODETYPE.HIDDEN)
            {
                node.SetBias(Random.Range(-1.0f, 1.0f));
                //node.SetBias(1.0f);
            }
            g1.AddNodeGene(node);
            //g1.GetNodeGenes()[inputSize + i].SetBias(Random.Range(-1.0f, 1.0f));



        }
        for (int i = 0; i < startConnections; i++)//sample n connections
        {

            ConnectionGene con = connections[Random.Range(0, connections.Count)].Copy();
            //ConnectionGene con = connections[i].Copy();
            con.SetWeight(Random.Range(-weightRange , weightRange));
            //con.SetWeight(1.0f);
            int inId = con.GetInNode().GetId();
            con.SetInNode(g1.GetNodeGenes()[inId]);
            int outId = con.GetOutNode().GetId();
            //Debug.Log(con.GetOutNode());
            //Debug.Log(outId);
            con.SetOutNode(g1.GetNodeGenes()[outId]);
            g1.AddConnectionGene(con);

        }
        if (speciation) {
            InsertGenomeIntoSpecies(g1,species);
            impf.StatsSpecies(species.Count);
        }
        genomes.Add(g1);
       
        return g1;

    }

    public void InsertGenomeIntoSpecies(Genome g, List<Species> inserIntoSpecies) {
       // bool speciesFound = false;

        for (int i = 0; i < inserIntoSpecies.Count; i++) {

            if (!inserIntoSpecies[i].CheckExtinct())
            {
                List<Genome> genomesInSpecies = inserIntoSpecies[i].GetGenomes();
                Genome g2 = genomesInSpecies[Random.Range(0, genomesInSpecies.Count)];
                //Genome g2 = genomesInSpecies[0];
                if (g.CheckMatchingSpecies(g2))
                {
                    //speciesFound = true;
                    //genomesInSpecies.Add(g);
                    inserIntoSpecies[i].AddGenome(g);

                    return;

                }
            }

        }
        Species newSpecies = new Species(g,gt.NewSpeciesId(),false);
        inserIntoSpecies.Add(newSpecies);

    }
    public void InsertGenomeIntoSpeciesChild(Genome g, List<Species> inserIntoSpecies)
    {
        // bool speciesFound = false;

        for (int i = 0; i < inserIntoSpecies.Count; i++)
        {
            if (!inserIntoSpecies[i].CheckExtinct()) {
                List<Genome> genomesInSpecies = inserIntoSpecies[i].GetGenomes();
                Genome g2 = genomesInSpecies[Random.Range(0, genomesInSpecies.Count)];
                //Genome g2 = genomesInSpecies[0];
                if (g.CheckMatchingSpecies(g2))
                {
                    //speciesFound = true;
                    //inserIntoSpecies[i].GetChildren().Add(g);
                    inserIntoSpecies[i].AddChildGenome(g);
                    return;

                }
            }
            

        }
        Species newSpecies = new Species(g, gt.NewSpeciesId(),true);
        inserIntoSpecies.Add(newSpecies);

    }


    public void InsertGenomeIntoSpecies(Genome g, List<Species> inserIntoSpecies, List<Species> checkSpecies)
    {
        // bool speciesFound = false;

        for (int i = 0; i < checkSpecies.Count; i++)
        {
            List<Genome> genomesInSpecies = checkSpecies[i].GetGenomes();
            if (!checkSpecies[i].CheckExtinct()) {
                Genome g2 = genomesInSpecies[0];
                if (g.CheckMatchingSpecies(g2))
                {
                    //speciesFound = true;
                    //genomesInSpecies.Add(g);
                    inserIntoSpecies[i].GetGenomes().Add(g);
                    return;

                }

            }
            

        }
        Debug.Log("fail"+g);

    }

   




}
