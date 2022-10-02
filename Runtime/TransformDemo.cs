using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Storm.SploinkySpring; 

public class TransformDemo : MonoBehaviour
{
    [FormerlySerializedAs("test")]
    public SploinkyTransform transform;
    public Transform pTransform; 
    public Transform[] demoArray;
    public int index;
    public float timer;
    public float t; 
    
    // Start is called before the first frame update
    void Start()
    {
        timer = Time.fixedTime; 
    }

    // Update is called once per frame
    void Update()
    {


        if ((Time.fixedTime - timer) > t)
        {
            if (index >= demoArray.Length - 1)
            {
                index = 0; 
            }
            else
            {
                index += 1;    
            }
            transform.SetTarget(GetCurrentTransform());
            
            pTransform.position = GetCurrentTransform().position; 
            pTransform.rotation = GetCurrentTransform().rotation;
            pTransform.localScale = GetCurrentTransform().localScale; 
            timer = Time.fixedTime; 
        }

    }

    public Transform GetCurrentTransform()
    {
        print("DONE");
        return demoArray[index];
    }
}
