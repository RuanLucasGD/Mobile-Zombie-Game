using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float turnSpeed;
   
    void Update()
    {
        transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
    }
}
