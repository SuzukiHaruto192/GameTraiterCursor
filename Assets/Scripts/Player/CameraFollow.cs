using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);                  //Độ dời camera để đảm bảo không bị chồng lên object
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;
    [SerializeField] private bool limitCamera = true;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        if (limitCamera)
        {
            float clampX = Mathf.Clamp(smoothedPosition.x, minPosition.x, maxPosition.x);
            float clampY = Mathf.Clamp(smoothedPosition.y, minPosition.y, maxPosition.y);
            smoothedPosition = new Vector3(clampX, clampY, smoothedPosition.z);
        }

        transform.position = smoothedPosition;
    }
}
