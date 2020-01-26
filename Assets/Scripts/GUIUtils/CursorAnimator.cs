using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class CursorAnimator
{
    [BoxGroup("Cursor frames")]
    [SerializeField]
    [Tooltip("Cursor frames and its durations")]
    [TableList(ShowPaging = true, ShowIndexLabels = true)]
    List<CursorFrameSettings> cursorFrames;

    private int currentFrameIndex = 0;
    private float timeSinceLatestFrameChange = 0f;

    public void Reset()
    {
        currentFrameIndex = 0;
        timeSinceLatestFrameChange = 0f;
    }

    public void Update(float delta, float widthMultiplier = 0, float heightMultiplier = 0)
    {
        timeSinceLatestFrameChange += delta;

        if (timeSinceLatestFrameChange > cursorFrames[currentFrameIndex].frameDuration)
        {
            currentFrameIndex++;
            if (currentFrameIndex >= cursorFrames.Count)
            {
                currentFrameIndex = 0;
            }

            timeSinceLatestFrameChange = 0;
        }

        Cursor.SetCursor(cursorFrames[currentFrameIndex].cursorTexture,
                new Vector2(cursorFrames[currentFrameIndex].cursorTexture.width * widthMultiplier,
                    cursorFrames[currentFrameIndex].cursorTexture.height * heightMultiplier),
                CursorMode.Auto);
    }
}

[Serializable]
public class CursorFrameSettings
{
    [TableColumnWidth(100, Resizable = false)]
    [PreviewField(Alignment = ObjectFieldAlignment.Center)]
    [Required]
    public Texture2D cursorTexture;
    [VerticalGroup("Frame duration in seconds")]
    public float frameDuration = 0.5f;
}
