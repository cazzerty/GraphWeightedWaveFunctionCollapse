using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DistanceTester : MonoBehaviour
{
    [SerializeField] private GraphManager graphManager;

    [SerializeField] private double distance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            
            this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        
        if(!graphManager){return;}

        distance = graphManager.GetDistanceToClosestEdge(gameObject.transform.position);
    }
}
