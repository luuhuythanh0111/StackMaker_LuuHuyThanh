using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.01f;

    //Start is called before the first frame update
    private void LateUpdate()
    {
        Vector3 vel = Vector3.zero;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref vel, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
