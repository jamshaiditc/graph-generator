using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoGraph2 : MonoBehaviour
{
    public RectTransform graphContainer;
    public List<Vector2> dataPoints = new List<Vector2>();
    public float xDivision, yDivision;
    public Color pointColor = Color.white;
    public Color lineColor = Color.white;
    public Color axisColor = Color.white;
    public Color textColor = Color.white;
    public string xAxisLabel = "X-axis";
    public string yAxisLabel = "Y-axis";
    public Color axisLabelColor = Color.white;

    // New fields for generating random data points
    public int maxDataPoints = 50;
    public float updateInterval = 0.5f;
    public float xAxisLength = 10f;
    public float yAxisLength = 10f;

    private int currentIndex = 0;

    private void Start()
    {
        ShowGraph();
        InvokeRepeating("AddDataPoints", 0f, updateInterval);
    }

    private void ShowGraph()
    {
        CreateLine(new Vector2(0f, 0f), new Vector2(graphContainer.sizeDelta.x, 0f), axisColor); // X-axis
        CreateLine(new Vector2(0f, 0f), new Vector2(0f, graphContainer.sizeDelta.y), axisColor); // Y-axis

        float xDivisionInterval = xAxisLength / xDivision;
        float yDivisionInterval = yAxisLength / yDivision;

        // X-axis markings
        for (int i = 0; i <= xDivision; i++)
        {
            float xValue = i * xDivisionInterval;
            float xPosition = Mathf.InverseLerp(0f, xAxisLength, xValue) * graphContainer.sizeDelta.x;
            CreateText(new Vector2(xPosition, -40f), System.DateTime.Now.AddSeconds(i * 5 - 1).ToString("HH:mm:ss"), textColor);
            CreateLine(new Vector2(xPosition, -5f), new Vector2(xPosition, 5f), axisColor);
        }

        // Y-axis markings
        for (int i = 0; i <= yDivision; i++)
        {
            float yValue = i * yDivisionInterval;
            float yPosition = Mathf.InverseLerp(0f, yAxisLength, yValue) * graphContainer.sizeDelta.y;
            CreateText(new Vector2(-40f, yPosition), yValue.ToString("F0"), textColor);
            CreateLine(new Vector2(-5f, yPosition), new Vector2(5f, yPosition), axisColor);
        }

        CreateText(new Vector2(graphContainer.sizeDelta.x * 0.5f, -70f), xAxisLabel, axisLabelColor);
        CreateText(new Vector2(-70f, graphContainer.sizeDelta.y * 0.5f), yAxisLabel, axisLabelColor);
    }

    private void AddDataPoints()
    {
        ClearGraph();
        ShowGraph();

        Vector2[] dataPoints = GenerateDataPoints();

        for (int i = 0; i < dataPoints.Length; i++)
        {
            Vector2 currentDataPoint = dataPoints[i];
            float xPosition = Mathf.InverseLerp(0f, xAxisLength, currentDataPoint.x) * graphContainer.sizeDelta.x;
            float yPosition = Mathf.InverseLerp(0f, yAxisLength, currentDataPoint.y) * graphContainer.sizeDelta.y;

            if (i > 0)
            {
                Vector2 prevDataPoint = dataPoints[i - 1];
                float prevXPosition = Mathf.InverseLerp(0f, xAxisLength, prevDataPoint.x) * graphContainer.sizeDelta.x;
                float prevYPosition = Mathf.InverseLerp(0f, yAxisLength, prevDataPoint.y) * graphContainer.sizeDelta.y;

                CreateLine(new Vector2(prevXPosition, prevYPosition), new Vector2(xPosition, yPosition), lineColor);
            }

            CreatePoint(new Vector2(xPosition, yPosition));
        }
    }

    private void ClearGraph()
    {
        foreach (Transform child in graphContainer)
            Destroy(child.gameObject);
    }

    private void CreatePoint(Vector2 anchoredPosition)
    {
        GameObject point = new GameObject("Point");
        point.transform.SetParent(graphContainer, false);

        RectTransform pointRectTransform = point.AddComponent<RectTransform>();
        pointRectTransform.anchoredPosition = anchoredPosition;
        pointRectTransform.sizeDelta = new Vector2(15f, 15f);

        Image pointImage = point.AddComponent<Image>();
        pointImage.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
        pointImage.color = pointColor;

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

    private void CreateText(Vector2 anchoredPosition, string text, Color color)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(graphContainer, false);

        RectTransform textRectTransform = textObj.AddComponent<RectTransform>();
        textRectTransform.anchoredPosition = anchoredPosition;

        Text labelText = textObj.AddComponent<Text>();
        labelText.text = text;
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 20;
        labelText.color = color;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.horizontalOverflow = HorizontalWrapMode.Overflow;
        labelText.verticalOverflow = VerticalWrapMode.Overflow;

        if (anchoredPosition.x < 0f)
            textRectTransform.Rotate(new Vector3(0f, 0f, 90f));

        textObj.transform.SetAsLastSibling();
    }

    private Vector2[] GenerateDataPoints()
    {
        Vector2[] dataPoints = new Vector2[maxDataPoints];

        for (int i = 0; i < maxDataPoints; i++)
        {
            float xValue = i * xAxisLength / (maxDataPoints - 1);
            float yValue = Random.Range(0f, yAxisLength);
            dataPoints[i] = new Vector2(xValue, yValue);
        }

        return dataPoints;
    }
}