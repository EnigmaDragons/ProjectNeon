using UnityEngine;
using System.Collections;
 
public class ParticleHit : MonoBehaviour 
{
    public ParticleSystem particleA;
	public ParticleSystem particleB;
	public ParticleSystem particleC;
	public ParticleSystem particleD;
 
  	public void HitParticlePlay()
    { 
		particleA.Play();
		particleB.Play();
		particleC.Play();
		particleD.Play();
	}        
           
}