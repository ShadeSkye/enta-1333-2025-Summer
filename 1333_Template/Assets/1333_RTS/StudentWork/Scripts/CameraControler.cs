using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public float PanSpeed;
    public float PanBorderTickness;
    public float MinHeight;
    public float MaxHeight;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - PanBorderTickness)
        {
            pos.z += PanSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.mousePosition.y <= PanBorderTickness)
        {
            pos.z -= PanSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - PanBorderTickness)
        {
            pos.x += PanSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.mousePosition.x <= PanBorderTickness)
        {
            pos.x -= PanSpeed * Time.deltaTime;
        }

        if (Input.GetKey("e"))
        {
            pos.y += PanSpeed * Time.deltaTime;
        }

        if (Input.GetKey("q"))
        {
            pos.y -= PanSpeed * Time.deltaTime;
        }

        transform.position = pos;
    }
}
