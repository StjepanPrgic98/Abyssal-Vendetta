using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Transform movingPlatformTransform;

    [Header("Variables")]
    [SerializeField] Vector3 startPositon;
    [SerializeField] Vector3 endPositon;
    [SerializeField] float travelSpeed;


    bool moveToEndPosition = true;
    

    void Update()
    {
        CheckIfEndPositionIsReached();

        if (moveToEndPosition)
        {
            movingPlatformTransform.position = Vector3.MoveTowards(movingPlatformTransform.position, endPositon, travelSpeed);
        }
        else
        {
            movingPlatformTransform.position = Vector3.MoveTowards(movingPlatformTransform.position, startPositon, travelSpeed);
        }
    }
    

    void CheckIfEndPositionIsReached()
    {
        if(movingPlatformTransform.position == endPositon)
        {
            moveToEndPosition = false;
        }
        else if(movingPlatformTransform.position == startPositon)
        {
            moveToEndPosition = true;
        }
    }
        
}
