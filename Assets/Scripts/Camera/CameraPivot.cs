using UnityEngine;
using System.Collections;

public class CameraPivot : MonoBehaviour
{
    public Transform VRcam;
    public GameObject sphere;
    void Start ()
    {
	
	}

    void LateUpdate()
    {
   
    }

    public void Recenter()
    {
        transform.localRotation = Quaternion.Inverse(VRcam.rotation);
    }
}
