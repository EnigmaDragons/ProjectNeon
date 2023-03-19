using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RandomPlayAnimation : MonoBehaviour 
{
 
	private SpriteRenderer spriteR;
	
	public void Awake()
    { 
		spriteR = gameObject.GetComponent<SpriteRenderer>();
		
		this.spriteR.enabled = false;
		this.SafeCoroutineOrNothing(Wait());
	}     

	 private IEnumerator  Wait()
    {
		yield return new WaitForSeconds(Random.Range(5, 20)); //for waiting
		Toggle();
	}
	
	public void Toggle()
    { 
		if (this.spriteR.enabled == true) {
			this.spriteR.enabled = false; 
			this.SafeCoroutineOrNothing(Wait());
			} 	
		 else
			{ 
			this.spriteR.enabled = true;
			this.SafeCoroutineOrNothing(Wait());
			} 
	 } 
	 
}