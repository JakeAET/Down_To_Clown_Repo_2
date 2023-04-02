using UnityEngine;

[System.Serializable]
public class zoomPreset
{
    public string presetName;
    public Vector3 endPosition;
    public float endZoom;
    public bool lockCam;
    public float duration;
    public bool following;
    public string targetTag;
}
