using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private const string mouseCroll = "Mouse ScrollWheel";
    private Camera cam;
    private const int panBorderThickness = 10;

    private Vector3 minPositions;
    private Vector3 maxPositions;
    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
        var currentLevel = GameManager.instance.CurrentLevel;
        var centerOfField = Helpers.GetPositionForTile(currentLevel.MapSize.y / 2, currentLevel.MapSize.x / 2);
        centerOfField.y = transform.position.y;
        transform.position = centerOfField;

        var bottomOfField = Helpers.GetPositionForTile(currentLevel.MapSize.y, currentLevel.MapSize.x);
        minPositions = new Vector3(0, 10, bottomOfField.z - 15);
        maxPositions = new Vector3(bottomOfField.x, Def.Instance.MaxCameraHeight, -10);
    }
	
	// Update is called once per frame
	void Update () {
        var mouseWheelInput = Input.GetAxis(mouseCroll);
        var vectorToMove = Vector3.zero;
        if (mouseWheelInput > 0f)
        {
            vectorToMove += transform.forward * Def.Instance.CameraZoomSpeed;
            if (transform.position.z >= maxPositions.z)
            {
                vectorToMove.z = 0;
            }
            if (transform.position.y <= minPositions.y)
            {
                vectorToMove.y = 0;
            }
        }
        else if (mouseWheelInput < 0f) // backwards
        {
            vectorToMove -= transform.forward * Def.Instance.CameraZoomSpeed;
            if (transform.position.z <= minPositions.z)
            {
                vectorToMove.z = 0;
            }
            if (transform.position.y >= maxPositions.y)
            {
                vectorToMove.y = 0;
            }
        }

        var mousePos = Input.mousePosition;
        if (mousePos.x >= Screen.width - panBorderThickness && transform.position.x < maxPositions.x)
        {
            vectorToMove += Vector3.right * Def.Instance.CameraMoveSpeed;
        }
        if (mousePos.x <= panBorderThickness && transform.position.x > minPositions.x)
        {
            vectorToMove -= Vector3.right * Def.Instance.CameraMoveSpeed;
        }
        if (mousePos.y >= Screen.height - panBorderThickness && transform.position.z < maxPositions.z)
        {
            vectorToMove += Vector3.forward * Def.Instance.CameraMoveSpeed;
        }
        if (mousePos.y <= panBorderThickness && transform.position.z > minPositions.z)
        {
            vectorToMove -= Vector3.forward * Def.Instance.CameraMoveSpeed;
        }

        transform.Translate(vectorToMove * Time.deltaTime, Space.World);
    }
}
