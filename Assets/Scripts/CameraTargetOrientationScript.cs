/***************************************************************/
/********** Simple target orientation camera script. ***********/
/*** You can change parameters, such as rotation/zoom speed. ***/
/***************************************************************/

using UnityEngine;
using System.Collections;

public class CameraTargetOrientationScript : MonoBehaviour
{
    [Header("Mouse input:", order = 0)]
    [Space(-10, order = 1)]
    [Header("- Hold and drag RMB to rotate", order = 2)]
    [Space(-10, order = 3)]
    [Header("- Use mouse wheel to zoom in/out", order = 4)]
    [Space(5, order = 5)]

    [Header("Touch input:", order = 6)]
    [Space(-10, order = 7)]
    [Header("- Swipe left/right to rotate", order = 8)]
    [Space(-10, order = 9)]
    [Header("- Use multitouch to zoom in/out", order = 10)]
    [Space(15, order = 11)]

    public bool enableRotation = true;

    [Header("Choose target")]
    public Transform target;

    //Camera fields
    private float _smoothness = 0.5f;
    private Vector3 _cameraOffset;

    //Mouse control fields
    [Space(2)]
    [Header("Mouse Controls")]
    public float rotationSpeedMouse = 5;
    public float zoomSpeedMouse = 10;

    private float _zoomAmountMouse = 0;
    private float _maxToClampMouse = 10;

    //Touch control fields
    [Space(2)]
    [Header("Touch Controls")]
    public float rotationSpeedTouch = 5;
    public float zoomSpeedTouch = 0.5f;

    void Start()
    {
        _cameraOffset = transform.position - target.position;
        transform.LookAt(target);
    }

    void LateUpdate()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        // Rotating camera with RMB dragging on PC.
        if (enableRotation && (Input.GetMouseButton(1)))
        {

            Quaternion camAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeedMouse, Vector3.up);

            Vector3 newPos = target.position + _cameraOffset;
            _cameraOffset = camAngle * _cameraOffset;

            transform.position = Vector3.Slerp(transform.position, newPos, _smoothness);
            transform.LookAt(target);
        }

        #endif

        // Rotating camera with touch dragging on mobiles.
        #if UNITY_ANDROID || UNITY_IOS

        if (enableRotation && (Input.touchCount==1))
        {
            
            float touchDelta = Mathf.Clamp(Input.GetTouch(0).deltaPosition.x, -1.0f, 1.0f);
            Quaternion camAngle = Quaternion.AngleAxis(touchDelta * rotationSpeedTouch, Vector3.up);
            
            Vector3 newPos = target.position + _cameraOffset;
            _cameraOffset = camAngle * _cameraOffset;
        
            transform.position = Vector3.Slerp(transform.position, newPos, _smoothness);
            transform.LookAt(target);
        }

        #endif
       
        else
        {
        // Translating camera on PC with mouse wheel.
        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

            _zoomAmountMouse += Input.GetAxis("Mouse ScrollWheel");
            _zoomAmountMouse = Mathf.Clamp(_zoomAmountMouse, -_maxToClampMouse, _maxToClampMouse);

            var translate = Mathf.Min(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")), _maxToClampMouse - Mathf.Abs(_zoomAmountMouse));
            transform.Translate(0, 0, translate * zoomSpeedMouse * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")));

            _cameraOffset = transform.position - target.position;

        #endif

        // Changing FOV on mobiles with multitouch.
        #if UNITY_ANDROID || UNITY_IOS

            if (Input.touchCount == 2)
            {

                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                this.GetComponent<Camera>().fieldOfView += deltaMagnitudeDiff * zoomSpeedTouch;

                // Clamp the field of view to make sure it's between 0 and 180.
                this.GetComponent<Camera>().fieldOfView = Mathf.Clamp(this.GetComponent<Camera>().fieldOfView, 0.1f, 179.9f);

            }

        #endif


        }

    }



}