using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 cameraPosTemp = Camera.main.transform.position;
        cameraPosTemp.x = Mathf.Lerp(cameraPosTemp.x, cameraPos.transform.position.x, .1f);
        cameraPosTemp.z = Mathf.Lerp(cameraPosTemp.z, cameraPos.transform.position.z, .1f);
        cameraPosTemp.y = Mathf.Lerp(cameraPosTemp.y, cameraPos.transform.position.y, .04f);

        Camera.main.transform.position = cameraPosTemp;

        Vector3 cameraRotTemp = Camera.main.transform.eulerAngles;

        cameraRotTemp = Camera.main.transform.eulerAngles;

        cameraRotTemp.y = Mathf.LerpAngle(cameraRotTemp.y, cameraPos.eulerAngles.y, .2f);
        cameraRotTemp.x = Mathf.LerpAngle(cameraRotTemp.x, cameraPos.eulerAngles.x, .01f);

        //Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraPos.transform.rotation, .1f);
        //Camera.main.transform.localRotation = Quaternion.Euler(Camera.main.transform.localRotation.eulerAngles.x, Camera.main.transform.localRotation.eulerAngles.y, 0);
        Camera.main.transform.rotation = Quaternion.Euler(cameraRotTemp);
    }
}
