using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeGene  {
    [SerializeField]
    private NODETYPE nodeType;
    [SerializeField]
    private ACTIVATION_FUNCTION activationFunction;
    [SerializeField]
    private int id;
    [SerializeField]
    private float bias;
    [SerializeField]
    private int layer;
    [SerializeField]
    private float output;
    [SerializeField]
    private Neat neat;

    public enum NODETYPE
    {
        INPUT,
        HIDDEN,
        OUTPUT
    };
    public enum ACTIVATION_FUNCTION
    {
        SIGMOID,
        TANH,
        STEP,
        NULL
    };


    public NodeGene(NODETYPE nodeType, int id, int layer, bool biasExpressed,ACTIVATION_FUNCTION activationFunction)
    {
      
        this.nodeType = nodeType;
        this.id = id;
        this.layer = layer;
        neat = GameObject.Find("Scripts").GetComponent<Neat>();
        if (biasExpressed)
        {
            bias = Random.Range(-neat.weightRange, neat.weightRange);
        }
        else {
            bias = 0;
        }
        this.activationFunction = activationFunction;
       

    }


    public NodeGene(NODETYPE nodeType,int id,int layer,float bias, ACTIVATION_FUNCTION activationFunction) {
        this.nodeType = nodeType;
        this.id = id;
        this.layer = layer;
        this.bias = bias;
        this.activationFunction = activationFunction;
    }

    public int GetId() {

        return id;
    }

    public float GetOutput()
    {

        return output;
    }
    public NODETYPE GetNodeType() {

        return nodeType;
    }
    public float GetBias() {
        return bias;

    }
    public void SetBias(float bias)
    {
        this.bias = bias;

    }

    public int GetLayer()
    {
        return layer;

    }
    public void MoveToNextLayer()
    {
        layer++;

    }
    public NodeGene Copy() {
        return new NodeGene(nodeType, id, layer, bias, activationFunction);

    }



    public float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Pow((float)System.Math.E, -x));


    }
    public float Tanh(float x)
    {
        //Debug.Log(x);
        return (float)System.Math.Tanh(x);

    }
    public float Step(float x)
    {
        if (x < 0.5f) {
            return 0.0f;
        }
        return 1.0f;

    }

    public float Calc(float[] input) {
        if (input.Length == 0) {//if no inputs not connected
            output = 0.0f;
            //output = Random.Range(-1.0f, 1.0f);
           
            return 0.0f;
        }
        float sum = 0;
        for (int i = 0; i < input.Length; i++) {
            sum += input[i];

        }
        /*if (nodeType == NODETYPE.OUTPUT) {
            if ((sum + bias) > 0.5)
            {
                return 1.0f;
            }
            else { return 0.0f; }
        }*/
        switch (activationFunction) {
            case ACTIVATION_FUNCTION.SIGMOID:
                output = Sigmoid(sum + bias);
                break;
            case ACTIVATION_FUNCTION.TANH:
                output = Tanh(sum + bias);
                break;
            case ACTIVATION_FUNCTION.STEP:
                output = Step(sum + bias);
                break;
            default:
                output = sum + bias;
                break;


        }
        //output = Tanh(sum + bias);
        //output = Random.Range(-1.0f,1.0f);
        //Debug.Log(output);
        return output;


    }



    public override bool Equals(object obj)
    {

        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
           
            return false;

        }
        else
        {
            NodeGene node = (NodeGene)obj;
           
            return (id == node.GetId());
        }
    }
    public override int GetHashCode()
    {
        return id;
    }
    public override string ToString()
    {
        return "id: "+id+"  type: "+nodeType.ToString()+"   layer: "+layer+"    bias: "+bias;
    }


}
