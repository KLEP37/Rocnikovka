using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Camera : MonoBehaviour
{
    float mouseX;
    float mouseY;
    [SerializeField] float sensitivity;
    [SerializeField] float baseSpeed;
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        speed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = baseSpeed * 10;
        }
        else
        {
            speed = baseSpeed;
        }
        transform.position += transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * speed;
        transform.position += transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        transform.rotation = transform.rotation * Quaternion.Euler(0, mouseX * sensitivity, 0);
        transform.rotation = transform.rotation * Quaternion.Euler(mouseY * sensitivity * -1, 0, 0);
        if (Input.GetKey(KeyCode.E))
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -sensitivity / 1000 * speed);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, sensitivity / 1000 * speed);
        }
    }
}
