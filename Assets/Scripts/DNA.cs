using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour
{
	public int MaxCalories;
	public float MaxSpeed;
	public float RayDistance;

	[Range(0f, 1f)]
	public float Uncertainty;

	// 'random' weights & other data for neural network
}
