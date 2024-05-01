using Meta.XR.MRUtilityKit;
using Unity.VisualScripting;
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
            
            if (child.name.ToLower().Contains("floor"))
            {
                child.gameObject.layer = LayerMask.NameToLayer("Floor");
                if (child.name.ToLower().Contains("effect"))
                {
                    CreateBiggerFloorCollider(child);
                }
            }
        }
        
    }

    private static void CreateBiggerFloorCollider(Transform child)
    {
        var collider = child.AddComponent<BoxCollider>();
        collider.size *= 2f;
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