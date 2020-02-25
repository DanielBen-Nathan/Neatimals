using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Genome 
{
    [SerializeField]
    private List<ConnectionGene> connections;
    [SerializeField]
    private List<NodeGene> nodes;
    [SerializeField]
    private int layers=1;//hidden layers
    private int atempts = 500;
    private float fitness;
    [SerializeField]
    private GeneTracker track;
    [SerializeField]
    private Neat neat;
    [SerializeField]
    private int generation = 1;
    [SerializeField]
    private int speciesId = 0;
  


    public Genome(GeneTracker track)
    {
      
        connections = new List<ConnectionGene>();
        nodes = new List<NodeGene>();
        this.track = track;
        layers = 1;
        neat = GameObject.Find("Scripts").GetComponent<Neat>();

    }

    

    public Genome(List<ConnectionGene> connections, List<NodeGene> nodes,GeneTracker track,int layers) {
       
        this.connections = connections;
        this.nodes = nodes;
        this.track = track;
        this.layers = layers;

    }

    public List<ConnectionGene> GetConnectionGenes() {

        return connections;
    }

    public List<NodeGene> GetNodeGenes()
    {

        return nodes;
    }
    public float GetFitness() {
        return fitness;

    }
    public void SetFitness(float fitness) {
        this.fitness = fitness;

    }
    public void SetLayers(int layers)
    {
        this.layers = layers;

    }

    public int GetGeneration()
    {
        return generation;

    }
    public void NextGen()
    {
        generation++;

    }
    public void SetUnusedGen()
    {
        generation = -1;

    }
    public void SetSpecies(int speciesId) {
        this.speciesId = speciesId;

    }

    public int GetSpeciesId() {
        return speciesId;

    }

    public void AddNodeGene(NodeGene node) {
        nodes.Add(node);

    }
    public void AddConnectionGene(ConnectionGene con)
    {
        connections.Add(con);

    }

    public void Mutate() {

       

        if (Random.Range(0.0f, 1.0f) <= neat.weightMutationProb)//weightMutationProb)
        {

            WeightsMutate();

        }
       
       
        if (Random.Range(0.0f, 1.0f) <= neat.nodeMutationProb)//connectionMutationProb)
        {
            AddNodeMutation();
        }
      
        if (Random.Range(0.0f, 1.0f) <= neat.connectionMutationProb)//connectionMutationProb)
        {
            AddConnectionMutation();
        }


        if (Random.Range(0.0f, 1.0f) <= neat.enableConnectionProb)//connectionMutationProb)
        {
            EnableConnectionMutation();
        }

        if (Random.Range(0.0f, 1.0f) <= neat.disableConnectionProb)//connectionMutationProb)
        {
            DisableConnectionMutation();
        }




    }

    public void WeightsMutate() {
        

        for (int i = 0; i < connections.Count; i++)
        {
          
            if (Random.Range(0.0f, 1.0f) <= neat.weightPerpMutationProb)//weightPerpMutationProb)
            {
                connections[i].SetWeight(connections[i].GetWeight() + neat.lr * Random.Range(-1.0f, 1.0f));
            }
            else {
                connections[i].SetWeight(Random.Range(-1.0f, 1.0f));
            }
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetNodeType() != NodeGene.NODETYPE.INPUT)
            {
              
                if (Random.Range(0.0f, 1.0f) <= neat.weightPerpMutationProb)//weightPerpMutationProb)
                {
                    nodes[i].SetBias(nodes[i].GetBias() + neat.lr * Random.Range(-1.0f, 1.0f));
                }
                else
                {
                    nodes[i].SetBias(Random.Range(-1.0f, 1.0f));
                }
            }
               
        }

        
       


    }
    


    public void AddConnectionMutation() {
        if (neat.maxConnections <= connections.Count) {
            return;
        }
        
        for (int tries = 0; tries < atempts; tries++)//try to find places in the network to add connections
        {
            NodeGene node1 = nodes[Random.Range(0, nodes.Count)];//pick 2 random nodes
            NodeGene node2 = nodes[Random.Range(0, nodes.Count)];

            bool reverse = false;
          
            if (node1.GetLayer() > node2.GetLayer())
            {
                reverse = true;
            }
           

            if (reverse)
            {//swap nodes
                NodeGene temp = node1;
                node1 = node2;
                node2 = temp;
            }
            bool connectionExists = false;
            if (node1.GetLayer() == node2.GetLayer())
            {
                connectionExists = true;
            }
            for (int i = 0; i < connections.Count; i++)
            {//check if connection exists
                if (connections[i].GetInNode()== node1 && connections[i].GetOutNode() == node2)
                {
                    connectionExists = true;
                    break;//or call method again
                }

            }
           // if (node1.GetLayer() >= node2.GetLayer()) {
               // connectionExists = true;
           // }
            
            if (!connectionExists)
            {
                connections.Add(new ConnectionGene(node1, node2, Random.Range(-1.0f, 1.0f), true, track.Innovate()));
                return;
            }
        }

    }
    public void AddNodeMutation() {
        if (neat.maxNeurones <= nodes.Count)
        {
            return;
        }
       
        ConnectionGene connectionSplit = connections[Random.Range(0, connections.Count)];//choose random connection to split
        connectionSplit.Disable();
       


        NodeGene nodeIn = connectionSplit.GetInNode();
       
        NodeGene nodeOut = connectionSplit.GetOutNode();
     
        //Debug.Log("nin" + nodeIn.GetLayer());
        //Debug.Log("nout"+ nodeOut.GetLayer());
        int newLayer;
        if (nodeOut.GetLayer() - nodeIn.GetLayer() == 1)
        {
            //Debug.Log(nodeIn.GetLayer());
            //Debug.Log(nodeOut.GetLayer());
            newLayer = nodeOut.GetLayer();
            AddLayer(nodeOut.GetLayer());
            //Debug.Log("new layer" + nodeOut.GetLayer());
            //newLayer = nodeOut.GetLayer();
        }
        else {
            newLayer = Random.Range(nodeIn.GetLayer()+1, nodeOut.GetLayer()-1);
        }


        NodeGene node = new NodeGene(NodeGene.NODETYPE.HIDDEN, track.NewId(), newLayer, Random.Range(-1.0f, 1.0f),neat.internalActiationFunction);
        nodes.Add(node);

        ConnectionGene connectionIn = new ConnectionGene(connectionSplit.GetInNode().Copy(),node,1.0f,true,track.Innovate());
        ConnectionGene connectionOut = new ConnectionGene(node, connectionSplit.GetOutNode().Copy(), connectionSplit.GetWeight(), true, track.Innovate());
        connections.Add(connectionIn);
        connections.Add(connectionOut);


    }
    public void EnableConnectionMutation() {
        List<ConnectionGene> disabledCons = new List<ConnectionGene>();
        for (int i = 0; i < connections.Count; i++) {
            if (connections[i].GetExpressed() == false) {
                disabledCons.Add(connections[i]);
            }

        }
        if (disabledCons.Count != 0) {
            disabledCons[Random.Range(0, disabledCons.Count)].SetExpressed(true);
        }
       


    }

    public void DisableConnectionMutation()
    {
        List<ConnectionGene> enabledCons = new List<ConnectionGene>();
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].GetExpressed() == true)
            {
                enabledCons.Add(connections[i]);
            }

        }
        if (enabledCons.Count != 0)
        {
            enabledCons[Random.Range(0, enabledCons.Count)].SetExpressed(false);
        }


    }

    public NodeGene FindNode(int id) {
        for (int i=0; i< nodes.Count; i++) {
            if (nodes[i].GetId()==id) {
               // Debug.Log("fnd" + nodes[i].GetId());
                return nodes[i];
            }
        }

        return null;
    }
    public List<NodeGene> FindNodes(int layer)
    {
        List<NodeGene>  nodesInLayer = new List<NodeGene>();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetLayer() == layer)
            {
                nodesInLayer.Add(nodes[i]);
              
            }
        }

        return nodesInLayer;
    }

    public void AddLayer(int newLayer)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            //Debug.Log("nxt" + nodes[i].GetLayer());
            if (nodes[i].GetLayer() >= newLayer)
            {
                //Debug.Log("nxt" + nodes[i].GetLayer());
                nodes[i].MoveToNextLayer();

            }
        }
        layers++;
    }
    public int GetLayers() {
        return layers;

    }
    public List<ConnectionGene> FindNodeConnections(NodeGene node) {
        List<ConnectionGene> nodeConnections = new List<ConnectionGene>();
        //Debug.Log("search"+node);
        
        for (int i = 0; i < connections.Count; i++) {
            //Debug.Log(connections[i].GetOutNode());
            //Debug.Log(i+"   "+node.GetId() +"   "+ connections[i].GetOutNode().GetId());
            if (node.GetId() == connections[i].GetOutNode().GetId()) {
                //Debug.Log("found "+i + "   " + node.GetId() + "   " + connections[i].GetOutNode().GetId());
                nodeConnections.Add(connections[i]);
            }

        }
        //Debug.Log(nodeConnections.Count);
        return nodeConnections;

    }

    public Genome Copy() {
        Genome g = new Genome(track);
        List<NodeGene> nodesList = new List<NodeGene>();
        for (int i = 0; i < nodes.Count; i++) {
            nodesList.Add(  nodes[i].Copy());
            g.AddNodeGene(nodesList[i]);
        }
        
        List<ConnectionGene> connectionsList = new List<ConnectionGene>();
        for (int i = 0; i < connections.Count; i++)
        {
            connectionsList.Add( connections[i].Copy());
            connectionsList[i].SetInNode(g.FindNode(connections[i].GetInNode().GetId()));
            connectionsList[i].SetOutNode(g.FindNode(connections[i].GetOutNode().GetId()));
            //Debug.Log(connectionsList[i]);
            g.AddConnectionGene(connectionsList[i]);
        }
        g.SetLayers(layers);
        return g;
    }

    public override string ToString()
    {
        
        string s="Species: "+speciesId+"    Layers:"+layers+"\nNodes: "+nodes.Count;
       
        for (int i = 0; i < nodes.Count; i++) {
            s = s +"\n" + nodes[i].ToString();
        }

        s = s+ "\nConnections: "+connections.Count;

        for (int i = 0; i < connections.Count; i++)
        {
            s = s + "\n" + connections[i].ToString();
        }

        return s;
    }

    /*int IComparer.Compare(object a, object b)
    {
        Genome g1 = (Genome)a;
        Genome g2 = (Genome)b;
        if (g1.fitness > g2.fitness)
            return 1;
        if (g1.fitness < g2.fitness)
            return -1;
        else
            return 0;
    }

    */
    public float[] Run(float[] inputs)
    {
        int inputIndex = 0;
        List<NodeGene> nodesInLayer = null;
        //Debug.Log(g1.GetLayers());
       
        for (int i = 0; i <= GetLayers(); i++)//for each layer
        {
            //Debug.Log(g1);

            nodesInLayer = FindNodes(i);
            //Debug.Log(g1);
            //Debug.Log(nodesInLayer.Count);//+"\n"+g1);

            //Debug.Log(nodesInLayer[0]);
            //Debug.Log(nodesInLayer[1]);
            for (int j = 0; j < nodesInLayer.Count; j++)//for each node in the layer
            {
                if (i == 0)//if first layer which is the input layer
                {
                    float[] input = new float[1];//takes in input as array
                    try
                    {
                        input[0] = inputs[inputIndex];//add input at current index
                    }
                    catch (System.Exception e) {
                        Debug.Log(e+"error "+this);
                    }
                    //input[0] = inputs[inputIndex];//add input at current index
                    inputIndex++;
                    nodesInLayer[j].Calc(input);//calculate neauron output
                   
                    //Debug.Log(input[0]);
                    //Debug.Log("in: "+j+" " + nodesInLayer[j].GetOutput());

                }
                else//if hidden or output
                {
                   
                    List<ConnectionGene> nodeConnections = FindNodeConnections(nodesInLayer[j]);//find the input connections to the neurone
                   
                    // Debug.Log(nodesInLayer[j]+" "+nodeConnections.Count);
                    float[] input = new float[nodeConnections.Count];
                    for (int k = 0; k < nodeConnections.Count; k++)//for each connection
                    {

                        //NodeGene node = g1.FindNode(nodeConnections[k].GetInNode());
                        NodeGene node = nodeConnections[k].GetInNode();//get the input node
                       
                        if (nodeConnections[k].GetExpressed())
                        {
                            
                            input[k] = node.GetOutput() * nodeConnections[k].GetWeight();//output from previous neruone * weight of connection
                           
                        }
                        else
                        {
                            input[k] = 0.0f;//if not expressed just 0
                        }


                    }
                    nodesInLayer[j].Calc(input);//calculate output of all incoming connections
                    
                    //Debug.Log("outlaer: " + j + " " + nodesInLayer[j].GetOutput());


                }

            }
        }
        List<NodeGene> nodesInOutput = FindNodes(GetLayers());
        float[] output = new float[nodesInOutput.Count];
        //Debug.Log(g1.GetLayers());
        //Debug.Log(nodesInOutput.Count);
        for (int j = 0; j < nodesInOutput.Count; j++)//for each neruone in the output
        {
          

            output[j] = nodesInOutput[j].GetOutput();
            
            //Debug.Log(output[j]);


        }


        return output;


    }
    public int CalcNumbDisjoint(Genome genome)
    {
        int disjoint = 0;
        for (int i = 0; i < connections.Count; i++) {
            if (!genome.connections.Contains(connections[i])){
                disjoint++;
            }

        }
        for (int i = 0; i < genome.connections.Count; i++)
        {
            if (!connections.Contains(genome.connections[i]))
            {
                disjoint++;
            }

        }
       
        return disjoint;

    }


    public float CalcAvgWeightDiff(Genome genome)
    {
        int sharedGenes = 0;
        float sum = 0;
        for (int i = 0; i < connections.Count; i++)
        {
            if (genome.connections.Contains(connections[i]))
            {
                //Debug.Log(genome.connections.Count);
                //Debug.Log(genome.connections[genome.connections.IndexOf(connections[i])].GetInnovation() + "   " + connections[i].GetInnovation()+" "+genome.connections[i].GetInnovation());
               
                sum += Mathf.Abs(genome.connections[genome.connections.IndexOf(connections[i])].GetWeight() - connections[i].GetWeight());
                sharedGenes++;
            }

        }
        if (sharedGenes == 0) {
            return 0;
        }
        return sum/sharedGenes;

    }
    public bool CheckMatchingSpecies(Genome genome)
    {
        int n = Mathf.Max(connections.Count, genome.connections.Count);
        //Debug.Log(this);
        //Debug.Log(genome);
        //CalcNumbDisjoint(genome);
        //CalcAvgWeightDiff(genome);
        //Debug.Log(neat.coeffWeightDiff);
        float compatibilityDistance =( CalcNumbDisjoint(genome) * neat.coeffDisjoint)/n + CalcAvgWeightDiff(genome) * neat.coeffWeightDiff;
        //Debug.Log(compatibilityDistance);
        return compatibilityDistance < neat.threshold;
       

    }
    public void SetNeat() {
        neat = GameObject.Find("Scripts").GetComponent<Neat>();

    }

   


}
