using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
        if (!cam)
        {
            cam = GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetKeyUp(KeyCode.Minus))
        {
            cam.orthographicSize = cam.orthographicSize + 1;
        }
        if (UnityEngine.Input.GetKeyUp(KeyCode.Equals))
        {
            cam.orthographicSize = cam.orthographicSize - 1;
        }

        Vector3 mov = new Vector3(UnityEngine.Input.GetAxis("Horizontal") * Time.deltaTime,UnityEngine.Input.GetAxis("Vertical") * Time. deltaTime);
        mov = mov * speed;
        
        cam.gameObject.transform.position += mov;
    }
}
