using System;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 200f;

    private void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed ;
        
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        
        
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }
}
