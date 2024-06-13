using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraSpeed = 10.0f;
    public float edgeBoundary = 10.0f; // Distance from edge to start moving camera
    public float zoomSpeed = 5.0f; // Speed of zooming in/out
    public float minZoom = 5.0f; // Minimum zoom level
    public float maxZoom = 20.0f; // Maximum zoom level

    public float minPanX = 0.0f;
    public float maxPanX = 10.0f;
    public float minPanY = 0.0f;
    public float maxPanY = 10.0f;

    void Update()
    {
        //MoveCameraBasedOnCursor();
        ZoomCameraBasedOnScroll();
        MoveCameraBasedOnArrowKeys();


        // For finding what key is pressed

        // if (Input.anyKeyDown)
        // {
        //     foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        //     {
        //         if (Input.GetKeyDown(kcode))
        //         {
        //             Debug.Log("Key pressed: " + kcode);
        //         }
        //     }
        // }
    }


    private void MoveCameraBasedOnArrowKeys()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);

        transform.position += movement * cameraSpeed * Time.deltaTime;
    }


    private void MoveCameraBasedOnCursor()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.mousePosition.x >= Screen.width - edgeBoundary && transform.position.x < maxPanX)
        {
            // Move camera right
            moveDirection.x += 1;
        }
        if (Input.mousePosition.x <= edgeBoundary && transform.position.x > minPanX)
        {
            // Move camera left
            moveDirection.x -= 1;
        }
        if (Input.mousePosition.y >= Screen.height - edgeBoundary && transform.position.y < maxPanY)
        {
            // Move camera up
            moveDirection.y += 1;
        }
        if (Input.mousePosition.y <= edgeBoundary && transform.position.y > minPanY)
        {
            // Move camera down
            moveDirection.y -= 1;
        }

        transform.position += moveDirection.normalized * cameraSpeed * Time.deltaTime;
    }

    private void ZoomCameraBasedOnScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera camera = GetComponent<Camera>();

        if (camera.orthographic)
        {
            camera.orthographicSize -= scroll * zoomSpeed;
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            camera.fieldOfView -= scroll * zoomSpeed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minZoom, maxZoom);
        }
    }
}