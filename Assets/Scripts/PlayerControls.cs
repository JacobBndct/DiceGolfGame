using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    private Camera cam;
    private DieMovement dieMovement;

    [SerializeField] private float cameraRotateSpeed = 10f;
    
    private void Start()
    {
        cam = Camera.main;
        dieMovement = GetComponent<DieMovement>();
    }
    
    private void Update()
    {
        DoDieMovement();
        DoCameraRotation();
    }

    private void DoDieMovement()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            var camDirection = cam.transform.forward;
            dieMovement.Move(new Vector2(camDirection.x, camDirection.z));
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var camDirection = -cam.transform.right;
            dieMovement.Move(new Vector2(camDirection.x, camDirection.z));
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            var camDirection = -cam.transform.forward;
            dieMovement.Move(new Vector2(camDirection.x, camDirection.z));
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            var camDirection = cam.transform.right;
            dieMovement.Move(new Vector2(camDirection.x, camDirection.z));
        }
    }

    private void DoCameraRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            cameraPivot.transform.Rotate(0, cameraRotateSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            cameraPivot.transform.Rotate(0, -cameraRotateSpeed * Time.deltaTime, 0);
        }
    }
}
