using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackroundMove2 : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float[] points;
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position += Vector3.down * _speed * Time.deltaTime;

            if (transform.GetChild(i).position.y <= points[0])
            {
                transform.GetChild(i).position = new Vector3(0, points[1]);
            }
        }
    }
}
