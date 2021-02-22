using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    /* To turn off the visibility 
     * of the float inputs in the editor 
     * just delete [SerializeField] 
     * from the beginning of each float line */

    public Transform cameraTransform;

    [SerializeField] private float _camSpeed = 1f; //Speed of the camera
    [SerializeField] private float _camSpeedFast = 5f; //Speed of the camera while holding "Fast camera movement button"

    [SerializeField] private float _camMovementSpeed = 1f;
    [SerializeField] private float _camSmoothness = 10f;

    [SerializeField] private float _camRotationAmount = 1f;
    [SerializeField] private float _camRotationMouseSpeed = .5f;
    [SerializeField] private float _camBorderMovement = 5f;

    [SerializeField] private float _maxCamZoom = 30f;
    [SerializeField] private float _minCamZoom = 100f;

    [SerializeField] private float _minZCamMovement = 100f;
    [SerializeField] private float _maxZCamMovement = 900f;
    [SerializeField] private float _minXCamMovement = 100f;
    [SerializeField] private float _maxXCamMovement = 900f;

    [SerializeField] public float zoomAmount;

    [SerializeField] private bool cursorVisible = true;

    public Vector3 zoomVector;
    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    //MouseMovement
    public float rotateStartPosition;
    public float rotateCurrentPosition;
    bool isRotating;

    Vector2 pos1;
    Vector2 pos2;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;

        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        zoomVector = new Vector3(0, zoomAmount, -zoomAmount);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovementInput();
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        //Scroll zooming
        if (Mouse.current.scroll.ReadValue().y != 0)
        {
            newZoom -= Mouse.current.scroll.ReadValue().y * zoomVector;

            if (newZoom.y <= _maxCamZoom) //Max zoom limit
            {
                newZoom = new Vector3(0, _maxCamZoom, -_maxCamZoom);
            } else if (newZoom.y >= _minCamZoom) //Min zoom limit
            {
                newZoom = new Vector3(0, _minCamZoom, -_minCamZoom);
            }

        }

        //Camera rotating on mouse scroll button hold

        
        if (!Mouse.current.middleButton.isPressed)
        {
            isRotating = false;
            Cursor.visible = cursorVisible;
        }
        else
        {
            Cursor.visible = false;
            newRotation *= Quaternion.Euler(Vector3.up * Mouse.current.delta.x.ReadValue() * _camRotationMouseSpeed);
        }

    }

    void HandleMovementInput()

        //Fast camera movement
    {
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            _camMovementSpeed = _camSpeedFast;
        }
        else
        {
            _camMovementSpeed = _camSpeed;
        }

        if (Keyboard.current.wKey.isPressed) // || Mouse.current.position.y.ReadValue() >= Screen.height - _camBorderMovement)
        {
            newPosition += (transform.forward * _camMovementSpeed);
        }

        if (Keyboard.current.sKey.isPressed) // || Mouse.current.position.y.ReadValue() <= _camBorderMovement)
        {
            newPosition += (transform.forward * -_camMovementSpeed);
        }

        if (Keyboard.current.dKey.isPressed) // || Mouse.current.position.x.ReadValue() >= Screen.width - _camBorderMovement)
        {
            newPosition += (transform.right * _camMovementSpeed);
        }

        if (Keyboard.current.aKey.isPressed) // || Mouse.current.position.x.ReadValue() <= _camBorderMovement)
        {
            newPosition += (transform.right * -_camMovementSpeed);
        }

        //Keyboard setup for camera rotate
        if (Keyboard.current.qKey.isPressed)
        {
            newRotation *= Quaternion.Euler(Vector3.up * _camRotationAmount);
        }

        if (Keyboard.current.eKey.isPressed)
        {
            newRotation *= Quaternion.Euler(Vector3.up * -_camRotationAmount);
        }

        //Keyboard setup for camera zoom
        if (Keyboard.current.rKey.isPressed)
        {
            newZoom += zoomVector;

            //Max zoom limit
            if (newZoom.y <= 30)
            {
                newZoom = new Vector3(0, 30, -30);

            }
            
        }

        //Min zoom limit
        if (Keyboard.current.fKey.isPressed)
        {
            newZoom -= zoomVector;
            if (newZoom.y >= 120)
            {
                newZoom = new Vector3(0, 120, -120);
            }
        }

        //Setting Borders
        if (newPosition.x < _minXCamMovement)
        {
            newPosition = new Vector3(_minXCamMovement, transform.position.y, transform.position.z);

        } else if(newPosition.x > _maxXCamMovement)
        {
            newPosition = new Vector3(_maxXCamMovement, transform.position.y, transform.position.z);
        }

        if (newPosition.z < _minZCamMovement)
        {
            newPosition = new Vector3(transform.position.x, transform.position.y, _minZCamMovement);

        }
        else if (newPosition.z > _maxZCamMovement)
        {
            newPosition = new Vector3(transform.position.x, transform.position.y, _maxZCamMovement);
        }


        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * _camSmoothness);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * _camSmoothness);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * _camSmoothness);
    }

   
}
