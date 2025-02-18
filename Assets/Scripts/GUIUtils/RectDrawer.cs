﻿using UnityEngine;
using UnityEngine.EventSystems;

public class RectDrawer
{
    SelectionManager selectionManager;

    public RectDrawer(SelectionManager selectionManager)
    {
        this.selectionManager = selectionManager;
    }

    static Texture2D whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if (whiteTexture == null)
            {
                whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();
            }

            return whiteTexture;
        }
    }

    bool isSelecting = false;
    Vector3 startMousePosition;

    public void MouseClickControl()
    {
        // If we press the left mouse button, save mouse location and begin selection
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isSelecting = true;
            startMousePosition = Input.mousePosition;
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            isSelecting = false;
    }

    public void DrawRect(float selectionBoxAccuracy, Camera camera)
    {
        if (isSelecting &&
            Vector3.Distance(camera.ScreenToViewportPoint(startMousePosition),
            camera.ScreenToViewportPoint(Input.mousePosition)) > selectionBoxAccuracy)
        {
            // Create a rect from both mouse positions
            var rect = GetScreenRect(startMousePosition, Input.mousePosition);
            // Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f)); // Fills the rectangle
            DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    public Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
}
