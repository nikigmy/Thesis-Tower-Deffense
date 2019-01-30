using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    private const string mouseCroll = "Mouse ScrollWheel";
    private const int panBorderThickness = 10;

    private Vector3 minPositions;
    private Vector3 maxPositions;
    bool debugging = false;
    bool init;
    // Use this for initialization
    void Awake()
    {
        init = false;
        GameManager.instance.LevelLoaded.AddListener(LevelLoaded);
    }

    private void LevelLoaded()
    {
        InitCamera();
    }

    private void InitCamera()
    {
        var mapSize = GameManager.instance.MapGenerator.RealMapSize;
        var centerOfField = Helpers.GetPositionForTile(mapSize.y / 2, mapSize.x / 2);
        centerOfField.y = transform.position.y;
        transform.position = centerOfField;

        var bottomOfField = Helpers.GetPositionForTile(mapSize.y, mapSize.x - 1);
        minPositions = new Vector3(7, 11, bottomOfField.z - 4);
        maxPositions = new Vector3(bottomOfField.x - 7, 30, -15);
        init = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!init || EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var vectorToMove = Vector3.zero;
        var mouseWheelInput = Input.GetAxis(mouseCroll);
        if (mouseWheelInput > 0f)
        {
            vectorToMove += transform.forward * Def.Instance.Settings.CameraZoomSpeed;
        }
        else if (mouseWheelInput < 0f) // backwards
        {
            vectorToMove -= transform.forward * Def.Instance.Settings.CameraZoomSpeed;
        }

        var mousePos = Input.mousePosition;
        if (mousePos.x >= Screen.width - panBorderThickness && transform.position.x < maxPositions.x && (mousePos.x < Screen.width || !debugging))//right
        {
            vectorToMove += Vector3.right * Def.Instance.Settings.CameraMoveSpeed;
        }
        if (mousePos.x <= panBorderThickness && transform.position.x > minPositions.x && (mousePos.x > 0 || !debugging))//left
        {
            vectorToMove -= Vector3.right * Def.Instance.Settings.CameraMoveSpeed;
        }
        if (mousePos.y >= Screen.height - panBorderThickness && transform.position.z < maxPositions.z && (mousePos.y < Screen.height || !debugging))//up
        {
            vectorToMove += Vector3.forward * Def.Instance.Settings.CameraMoveSpeed;
        }
        if (mousePos.y <= panBorderThickness && transform.position.z > minPositions.z && (mousePos.y > 0 || !debugging))//down
        {
            vectorToMove -= Vector3.forward * Def.Instance.Settings.CameraMoveSpeed;
        }

        vectorToMove = vectorToMove * Time.unscaledDeltaTime;

        vectorToMove.x = Mathf.Clamp(vectorToMove.x, -(transform.position.x - minPositions.x), maxPositions.x - transform.position.x);
        vectorToMove.y = Mathf.Clamp(vectorToMove.y, -(transform.position.y - minPositions.y), maxPositions.y - transform.position.y);
        vectorToMove.z = Mathf.Clamp(vectorToMove.z, -(transform.position.z - minPositions.z), maxPositions.z - transform.position.z);
        var rotation = ((transform.position.y + vectorToMove.y - 6) / 19 * 30) + 40;//the gigher the camera is the more its rotated(40 - 70)) ((current position above 6(min) / movementRange) * rotationRange) + minRotation
        transform.Rotate(new Vector3(rotation - transform.rotation.eulerAngles.x, 0, 0), Space.World);
        transform.Translate(vectorToMove, Space.World);
    }
}
