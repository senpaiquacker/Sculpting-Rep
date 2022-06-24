using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0.1f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        var x = Vector3.right * Input.GetAxis("Horizontal");
        var z = Vector3.forward * Input.GetAxis("Vertical");
        var y = Vector3.up * Input.GetAxis("Flying");
        var mov = (x + y + z).normalized * speed * Time.deltaTime;
        mov = Quaternion.Euler(Vector3.up * transform.rotation.eulerAngles.y) * mov;
        transform.position += mov;
        var mouseX = Vector3.up * Input.GetAxis("Mouse X");
        var mouseY = Vector3.right * Input.GetAxis("Mouse Y") * -1;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + mouseX + mouseY);
    }
}
