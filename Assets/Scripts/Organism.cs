#define NN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organism : MonoBehaviour, IEatable
{
	enum Task
	{
		Idle,
		SearchForFood
	}

	int calories;

	Task currentTask;
	Rigidbody body;
	Vector3 destination;
	bool isFoodSeen = false;

	DNA dna;
	NeuralNetwork brain;

	System.DateTime birthDate;

	// Use this for initialization
	void Awake ()
	{
		birthDate = System.DateTime.Now;

		Random.InitState (System.Environment.TickCount);
		body = GetComponentInChildren<Rigidbody> ();
		dna = GetComponent<DNA> ();
		brain = GetComponent<NeuralNetwork> ();
		// TODO: create proper component for head and body

		currentTask = Task.Idle;
		calories = dna.MaxCalories/2 + 1;
	}

	// Update is called once per frame
	void Update ()
	{
		#if NN
		var uncertainParam = Random.value > dna.Uncertainty ? 1f : Random.value;
		var inputs = new float[5] { (float)calories,
									isFoodSeen ? 1f : 0f,
									transform.position.x,
									transform.position.z,
									uncertainParam };
		var output = brain.Process(inputs);

		LookForObjects(new Vector3(output[0], 0f, output[1]));
		body.velocity += new Vector3(output[2], 0f, output[3]);
		body.velocity = Vector3.ClampMagnitude (body.velocity, dna.MaxSpeed);
		#else
		if (currentTask == Task.SearchForFood)
			SearchForFood ();
		#endif
		SpendCalories ();
	}

	public void Consume(IEatable food)
	{
		calories += food.Eat ();
		Debug.Log ("Now " + gameObject.name + " has " + calories + " calories!");
	}

	void SpendCalories()
	{
		calories--; // basic calory intake

		calories -= (int)body.velocity.magnitude; // moving costs calories

		if (calories < 0)
			Die ();
		else if (calories < dna.MaxCalories / 2)
			currentTask = Task.SearchForFood;
		else
			currentTask = Task.Idle;
	}

	bool destinationSet;

	void SearchForFood()
	{
		if (!destinationSet)
		{
			destination = new Vector3 (Random.value * 2f - 1f, 0f, Random.value * 2f - 1f) * dna.RayDistance;

			var rayDirection = new Vector3 (Random.value * 2f - 1f, 0f, Random.value * 2f - 1f);

			RaycastHit hitInfo;
			Debug.DrawRay (transform.position, rayDirection);
			if (Physics.Raycast (transform.position, rayDirection, out hitInfo, dna.RayDistance)) {
				if (hitInfo.transform.gameObject.GetComponent<Food> () != null) {
					destination = hitInfo.transform.position;
					destinationSet = true;
					Debug.Log ("Destination set to " + destination);
				}
			}
		}

		body.velocity += (destination - transform.position).normalized * dna.MaxSpeed;
		body.velocity = Vector3.ClampMagnitude (body.velocity, dna.MaxSpeed);
	}

	void LookForObjects(Vector3 direction)
	{
		var rayDirection = new Vector3 (direction.x, 0f, direction.z);
		isFoodSeen = false;

		RaycastHit hitInfo;
		Debug.DrawRay (transform.position, rayDirection);
		if (Physics.Raycast (transform.position, rayDirection, out hitInfo, dna.RayDistance)) {
			if (hitInfo.transform.gameObject.GetComponent<Food> () != null)
			{
				isFoodSeen = true;
				Debug.Log (gameObject.name + " can see food!");
			}
		}
	}

	public int Eat()
	{
		return calories*2;
	}

	void Die()
	{
		var lifetime = System.DateTime.Now - birthDate;
		Debug.Log (gameObject.name + " is now dead. He lived for " + lifetime.Seconds + " seconds.");
		Destroy (gameObject);
	}

	void OnCollisionEnter(Collision collision)
	{
		var list = collision.gameObject.GetComponents<MonoBehaviour>();
		foreach(MonoBehaviour mb in list)
		{
			if (mb is IEatable)
			{
				Consume (mb as IEatable);
				destinationSet = false;
			}
		}
	}
}
