using UnityEngine;

/// <summary>
/// Brutally restores consistency and sanity to the unity orthographic camera.
/// </summary>
public class CameraConformity : MonoBehaviour
{
    [SerializeField] public int defaultPixelsPerUnit = 100;
    private Vector2Int screenDims;

    // Start is called before the first frame update
    void Start()
    {
        SizeAndAlignCamera(defaultPixelsPerUnit);
    }

    private void Update()
    {
        if (screenDims.x != Screen.width || screenDims.y != Screen.height)
        {
            SizeAndAlignCamera(defaultPixelsPerUnit);
        }
    }

    public void SizeAndAlignCamera(int pDesiredPixelsPerUnit)
    {
        screenDims = new Vector2Int(Screen.width, Screen.height);
        float camSize = Screen.height * 0.5f / pDesiredPixelsPerUnit;
        GetComponent<Camera>().orthographicSize = camSize;
        Vector3 pos = transform.position;
        pos.y = camSize;
        pos.x = Screen.width * camSize / Screen.height;
        transform.SetPositionAndRotation(pos, Quaternion.identity);
    }
}
