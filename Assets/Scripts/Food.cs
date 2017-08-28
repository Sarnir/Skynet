using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour, IEatable {

	public int Calories;

	public int Eat()
	{
		Destroy (gameObject);
		return Calories;
	}
}
