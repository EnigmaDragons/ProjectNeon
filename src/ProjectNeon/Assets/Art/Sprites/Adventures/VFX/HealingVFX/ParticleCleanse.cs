using UnityEngine;
using System.Collections;
 
public class ParticleCleanse : MonoBehaviour 
{
    public ParticleSystem particleA;
	public ParticleSystem particleB;
	public ParticleSystem particleC;
	public ParticleSystem particleD;
 
  	public void CleanseParticlePlay()
    { 
		particleA.Play();
		particleB.Play();
		particleC.Play();
		particleD.Play();
	}        
           
}