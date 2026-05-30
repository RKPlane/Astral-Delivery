using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class MassSystem : MonoBehaviour
{
	public float baseMass = 1f;

	private PlayerController player;
	private float attachedMass = 0f;

	public float TotalMass => baseMass + attachedMass;
	public float Momentum => TotalMass * player.Velocity.magnitude;
	public float KineticEnergy
	{
		get { float v = player.Velocity.magnitude; return 0.5f * TotalMass * v * v; }
	}

	void Awake()
	{
		player = GetComponent<PlayerController>();
		player.SetMass(baseMass);
	}

	public void AddMass(float m)
	{
		attachedMass += m;
		player.SetMass(TotalMass);
	}

	public void RemoveMass(float m)
	{
		attachedMass = Mathf.Max(0f, attachedMass - m);
		player.SetMass(TotalMass);
	}

	public void ResetToBase()
	{
		attachedMass = 0f;
		player.SetMass(baseMass);
	}
}