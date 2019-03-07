using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// given a bunch of background pieces, make them into a parallax widget that can be scrolled horizontally
/// to simulate the player standing in place and spinning around to see everything.
/// z position of each piece is important as it determines its layer.
/// each layer will be tiled into place separately, so for best results, each layer should all total to the same width.
/// </summary>
public class RevolvingBackground : MonoBehaviour
{
    [SerializeField] List<GameObject> backgroundPieces;
    private float totalBackgroundWidth;
    private float startingPosition = 0f;
    private float currentPosition = 0f;
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

    private void Setup()
    {
        screenDims = new Vector2Int(Screen.width, Screen.height);
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
        TileBackgroundPieces(0);
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

    private void UpdateBackgroundPosition()
    {
        // move with the mouse while the button is down
        if (Input.GetMouseButton(0))
        {
            TileBackgroundPieces(currentPosition + Input.GetAxis("Mouse X"));
        }
        else
        {
            // drift gradually to a stop once mouse is released
            float t = (Time.time - startTime) / totalTime;
            float offset = Mathf.Lerp(startingPosition, startingPosition + (speed * totalTime), t);
            if ((Time.time - startTime) > totalTime)
            {
                onUpdate -= UpdateBackgroundPosition;
            }
            TileBackgroundPieces(offset);
        }
    }

    public void OnMouseDown()
    {
        startingPosition = currentPosition;
        if (onUpdate == null)
        {
            onUpdate = UpdateBackgroundPosition;
        }
    }

    public void OnMouseUp()
    {
        startTime = Time.time;
        speed = Input.GetAxis("Mouse X");
        totalTime = startTime + Mathf.Abs(speed);
        Debug.Log("Speed: " + speed + " TotalTime: " + totalTime);
        startingPosition = currentPosition;
    }
    
    void TileBackgroundPieces(float xOffset)
    {
        currentPosition = xOffset;
        float startingOffset = currentPosition % totalBackgroundWidth;
        float totalWidthUsed = 0f;
        foreach (GameObject item in backgroundPieces)
        {
            float wrapAdjustment = 0f;
            float currentItemWidth = GetCalculatedWidth(item);
            if (startingOffset + totalWidthUsed > GetCamUnits().x)
            {
                // adjust position to be wrapped to other side of startingOffset
                wrapAdjustment = totalBackgroundWidth * -1;
            }
            else if (startingOffset + totalWidthUsed + currentItemWidth < 0)
            {
                wrapAdjustment += totalBackgroundWidth;
            }
            var pos = item.transform.position;
            pos.x = startingOffset + totalWidthUsed + wrapAdjustment;
            item.transform.position = pos;
            totalWidthUsed += currentItemWidth;
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
