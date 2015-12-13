using System;
using UnityEngine;

public class Tail : MonoBehaviour
{
    private const float FOLLOW_SPEED = 7.0f;
    private const float MIN_DISTANCE_TO_PARENT = 0.2f;

    public GameObject Head;

    public void SetParent(GameObject head)
    {
        Head = head;
    }

    public void FollowParent(GameObject parent)
    {
        var distanceFromParent =
            (float)Math.Sqrt(Mathf.Pow(parent.transform.position.x - transform.position.x, 2) +
                      Mathf.Pow(parent.transform.position.y - transform.position.y, 2));

        float followSpeed = FOLLOW_SPEED * distanceFromParent;

        if (distanceFromParent > MIN_DISTANCE_TO_PARENT)
        {
            float angle = Mathf.Atan2(parent.transform.position.y - transform.position.y,
                parent.transform.position.x - transform.position.x);

            transform.position += new Vector3(Mathf.Cos(angle)*followSpeed*Time.deltaTime,
                Mathf.Sin(angle)*followSpeed*Time.deltaTime);
        }
    }
}