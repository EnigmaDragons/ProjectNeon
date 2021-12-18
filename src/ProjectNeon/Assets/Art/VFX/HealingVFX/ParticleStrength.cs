using UnityEngine;
using System.Collections;
 
public class ParticleStrength : MonoBehaviour 
{
    public ParticleSystem particleA;
	public ParticleSystem particleB;
	public ParticleSystem particleC;
	public ParticleSystem particleD;
 
  	public void StrengthParticlePlay()
    { 
		particleA.Play();
		particleB.Play();
		particleC.Play();
		if (particleD != null)
			particleD.Play();
	}        
}
