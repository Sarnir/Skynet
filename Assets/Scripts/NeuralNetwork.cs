using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public delegate float ActivationFunction(float input);

public class NeuralNetwork : MonoBehaviour
{
	public int InputsNum;
	public int[] Layers;
	ActivationFunction Activate;

	NNLayer[] NNlayers;

	public float[] Process(float[] inputs)
	{
		List<float> NNinputs = new List<float> (inputs);

		for (int i = 0; i < NNlayers.Length; i++)
		{
			NNlayers [i].inputs = NNinputs.ToArray ();
			NNinputs = NNlayers [i].GenerateOutputs (Sigmoid);
		}

		return NNinputs.ToArray ();
	}

	// Use this for initialization
	void Start ()
	{
		Activate = test;

		NNlayers = new NNLayer[Layers.Length];

		int inputsNum = InputsNum;

		for (int i = 0; i < NNlayers.Length; i++)
		{
			NNlayers [i] = new NNLayer (inputsNum, Layers [i]);
			inputsNum = Layers [i];
		}
	}

	public float[] Init()
	{
		return null;
	}

	float Sigmoid(float x)
	{
		return 1f / (float)(1 + Mathf.Exp (-x));
	}

	float test(float tt)
	{
		return tt;
	}
}

public class NNLayer
{
	public float[] inputs; // inputy są wspólne dla każdego perceptronu z danego layera!
	public Perceptron[] nodes;

	public NNLayer(int inputNum, int nodesNum)
	{
		nodes = new Perceptron[nodesNum];
		for (int i = 0; i < nodesNum; i++)
		{
			nodes [i] = new Perceptron ();
			nodes [i].weights = new float[inputNum];
			for (int j = 0; j < nodes [i].weights.Length; j++)
			{
				nodes [i].weights [j] = (Random.value * 2f - 1) * Random.Range(0f, 10f);
			}
		}
	}

	public List<float> GenerateOutputs(ActivationFunction activationFunc)
	{
		List<float> outputs = new List<float> ();
		for (int i = 0; i < nodes.Length; i++)
		{
			outputs.Add(nodes [i].GenerateOutput (inputs, activationFunc));
		}

		return outputs;
	}
}

public class Perceptron
{
	public float[] weights;

	public float GenerateOutput(float[] inputs, ActivationFunction activationFunc)
	{
		Assert.AreEqual (inputs.Length, weights.Length);

		float sum = 0f;
		for (int i = 0; i < inputs.Length; i++)
		{
			sum += inputs [i] * weights [i];
		}

		return activationFunc (sum);
	}
}