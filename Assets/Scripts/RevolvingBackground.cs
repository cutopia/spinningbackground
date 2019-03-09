using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// given a bunch of background pieces, make them into a parallax widget that can be scrolled horizontally
/// to simulate the player standing in place and spinning around to see everything.
/// z position of each piece is important as it determines its layer.
/// for best results, each layer should all total to the same width.
/// </summary>
public class RevolvingBackground : MonoBehaviour
{
    [SerializeField] List<GameObject> backgroundPieces;
    private float totalBackgroundWidth;
    private float startTime;
    private float totalTime = 1f;

    private float speed = 0f;
    private delegate void UpdateDelegate();
    private UpdateDelegate onUpdate;

    private Vector2Int screenDims;

    private Dictionary<int, List<GameObject>> piecesSeparatedByZ;

    private void Start()
    {
        Setup();
    }

    /// <summary>
    /// sets up the layers and sizes them to fit the view height
    /// This is also called when the resolution changes
    /// in that scenario, the user's current place is lost as everything
    /// resets its position for the new size.
    /// </summary>
    private void Setup()
    {
        Debug.Log("Setup");
        screenDims = new Vector2Int(Screen.width, Screen.height);
        piecesSeparatedByZ = new Dictionary<int, List<GameObject>>();
        foreach (GameObject item in backgroundPieces)
        {
            ScaleSpriteToFit(item);
            // sort the item into a layer based on its z index.
            int zIndex = (int)item.transform.position.z;
            if (!piecesSeparatedByZ.ContainsKey(zIndex))
            {
                piecesSeparatedByZ[zIndex] = new List<GameObject>();
            }
            piecesSeparatedByZ[zIndex].Add(item);
        }
        totalBackgroundWidth = GetTotalWidthOfBackgroundPieces();
        TileBackgroundPieces();
        PositionClickCollider();
    }

    private void Update()
    {
        if (screenDims.x != Screen.width || screenDims.y != Screen.height)
        {
            Setup();
        }
        onUpdate?.Invoke();
    }
    
    private void OffsetLayers(float xOffset)
    {
        foreach (var dictItem in piecesSeparatedByZ)
        {
            float adjustedOffset = (dictItem.Key > 0) ? xOffset / dictItem.Key : (dictItem.Key < 0) ? xOffset * -1 * dictItem.Key : xOffset;
            foreach (GameObject item in dictItem.Value)
            {

                float wrapAdjustment = 0f;
                float currentItemWidth = GetCalculatedWidth(item);
                // ultimately, only one item doesn't work quite right. You can't scroll left and see the one item layer.
                if (dictItem.Value.Count > 1)
                {
                    if (item.transform.position.x + xOffset > GetCamUnits().x)
                    {
                        // adjust position to be wrapped to other side
                        wrapAdjustment = totalBackgroundWidth * -1;
                    }
                    else if (item.transform.position.x + xOffset + currentItemWidth < 0)
                    {
                        wrapAdjustment += totalBackgroundWidth;
                    }
                }
                var pos = item.transform.position;
                pos.x += adjustedOffset + wrapAdjustment;
                item.transform.position = pos;
            }
        }
    }

    private void UpdateBackgroundPosition()
    {
        // move with the mouse while the button is down
        if (Input.GetMouseButton(0))
        {
            var movement = Input.GetAxis("Mouse X");
            if (!Mathf.Approximately(movement, 0))
            {
                OffsetLayers(Input.GetAxis("Mouse X"));
            }
        }
        else
        {
            if (!Mathf.Approximately(totalTime, 0))
            {
                // drift gradually to a stop once mouse is released
                float t = (Time.time - startTime) / totalTime;
                float offset = Mathf.Lerp(speed, 0f, t);
                if ((Time.time - startTime) > totalTime)
                {
                    onUpdate -= UpdateBackgroundPosition;
                }
                OffsetLayers(offset);
            }
        }
    }

    public void OnMouseDown()
    {
        if (onUpdate == null)
        {
            onUpdate = UpdateBackgroundPosition;
        }
    }

    public void OnMouseUp()
    {
        startTime = Time.time;
        speed = Input.GetAxis("Mouse X");
        totalTime = Mathf.Abs(speed) / 2;
    }
    
    /// <summary>
    /// initially tiles the background pieces into their starting positions.
    /// they are layered by z index.
    /// </summary>
    void TileBackgroundPieces()
    {
        foreach (var dictItem in piecesSeparatedByZ)
        {
            float totalWidthUsed = 0f;
            foreach (GameObject item in dictItem.Value)
            {
                float currentItemWidth = GetCalculatedWidth(item);
                var pos = item.transform.position;
                pos.x = totalWidthUsed;
                item.transform.position = pos;
                totalWidthUsed += currentItemWidth;
            }
        }
    }

    float GetTotalWidthOfBackgroundPieces()
    {
        float totalWidth = 0f;
        foreach (GameObject item in backgroundPieces)
        {
            if (Mathf.Approximately(item.transform.position.z, 0))
            {
                totalWidth += GetCalculatedWidth(item);
            }
        }
        Debug.Log("Total width is " + totalWidth);
        return totalWidth;
    }

    float GetCalculatedWidth(GameObject item)
    {
        RectTransform rect = item.GetComponent<RectTransform>();
        return rect.sizeDelta.x * rect.localScale.x;
    }

    void ScaleSpriteToFit(GameObject sprite)
    {
        float camUnits = Camera.main.orthographicSize * 2; // cam's view height in units
        RectTransform rectTrans = sprite.GetComponent<RectTransform>();
        float scaleFactor = camUnits / rectTrans.sizeDelta.y;
        rectTrans.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    /// <summary>
    /// Sizes and positions box collider to cover entire screen to capture user interactions
    /// </summary>
    void PositionClickCollider()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.size = GetCamUnits();
        box.offset = new Vector2(0.5f * box.size.x, 0.5f * box.size.y);
    }

    private Vector2 GetCamUnits()
    {
        float camUnitsHeight = Camera.main.orthographicSize * 2;
        float camUnitsWidth = (camUnitsHeight * Screen.width) / Screen.height;
        return new Vector2(camUnitsWidth, camUnitsHeight);
    }
}
