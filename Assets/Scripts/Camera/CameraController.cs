using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public Transform lookatPosition;
    public Transform cameraPosition;
    public Transform target;
    private bool smooth = true;
    private float smoothSpeed = 0.06f;
    private float smoothLookSpeed = 0.1f;
    private Vector3 offset = Vector3.zero;

    private Vector3 _cameraLocalEulerAngles = Vector3.zero;
    private float moveAngle = 0f;

    public float positionY = 3;
    void Start()
    {
        offset = target.position - cameraPosition.position;
        transform.position = target.transform.position + offset;
        lookatPosition.position = target.position + (target.forward * 3f);
        transform.LookAt(lookatPosition.position);
    }
    void LateUpdate()
    {
        UpdateCamera();
    }
    void UpdateCamera()
    {
        //Vector3 desirePosition = target.transform.position + offset;
        //UpdateCameraOffset();
        Vector3 desirePosition = cameraPosition.position;
        if (desirePosition.y <= 3)
            desirePosition = new Vector3(desirePosition.x,3f, desirePosition.z);
        transform.position = smooth ? Vector3.Lerp(transform.position, desirePosition, smoothSpeed) : desirePosition;

        Vector3 desireLookAt = target.position + (target.forward * 3f);
        lookatPosition.position = smooth ? Vector3.Lerp(lookatPosition.position, desireLookAt, smoothLookSpeed) : desireLookAt;
        transform.LookAt(lookatPosition.position);

    }
    //void CameraMove(float dirNormal)
    //{
    //    Vector3 originPos = new Vector3(target.position.x, offset.y, target.position.z);
    //    float radius = Vector3.Distance(target.transform.position + offset, originPos);
    //    float fX = target.transform.position.x + offset.x;
    //    float fY = target.transform.position.z + offset.z;
    //    float angleIncrease = 100f;

    //    moveAngle += (angleIncrease * Time.deltaTime) * dirNormal;

    //    float radian = Mathf.PI / 180f * moveAngle;

    //    fX = radius * Mathf.Sin(radian) + originPos.x;
    //    fY = radius * Mathf.Cos(radian) + originPos.z;

    //    offset = new Vector3(fX - target.transform.position.x, offset.y, fY - target.transform.position.z);
    //}

    //void UpdateCameraOffset()
    //{
    //    int count = FlockingManager.instance.boids.Count;
    //    Dictionary<int, float> targetDist = new Dictionary<int, float>();
    //    for (int i = 0; i < count; i++)
    //    {
    //        float dist = Vector3.Distance(target.transform.position, FlockingManager.instance.boids[i].transform.position);
    //        targetDist.Add(i, dist);
    //    }
    //    List<KeyValuePair<int, float>> distList = new List<KeyValuePair<int, float>>(targetDist);
    //    distList.Sort((first, next) => { return first.Value.CompareTo(next.Value); });
    //}
}
