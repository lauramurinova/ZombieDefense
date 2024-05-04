using Meta.XR.MRUtilityKit;
using UnityEngine;

public class DestroyablePlane : MonoBehaviour
{
    [SerializeField] private string _objectName;
    [SerializeField] private GameObject _planePrefab;

    public void Initialize()
    {
        foreach (Transform plane in FindObjectsOfType<Transform>())
        {
            if (plane.name.ToLower().Contains(_objectName))
            {
                var destroyablePlane = Instantiate(_planePrefab, plane);
                var mrukAnchor = plane.parent.GetComponent<MRUKAnchor>();
                float distanceX = Mathf.Abs(mrukAnchor.PlaneBoundary2D[1].x - mrukAnchor.PlaneBoundary2D[0].x);
                float distanceY = Mathf.Abs(mrukAnchor.PlaneBoundary2D[3].y - mrukAnchor.PlaneBoundary2D[0].y);

                // destroyablePlane.transform.localScale = new Vector3(distanceX, distanceY, 1f);
            }
        }
    }
}
