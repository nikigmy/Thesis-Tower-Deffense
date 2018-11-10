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
    // Use this for initialization
    void Start()
    {
        var currentLevel = GameManager.instance.CurrentLevel;
        var centerOfField = Helpers.GetPositionForTile(currentLevel.MapSize.y / 2, currentLevel.MapSize.x / 2);
        centerOfField.y = transform.position.y;
        transform.position = centerOfField;

        var bottomOfField = Helpers.GetPositionForTile(currentLevel.MapSize.y, currentLevel.MapSize.x);
        minPositions = new Vector3(0, 3, bottomOfField.z - 15);
        maxPositions = new Vector3(bottomOfField.x, Def.Instance.MaxCameraHeight, -10);
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var vectorToMove = Vector3.zero;
        var mouseWheelInput = Input.GetAxis(mouseCroll);
        if (mouseWheelInput > 0f)
        {
            vectorToMove += transform.forward * Def.Instance.CameraZoomSpeed;
        }
        else if (mouseWheelInput < 0f) // backwards
        {
            vectorToMove -= transform.forward * Def.Instance.CameraZoomSpeed;
        }

        var mousePos = Input.mousePosition;
        if (mousePos.x >= Screen.width - panBorderThickness && transform.position.x < maxPositions.x)//right
        {
            vectorToMove += Vector3.right * Def.Instance.CameraMoveSpeed;
        }
        if (mousePos.x <= panBorderThickness && transform.position.x > minPositions.x)//left
        {
            vectorToMove -= Vector3.right * Def.Instance.CameraMoveSpeed;
        }
        if (mousePos.y >= Screen.height - panBorderThickness && transform.position.z < maxPositions.z)//up
        {
            vectorToMove += Vector3.forward * Def.Instance.CameraMoveSpeed;
        }
        if (mousePos.y <= panBorderThickness && transform.position.z > minPositions.z)//down
        {
            vectorToMove -= Vector3.forward * Def.Instance.CameraMoveSpeed;
        }

        vectorToMove = vectorToMove * Time.unscaledDeltaTime;

        vectorToMove.x = Mathf.Clamp(vectorToMove.x, -(transform.position.x - minPositions.x), maxPositions.x - transform.position.x);
        vectorToMove.y = Mathf.Clamp(vectorToMove.y, -(transform.position.y - minPositions.y), maxPositions.y - transform.position.y);
        vectorToMove.z = Mathf.Clamp(vectorToMove.z, -(transform.position.z - minPositions.z), maxPositions.z - transform.position.z);
        
        transform.Translate(vectorToMove, Space.World);
    }
}
