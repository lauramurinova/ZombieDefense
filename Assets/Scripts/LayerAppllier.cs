using System.Linq;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class LayerApplier : MonoBehaviour
{
    public void GetRoomObjectAndApplyLayer()
    {
        MRUKRoom mrukComponent = FindObjectOfType<MRUKRoom>();
        GameObject mrukObject = mrukComponent.gameObject;

        ApplyLayer(mrukObject, "Wall");
    }

    private void ApplyLayer(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            if(child.gameObject.name.ToLower().Contains("window")) continue;
            ApplyLayer(child.gameObject, layerName);
        }
    }

    public void EnableWindows()
    {
        foreach (var renderer in FindObjectsOfType<MeshRenderer>())
        {
            if (renderer.name.ToLower().Contains("window_frame_effect"))
            {
                renderer.enabled = true;
            }
        }
    }
    
}