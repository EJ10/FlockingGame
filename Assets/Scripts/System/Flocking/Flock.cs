using UnityEngine;
using System.Collections;

public class Flock : MonoBehaviour
{
    public GameObject goal;

    public int id;
    public float speed = 0.001f;
    public float MaxFlySpeed = 5f;
    public float MaxGroundSpeed = 0.5f;
    public float separationDist = 0.5f;
    float rotationSpeed = 10f;
    float neighbourDistance = 10000f;

    Vector3 vcenter;
   
    BoidAIController boisAIController;
    Rigidbody boidRigid;

    [SerializeField] private bool isHeader = false;
    public bool IsHeader { get { return isHeader; } set { isHeader = value; } }
    void Start ()
    {
        id  = GameInfo.boidCount;
        GameInfo.boidCount++;
        FlockingManager.instance.boids.Add(this);
        boisAIController = GetComponent<BoidAIController>();
        boidRigid = GetComponent<Rigidbody>();

        if (goal.GetComponent<GoalController>() != null)
            speed = goal.GetComponent<GoalController>().speed;
        else
            speed = 3f;
    }
	
	void Update ()
    {
        if (goal == null || boisAIController.IsDie)
            return;

        if (!isHeader)
        {
            if (Random.Range(0, 5) < 1)
                ApplyRules();

            if (speed > 0)
                transform.Translate(Vector3.forward * (Time.deltaTime * speed));
        }
        else
        {
            HeadRules();
        }
    }
    void HeadRules()
    {
        float smoothSpeed = 0.03f;

        Vector3 desirePosition = goal.transform.position;
        transform.position = Vector3.Lerp(transform.position, desirePosition, smoothSpeed);
        
        Vector3 dir = goal.transform.position - transform.position;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                 Quaternion.LookRotation(dir),
                                 rotationSpeed * Time.deltaTime);
        }
    }
    void ApplyRules()
    {
        Flock[] gos;
        gos = GetComponentInParent<BoidGroup>().boids.ToArray();

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;

        float gSpeed = 5f;
        Vector3 goalPos = goal.transform.position;

        int groupSize = 0;
        for (int i = 0; i < gos.Length; i++)
        {
            GameObject go = gos[i].gameObject;
            if (go != gameObject)
            {
                float dist = Vector3.Distance(go.transform.position, transform.position);
                if (dist <= neighbourDistance)
                {
                    //Cohesion
                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < separationDist)
                    {
                        //Separation
                        vavoid += transform.position - go.transform.position;
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }

            }
        }
        if (groupSize > 0)
        {
            //Alignment
            vcentre = vcentre / (gos.Length-1) + (goalPos - transform.position);
            
            float goalDist = Vector3.Distance(goalPos, transform.position);
            FlockingManager.instance.avrSpeed = speed;
            Vector3 dir = (vcentre + vavoid) - transform.position;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                     Quaternion.LookRotation(dir),
                                     rotationSpeed * Time.deltaTime);
            }
        }
        vcenter = vcentre;
    }
    void OnDrawGizmos()
    {
        if (FlockingManager.instance.dispalyGridGizmos)
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawWireCube(vcenter, new Vector3(0.5f, 0.5f, 0.5f));
            //Gizmos.DrawCube(vcenter, new Vector3(0.5f, 0.5f, 0.5f));
            Gizmos.DrawCube(transform.position +transform.forward * 1f, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }
}
