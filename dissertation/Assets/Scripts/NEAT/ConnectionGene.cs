using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConnectionGene 
{
    [SerializeField]
    private NodeGene inNode;
    [SerializeField]
    private NodeGene outNode;
    [SerializeField]
    private float weight;
    [SerializeField]
    private bool expressed;
    [SerializeField]
    private int innovation;
    [SerializeField]
    private Neat neat;
   

    public ConnectionGene(NodeGene inNode, NodeGene outNode, int innovation)
    {
        this.inNode = inNode;
        this.outNode = outNode;
        neat = GameObject.Find("Scripts").GetComponent<Neat>();
        weight = Random.Range(-neat.weightRange, neat.weightRange);
        expressed = true;
        this.innovation = innovation;


    }


    public ConnectionGene(NodeGene inNode, NodeGene outNode, bool expressed, int innovation)
    {
        this.inNode = inNode;
        this.outNode = outNode;
        weight = Random.Range(-neat.weightRange, neat.weightRange);
        this.expressed = expressed;
        this.innovation = innovation;


    }

    public ConnectionGene(NodeGene inNode, NodeGene outNode,float weight,bool expressed,int innovation) {
        this.inNode = inNode;
        this.outNode = outNode;
        this.weight = weight;
        this.expressed = expressed;
        this.innovation = innovation;


    }

    public NodeGene GetInNode() {

        return inNode;
    }
    public NodeGene GetOutNode()
    {

        return outNode;
    }
    public void SetInNode(NodeGene inNode)
    {

        this.inNode = inNode;
    }
    public void SetOutNode(NodeGene outNode)
    {

        this.outNode = outNode;
    }
    
    public float GetWeight() {
        return weight;

    }
    public void SetWeight(float weight) {
        this.weight = weight;

    }

    public int GetInnovation() {
        return innovation;

    }
    public bool GetExpressed() {
        return expressed;

    }
    public void SetExpressed(bool expressed)
    {
        this.expressed = expressed;

    }
    public void Disable() {
        expressed = false;

    }
    public ConnectionGene Copy() {
        return new ConnectionGene(inNode, outNode, weight, expressed, innovation);


    }
    public override bool Equals(object obj)
    {

        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;

        }
        else {
            ConnectionGene con = (ConnectionGene)obj;
            return (innovation == con.GetInnovation());
        }
    }
    public override int GetHashCode()
    {
        return innovation;
    }

    public override string ToString()
    {
        return "innovation: " + innovation + "  Nodes: " + inNode.GetId() + " -> " + outNode.GetId()+"    weight: "+weight+"  expressed: "+expressed+"      Layer: "+inNode.GetLayer()+"- > "+outNode.GetLayer()+"  nodeIn: "+inNode+"    Nodeout: "+outNode;
    }


    


}
