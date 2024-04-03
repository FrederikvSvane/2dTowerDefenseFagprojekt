using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
    private LineRenderer circleRenderer;
    public Tower towerScript;

    public int segments = 50;
    public float lineWidth = 0.1f;
    public Color lineColor = Color.red;
    // Start is called before the first frame update
    void Start()
    {
        towerScript = GetComponent<Tower>();
        circleRenderer = gameObject.AddComponent<LineRenderer>();
        circleRenderer.sortingLayerName = "Tower";
        circleRenderer.useWorldSpace = false;
        circleRenderer.startWidth = lineWidth;
        circleRenderer.endWidth = lineWidth;
        circleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        circleRenderer.startColor = lineColor;
        circleRenderer.endColor = lineColor;
        circleRenderer.positionCount = segments + 1;
    }

    // Update is called once per frame
    void Update()
    {
        float range = towerScript.getRange();
        float angleInc = 360f / segments;
        for(int i = 0; i <= segments; i++)
        {
            float angle = i * angleInc * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * range;
            float y = Mathf.Sin(angle) * range;
            circleRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
