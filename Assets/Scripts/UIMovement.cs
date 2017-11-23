using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMovement : MonoBehaviour {

    public Transform uiElement;
    public Transform cameraTransform;

    private float distanceFromCamera;

    private void Start()
    {
        distanceFromCamera = Vector3.Distance(uiElement.position, cameraTransform.position);
    }

    private void Update()
    {
        uiElement.rotation = Quaternion.LookRotation(uiElement.position - cameraTransform.position);
    }
}
