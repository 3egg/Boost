using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] private Vector3 movementVector = new Vector3(10f, 10f, 10f);

    /*[SerializeField] [Range(0, 1)]*/
    private float movementFactor;
    [SerializeField] private float period = 2f;
    private Vector3 startPos;

    // Use this for initialization
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    //bool flag = false;

    void Update()
    {
        if (Math.Abs(transform.position.z + 5f) > 0.1f)
        {
            transform.position = new Vector3(0, 0, 5f);
        }

        /*if (flag == false && movementFactor <= 0.5f)
        {
            movementFactor += 0.002f;
            if (movementFactor > 0.5f)
            {
                flag = true;
            }

            
        }

        else
        {
            movementFactor -= 0.002f;
            if (movementFactor <= 0f)
            {
                flag = false;
            }  
        }*/
        if (period <= Mathf.Epsilon) return;
        float cycles = Time.time / period; //从0一直增长
        const float tau = Mathf.PI * 2f; //6.28 2pi
        float rawSinWave = Mathf.Sin(cycles * tau); //-1 to 1 不管数值怎么大,都是-1 到 1 之间
        movementFactor = rawSinWave / 2f + 0.5f; // -0.5 to 0.5 我们的移动范围是 -0.5 到 0.5
        Vector3 offset = movementFactor * movementVector;
        transform.position = offset + startPos;
    }
}