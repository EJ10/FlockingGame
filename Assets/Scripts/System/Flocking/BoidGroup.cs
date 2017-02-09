using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoidGroup : MonoBehaviour
{
    public bool isMainGroup = false;
    public GameObject groupTarget;
    public List<Flock> boids = new List<Flock>();

    public GameObject boidPrefab;
    GameObject[] boidObject;
    public int maxXNum = 2;
    public int maxYNum = 3;
    public int maxZNum = 4;

    public Text boidCountText;
    int boidCount;

    void Start()
    {
        Create();
    }
	
	void Update ()
    {
        if(isMainGroup)
            boidCountText.text = boids.Count.ToString();
        UpdateBoid();
    }
    void Create()
    {
        boidCount = maxZNum * maxYNum * maxXNum;
        boidObject = new GameObject[boidCount];
        for (int k = 0; k < maxZNum; k++)
        {
            for (int j = 0; j < maxYNum; j++)
            {
                for (int i = 0; i < maxXNum; i++)
                {
                    int sNum = k * maxXNum * maxYNum + j * maxXNum + i;
                    boidObject[sNum] = Instantiate(boidPrefab, transform.position + Vector3.right * i + Vector3.up * j + Vector3.forward * k, transform.rotation);
                    boidObject[sNum].transform.localScale = transform.localScale;
                    boidObject[sNum].transform.SetParent(transform);
                    Flock flock = boidObject[sNum].GetComponent<Flock>();
                    flock.goal = groupTarget;
                    boids.Add(flock);
                }
            }
        }
    }
    void UpdateBoid()
    {
       boids.Clear();
       Flock[] childboids = GetComponentsInChildren<Flock>();
        for (int i = 0; i < childboids.Length;i++)
        {
            boids.Add(childboids[i]);
        }

    }
    public void SpawnBoid()
    {
        GameObject boid = Instantiate(boidPrefab, groupTarget.transform.position, groupTarget.transform.rotation);
        boid.transform.localScale = transform.localScale;
        boid.transform.SetParent(transform);
        Flock flock = boid.GetComponent<Flock>();
        flock.goal = groupTarget;
        boids.Add(flock);
    }
}
