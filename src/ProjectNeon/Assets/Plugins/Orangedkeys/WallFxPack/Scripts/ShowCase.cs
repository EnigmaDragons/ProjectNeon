using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCase : MonoBehaviour
{

    public float speed;
    public int childc;
    public GameObject grandChild;

    // Start is called before the first frame update
    void Start()
    {
        childc = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Mathf.Floor(Time.time));
        int index = (int)Mathf.Floor(Time.time * speed);
        //Debug.Log(this.gameObject.transform.GetChild(index).name);
        for (int i = 0; i < childc; i++)
        {
            if (i == index)
            {


                transform.GetChild(i).gameObject.SetActive(true);
            }
            else transform.GetChild(i).gameObject.SetActive(false);


        }
    }
}
