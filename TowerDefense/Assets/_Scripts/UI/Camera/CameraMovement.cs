using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float _cameraSpeed = 10.0f;
    public float _edgeBoundary = 10.0f; // Distance from edge to start moving camera
    public float _zoomSpeed = 5.0f; // Speed of zooming in/out
    public float _minZoom = 5.0f; // Minimum zoom level
    public float _maxZoom = 20.0f; // Maximum zoom level
    public float _minPanX = 0.0f;
    public float _maxPanX = 10.0f;
    public float _minPanY = 0.0f;
    public float _maxPanY = 10.0f;

    void Update()
    {
        //MoveCameraBasedOnCursor();
        ZoomCameraBasedOnScroll();
        MoveCameraBasedOnArrowKeys();
    }


    private void MoveCameraBasedOnArrowKeys()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);

        transform.position += movement * _cameraSpeed * Time.deltaTime;
    }


    private void MoveCameraBasedOnCursor()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.mousePosition.x >= Screen.width - _edgeBoundary && transform.position.x < _maxPanX)
        {
            // Move camera right
            moveDirection.x += 1;
        }
        if (Input.mousePosition.x <= _edgeBoundary && transform.position.x > _minPanX)
        {
            // Move camera left
            moveDirection.x -= 1;
        }
        if (Input.mousePosition.y >= Screen.height - _edgeBoundary && transform.position.y < _maxPanY)
        {
            // Move camera up
            moveDirection.y += 1;
        }
        if (Input.mousePosition.y <= _edgeBoundary && transform.position.y > _minPanY)
        {
            // Move camera down
            moveDirection.y -= 1;
        }

        transform.position += moveDirection.normalized * _cameraSpeed * Time.deltaTime;
    }

    private void ZoomCameraBasedOnScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera camera = GetComponent<Camera>();

        if (camera.orthographic)
        {
            camera.orthographicSize -= scroll * _zoomSpeed;
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, _minZoom, _maxZoom);
        }
        else
        {
            camera.fieldOfView -= scroll * _zoomSpeed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, _minZoom, _maxZoom);
        }
    }
}