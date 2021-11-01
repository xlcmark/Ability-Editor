using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float PanelSpeed=20;
    public float PanelBorderThickness=10;
    public int PanelLimitXmin;
    public int PanelLimitXmax;
    public int PanelLimitYmin;
    public int PanelLimitYmax;
    void Update()
    {
        Vector3 pos = transform.position;
        if (Input.mousePosition.x > Screen.width - PanelBorderThickness)
        {
            pos.x += PanelSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x < PanelBorderThickness)
        {
            pos.x -= PanelSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y > Screen.height - PanelBorderThickness)
        {
            pos.z += PanelSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <PanelBorderThickness)
        {
            pos.z -= PanelSpeed * Time.deltaTime;
        }
        pos.x = Mathf.Clamp(pos.x, PanelLimitXmin, PanelLimitXmax);
        pos.z = Mathf.Clamp(pos.z, PanelLimitYmin, PanelLimitYmax);

        transform.position = pos;

    }
    public void CameraMove(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, PanelLimitXmin, PanelLimitXmax);
        pos.z = Mathf.Clamp(pos.z, PanelLimitYmin, PanelLimitYmax);

        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
    }
}
