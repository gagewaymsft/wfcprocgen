using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;

    [Range(0f, 0.2f)]
    public float smoothing;

    public Vector2 maxPosition;
    public Vector2 minPosition;
    public bool ClampCamera;

    // Update is called once per frame
    void FixedUpdate()
    {

        if (transform.position != target.position)
        {
            Vector3 targetPosition = new(target.position.x, target.position.y, transform.position.z);

            if (ClampCamera)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }

    }
}
