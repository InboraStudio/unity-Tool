using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectRotate : MonoBehaviour
{
    [Range(0, 360)] [SerializeField] private float rotateSpeed = 360;
    private Vector3 _rotate;

    void Start()
    {
        _rotate = Vector3.forward * rotateSpeed;
    }
    
    private void Update()
    {
        transform.Rotate(_rotate * Time.smoothDeltaTime);
    }
}
