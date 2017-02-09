using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoSingleton<FlockingManager>
{
    public List<Flock> MainBoids = new List<Flock>();
    public List<Flock> boids = new List<Flock>();
    public bool dispalyGridGizmos = false;
    public float avrSpeed = 0f;
    public Vector3 avrPosition = Vector3.zero;
    public Vector3 centerPos = Vector3.zero;
    void Start()
    {

    }

    void Update()
    {
        CenterPosition();
        UpdateMainBoid();
    }

    void CenterPosition()
    {
        int count = boids.Count;
        Vector3 tempPos = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            tempPos += boids[i].transform.position;
        }
        avrPosition = tempPos / count;
    }
    void UpdateMainBoid()
    {
        int count = boids.Count;
        MainBoids.Clear();
        for (int i = 0; i < count; i++)
        {
            bool isMain = boids[i].GetComponentInParent<BoidGroup>().isMainGroup;
            if (isMain)
            {
                MainBoids.Add(boids[i]);
            }
        }

        int mainCount = MainBoids.Count;
        for (int i = 0; i < mainCount; i++){
            MainBoids[i].IsHeader = (i == 0);
        }


    }
    void UpdateCenter()
    {
        Vector3 tempPos = Vector3.zero;
        int count = MainBoids.Count;
        for (int i = 0; i < count; i++)
        {
            tempPos += MainBoids[i].transform.position;
        }

        centerPos = (tempPos+GoalController.instance.transform.position)/ (count+1);
    }

}
