using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;

public class GoalController : MonoSingleton<GoalController>
{
    float moveAngle = 0f;
    public float speed = 3f;
    public float flyingSpeed = 5f;
    public float walkingSpeed = 0f;
    public float groundCheckDistance = 0.2f;
    public Vector2 InputVector { get { return inputVector; } }
    void Start ()
    {
        
	}
	void Update ()
    {
        UpdateMovement();
        UpdateRotation();
    }
    
    void UpdateMovement()
    {
        transform.position = transform.position + (transform.forward * (speed * Time.deltaTime));

        Vector3 desiredMoveY = Camera.main.transform.up * inputVector.y;
        transform.position = transform.position + (desiredMoveY.normalized* (speed * Time.deltaTime));
        if (transform.position.y <= 0)
        {
            speed = walkingSpeed;
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }

        if(groundCheckDistance < transform.position.y)
        {
            speed = flyingSpeed;
        }
       

    }
    void UpdateRotation()
    {
        //Rotation
            Vector3 desiredMove = Camera.main.transform.right * inputVector.x;
        if (desiredMove != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(desiredMove.x, 0f, desiredMove.z)), 0.05f);
        }
    }
    void CircleMovePath(float dirNormal)
    {
        Vector3 originPos = new Vector3(0f, 3, 0f);
        float radius = 10f;// Vector3.Distance(transform.position, originPos);
        float fX = transform.position.x;
        float fY = transform.position.z;
        float angleIncrease = FlockingManager.instance.avrSpeed / 2f;

        moveAngle += ((angleIncrease * 10f) * Time.deltaTime) * dirNormal;

        float radian = Mathf.PI / 180f * moveAngle;

        fX = radius * Mathf.Sin(radian) + originPos.x;
        fY = radius * Mathf.Cos(radian) + originPos.z;

        transform.position = new Vector3(fX, originPos.y, fY);
    }
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Flock>())
        {
            BoidGroup otherboidGroup = other.GetComponentInParent<BoidGroup>();
            if (otherboidGroup)
            {
                if (!otherboidGroup.isMainGroup)
                {
                    Flock otherFlock = other.GetComponent<Flock>();
                    otherFlock.transform.SetParent(FlockingManager.instance.MainBoids[0].transform.parent);
                    otherFlock.goal = gameObject;
                    otherFlock.speed = speed;
                }
            }
        }
    }


    private static Vector2 inputVector
    {
        get
        {
            float x = Mathf.Round(CnInputManager.GetAxis("Horizontal"));
            float y = Mathf.Round(CnInputManager.GetAxis("Vertical"));

            return new Vector2(x, y);
        }
    }
}
