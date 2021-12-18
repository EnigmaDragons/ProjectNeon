using UnityEngine;
using System.Collections;
 
public class ParticleHealing : MonoBehaviour 
{
    public ParticleSystem particleA;
	public ParticleSystem particleB;
	public ParticleSystem particleC;
	public ParticleSystem particleD;
 
  	public void HealingParticlePlay()
    { 
		particleA.Play();
		particleB.Play();
		particleC.Play();
		particleD.Play();
	}        
           
}