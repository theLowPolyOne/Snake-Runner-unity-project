using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLine : MonoBehaviour, IRecolourible
{
	[SerializeField] Material colorMaterial;
	private Color color;

    private void OnTriggerEnter(Collider target)
	{
		if (target.tag == "Player")
		{
			PlayerController player = target.GetComponent<PlayerController>();
			player.SetColor(color);
		}
	}

	public void SetColorMaterial(Material newMaterial)
	{
		colorMaterial = newMaterial;
		color = colorMaterial.color;

		gameObject.SetActive(false);
		GetComponentInChildren<Renderer>().sharedMaterial = colorMaterial;

		ParticleSystem[] sprayParticles = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem spray in sprayParticles)
		{
			var main = spray.main;
			main.startColor = color;
		}
		gameObject.SetActive(true);
	}
}
