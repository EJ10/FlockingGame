using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAIController : MonoBehaviour
{
    public Flock boidFlock;
    public Animator boidAnimator;
    public Rigidbody boidRigid;
    public SphereCollider basicCollider;
    public BoxCollider diveCollider;

    public float forwardAcceleration = 0f;
    public float groundCheckDistance = 0.2f;
    public float forwardCheckDistance = 100f;
    public bool isGrounded = false;
    public bool soaring = false;
    public bool tryingToLand = false;

    private float flapTime = 3f;
    private float flapElapsedTime = 0;

    

    private bool isFlying = false;
    private bool isDie    = false;
    private bool isHead   = false;

    public bool IsDie { get { return isDie; } set { isDie = value; } }
    public bool IsHead { get { return isHead; } set { isHead = value; } }
    public bool IsFlying { get { return isFlying; } set { isFlying = value; } }

    void Start()
    {
        boidFlock    = GetComponent<Flock>();
        boidAnimator = GetComponent<Animator>();
        boidRigid    = GetComponent<Rigidbody>();
        basicCollider= GetComponent<SphereCollider>();
        diveCollider = GetComponent<BoxCollider>();

        flapElapsedTime = flapTime;
        Soar();
    }
    void FixedUpdate()
    {
        if (isDie)
        {
            boidAnimator.speed = 2f;
            return;
        }

        Move();
        if (boidAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Flap1" || boidAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Glide")
        {
            if (soaring)
            {
                soaring = false;
                boidAnimator.SetBool("IsSoaring", false);
                tryingToLand = true;
                forwardAcceleration = 1f;
            }
        }
        if (!soaring)
        {
            GroundedCheck();
        }
    }
    

    void GroundedCheck()
    {
        if (boidFlock.goal.transform.position.y <= groundCheckDistance)
        {
            isGrounded = true;
            if (transform.position.y <= groundCheckDistance)
            {
               if (tryingToLand)
               {
                    Landing();
                    tryingToLand = false;
               }
            }
        }
        else
        {
            isGrounded = false;
            Soar();
        }

    }
    
    public void Landing()
    {
        if (isFlying)
        {
            boidAnimator.SetTrigger("Landing");
            boidAnimator.applyRootMotion = true;
            boidRigid.useGravity = true;
            basicCollider.isTrigger = false;
            isFlying = false;
            soaring = false;
            boidAnimator.SetBool("IsSoaring", false);

            if (boidFlock.goal.GetComponent<GoalController>() != null)
                boidFlock.speed = boidFlock.goal.GetComponent<GoalController>().walkingSpeed;
        }
    }

    public void Soar()
    {
        if (!isFlying && !tryingToLand)
        {
            boidAnimator.SetBool("IsSoaring", true);
            boidAnimator.applyRootMotion = false;
            boidRigid.useGravity = false;
            basicCollider.isTrigger = true;
            isFlying = true;
            isGrounded = false;
            soaring = true;

            if (boidFlock.goal.GetComponent<GoalController>() != null)
                boidFlock.speed = boidFlock.goal.GetComponent<GoalController>().flyingSpeed;
        }
    }

    public void Attack()
    {
        boidAnimator.SetTrigger("Attack");
    }

    public void Eat()
    {
        boidAnimator.SetTrigger("Eat");
    }

    public void Hop()
    {
        boidAnimator.SetTrigger("Hop");
    }

    public void Move()
    {
        boidAnimator.SetFloat("Forward", forwardAcceleration);
        boidAnimator.SetFloat("Turn", GoalController.instance.InputVector.x);

        if (isFlying)
        {
            if (flapElapsedTime == 0f && Random.Range(0, 100) > 1)
            {
                flapElapsedTime = flapTime;
            }

            if (flapElapsedTime < flapTime)
            {
                flapElapsedTime += Time.deltaTime;
                boidAnimator.SetBool("IsFlapLoop", true);
                boidAnimator.SetBool("IsFly", false);
            }
            else
            {
                flapElapsedTime = 0;
                boidAnimator.SetBool("IsFlapLoop", false);
                boidAnimator.SetBool("IsFly", true);
            }
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Sphere")
        {
            Flock flock = GetComponent<Flock>();
            if (FlockingManager.instance.boids.Contains(flock))
            {
                isDie = true;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<BoxCollider>().enabled = true;
                UpdateMainBoids(flock);
                UpdateBoids(flock,other.transform);
            }
        }
    }
    void UpdateBoids(Flock boid,Transform col)
    {
        if (boid != null)
        {
            if (FlockingManager.instance.boids.Contains(boid))
            {
                //Destroy(boid.gameObject);
                boid.transform.SetParent(col);
                int idx = FlockingManager.instance.boids.FindIndex(b => b.id == boid.id);
                FlockingManager.instance.boids.RemoveAt(idx);
            }
        }        
    }
    void UpdateMainBoids(Flock boid)
    {
        if (boid != null)
        {
            if (FlockingManager.instance.MainBoids.Contains(boid))
            {
                bool akaHeader = boid.IsHeader;
                boid.IsHeader = false;
                int idx = FlockingManager.instance.MainBoids.FindIndex(b => b.id == boid.id);
                FlockingManager.instance.MainBoids.RemoveAt(idx);
                if (FlockingManager.instance.MainBoids.Count > 0 && akaHeader)
                    FlockingManager.instance.MainBoids[0].IsHeader = true;
            }
        }
    }

    float GetAnimationtime(string name)
    {
        float time = 0f;
        RuntimeAnimatorController ac = boidAnimator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == name)
            {
                time = ac.animationClips[i].length;
            }
        }
        return time;
    }
}
