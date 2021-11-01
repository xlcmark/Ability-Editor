using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingBillboard : MonoBehaviour
{
    Vector3 vLook, vRight, vUp;
    void Update()
    {
        //create temporary billboard look vector
        vLook = transform.position - Camera.main.transform.position;
        vLook.Normalize();

        //create billboard right vector
        float visible = Mathf.Abs(Vector3.Dot(transform.forward, vLook));
        if (visible >= 1)
        {
            // look vector is parallel to axis
            vLook = transform.forward;
        }
        else
        {
            // create and normalize right vector
            vRight = Vector3.Cross(transform.forward, vLook);
            vRight.Normalize();

            // create final billboard look vector
            vLook = Vector3.Cross(vRight, transform.forward);

            // create billboard up vector
            vUp = Vector3.Cross(vLook, vRight);

            // axial billboard with look rotation axis aligned
            transform.rotation = Quaternion.LookRotation(vLook, vUp);
        }
    }
}
