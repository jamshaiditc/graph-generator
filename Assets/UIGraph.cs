using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGraph : MonoBehaviour
{
    public RectTransform graphContainer;
    public List<Vector2> dataPoints = new List<Vector2>();

    public float xMin, xMax, yMin, yMax;
    public Color pointColor = Color.white;
    public Color lineColor = Color.white;
    public Color xAxisColor = Color.white;
    public Color yAxisColor = Color.white;
    public Color textColor = Color.white;

    private void Start()
    {
        ShowGraph();
    }

    private void ShowGraph()
    {
        Debug.Log("ShowGraph called");

        // Create X-axis line
        CreateLine(new Vector2(0f, 0f), new Vector2(graphContainer.sizeDelta.x, 0f), xAxisColor);

        // Create Y-axis line
        CreateLine(new Vector2(0f, 0f), new Vector2(0f, graphContainer.sizeDelta.y), yAxisColor);

        // Add X-axis text and markings
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float xPosition = Mathf.InverseLerp(xMin, xMax, dataPoints[i].x) * graphContainer.sizeDelta.x;
            CreateText(new Vector2(xPosition, -20f), dataPoints[i].x.ToString("F0"));

            // Add X-axis markings
            CreateLine(new Vector2(xPosition, 0f), new Vector2(xPosition, -10f), xAxisColor);
        }

        // Add Y-axis text and markings
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float yPosition = Mathf.InverseLerp(yMin, yMax, dataPoints[i].y) * graphContainer.sizeDelta.y;
            CreateText(new Vector2(-20f, yPosition), dataPoints[i].y.ToString("F0"));

            // Add Y-axis markings
            CreateLine(new Vector2(0f, yPosition), new Vector2(-10f, yPosition), yAxisColor);
        }

        for (int i = 0; i < dataPoints.Count; i++)
        {
            float xPosition = Mathf.InverseLerp(xMin, xMax, dataPoints[i].x) * graphContainer.sizeDelta.x;
            float yPosition = Mathf.InverseLerp(yMin, yMax, dataPoints[i].y) * graphContainer.sizeDelta.y;

            if (i > 0)
            {
                float prevXPosition = Mathf.InverseLerp(xMin, xMax, dataPoints[i - 1].x) * graphContainer.sizeDelta.x;
                float prevYPosition = Mathf.InverseLerp(yMin, yMax, dataPoints[i - 1].y) * graphContainer.sizeDelta.y;
                CreateLine(new Vector2(prevXPosition, prevYPosition), new Vector2(xPosition, yPosition), lineColor);
            }

            CreatePoint(new Vector2(xPosition, yPosition));

            // Add data point text
            CreateText(new Vector2(xPosition, yPosition + 40f), "(" + dataPoints[i].x.ToString("F0") + ", " + dataPoints[i].y.ToString("F0") + ")");
        }
    }

    private void CreatePoint(Vector2 anchoredPosition)
    {
        Debug.Log("CreatePoint called with anchoredPosition: " + anchoredPosition);
        GameObject point = new GameObject("Point");
        point.transform.SetParent(graphContainer, false);
        RectTransform pointRectTransform = point.AddComponent<RectTransform>();
        pointRectTransform.anchoredPosition = anchoredPosition;
        pointRectTransform.sizeDelta = new Vector2(15f, 15f);

        Image pointImage = point.AddComponent<Image>();
        pointImage.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
        pointImage.color = pointColor;

        // Ensure the point is displayed on top of the lines
        point.transform.SetAsLastSibling();
    }

    private void CreateLine(Vector2 startAnchoredPosition, Vector2 endAnchoredPosition, Color color)
    {
        GameObject line = new GameObject("Line", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        RectTransform lineRectTransform = line.GetComponent<RectTransform>();
        Vector2 direction = (endAnchoredPosition - startAnchoredPosition).normalized;
        float distance = Vector2.Distance(startAnchoredPosition, endAnchoredPosition);
        lineRectTransform.anchoredPosition = startAnchoredPosition + direction * distance * 0.5f;
        lineRectTransform.sizeDelta = new Vector2(distance, 5f);
        lineRectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        line.GetComponent<Image>().color = color;
    }

    private void CreateText(Vector2 anchoredPosition, string text)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(graphContainer, false);
        RectTransform textRectTransform = textObj.AddComponent<RectTransform>();
        textRectTransform.anchoredPosition = anchoredPosition;

        Text labelText = textObj.AddComponent<Text>();
        labelText.text = text;
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 20;
        labelText.color = textColor;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.horizontalOverflow = HorizontalWrapMode.Overflow;
        labelText.verticalOverflow = VerticalWrapMode.Overflow;

        textObj.transform.SetAsLastSibling();
    }
}
