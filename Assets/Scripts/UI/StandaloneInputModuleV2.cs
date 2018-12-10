using UnityEngine;
using UnityEngine.EventSystems;

public class StandaloneInputModuleV2 : StandaloneInputModule
{
    public GameObject GameObjectUnderPointer()
    {
        var lastPointer = GetLastPointerEventData(0);
        if (lastPointer != null)
            return lastPointer.pointerCurrentRaycast.gameObject;
        return null;
    }
}
