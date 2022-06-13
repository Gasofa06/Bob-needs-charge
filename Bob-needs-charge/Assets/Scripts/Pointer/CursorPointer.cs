using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorPointer : MonoBehaviour
{
    public float radius = 2f;
    public float speed = 1f;
    public Transform pointer;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public Camera cm;
    public GameObject gun;

    private void Start()
    {
        Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnLookToMouse(InputValue value)
    {
        // We're getting a Vector2, whereas we will need a Vector3
        // Get a z value based on camera, and include it in a Vector3
        Vector2 mousePosition = value.Get<Vector2>();

        Vector3 mouseViewportPosition = Camera.main.ViewportToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        pointer.localPosition = Camera.main.ScreenToWorldPoint(mouseViewportPosition);

        /*
            Debug.Log("MousePos: " + mouseViewportPosition);

            Vector3 positionToLookAt;
            positionToLookAt.x = mouseViewportPosition.x;
            positionToLookAt.y = 0.0f;
            //positionToLookAt.z = currentMovement.z;
            positionToLookAt.z = mouseViewportPosition.z;

            Quaternion currentRotation = transform.rotation;

            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt - transform.position);
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);*/
    }
    private void OnLookToGamepad(InputValue value)
    {
        var delta = value.Get<Vector2>();
        var input = new Vector3(delta.x, delta.y, 0);

        if (input.normalized != new Vector3(0, 0, 0))
        {
            // POINTER
            pointer.localPosition = Vector2.ClampMagnitude(input.normalized * radius, radius);

            // GUN
            gun.transform.right = input;
        }
    }

    private void OnShoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

}
