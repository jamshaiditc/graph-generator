using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoGraph : MonoBehaviour
{
    public RectTransform graphContainer;
    public List<Vector2> dataPoints = new List<Vector2>();

    public float xMin, xMax, yMin, yMax;
    public float xDivision, yDivision;
    public Color pointColor = Color.white;
    public Color lineColor = Color.white;
    public Color xAxisColor = Color.white;
    public Color yAxisColor = Color.white;
    public Color textColor = Color.white;

    public string xAxisLabel = "X-axis";
    public string yAxisLabel = "Y-axis";
    public Color xAxisLabelColor = Color.white;
    public Color yAxisLabelColor = Color.white;

    private int currentIndex = 0;

    private void Start()
    {
        GenerateRandomDataPoints();
        ShowGraph();
        InvokeRepeating("AddDataPoint", 0f, 5f);
    }

    private void GenerateRandomDataPoints()
    {
        // Clear existing data points
        dataPoints.Clear();

        // Generate random data points
        for (int i = 0; i < xDivision; i++)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomY = Random.Range(yMin, yMax);
            Vector2 dataPoint = new Vector2(randomX, randomY);
            dataPoints.Add(dataPoint);
        }

        // Sort data points based on x-values in ascending order
        dataPoints.Sort((a, b) => a.x.CompareTo(b.x));
    }


    private void ShowGraph()
    {
        // Create X-axis line
        CreateLine(new Vector2(0f, 0f), new Vector2(graphContainer.sizeDelta.x, 0f), xAxisColor);

        // Create Y-axis line
        CreateLine(new Vector2(0f, 0f), new Vector2(0f, graphContainer.sizeDelta.y), yAxisColor);

        // Calculate xDivisionInterval and yDivisionInterval
        float xDivisionInterval = (xMax - xMin) / xDivision;
        float yDivisionInterval = (yMax - yMin) / yDivision;

        // Add X-axis text and markings
        for (int i = 0; i <= xDivision; i++)
        {
            float xValue = xMin + i * xDivisionInterval;
            float xPosition = Mathf.InverseLerp(xMin, xMax, xValue) * graphContainer.sizeDelta.x;
            CreateText(new Vector2(xPosition, -40f), xValue.ToString("F0"), textColor);
            CreateLine(new Vector2(xPosition, -5f), new Vector2(xPosition, 5f), xAxisColor);
        }

        // Add Y-axis text and markings
        for (int i = 0; i <= yDivision; i++)
        {
            float yValue = yMin + i * yDivisionInterval;
            float yPosition = Mathf.InverseLerp(yMin, yMax, yValue) * graphContainer.sizeDelta.y;
            CreateText(new Vector2(-40f, yPosition), yValue.ToString("F0"), textColor);
            CreateLine(new Vector2(-5f, yPosition), new Vector2(5f, yPosition), yAxisColor);
        }

        CreateText(new Vector2(graphContainer.sizeDelta.x * 0.5f, -70f), xAxisLabel, xAxisLabelColor);
        CreateText(new Vector2(-70f, graphContainer.sizeDelta.y * 0.5f), yAxisLabel, yAxisLabelColor);
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
        {
            textRectTransform.Rotate(new Vector3(0f, 0f, 90f));
        }
        textObj.transform.SetAsLastSibling();
    }

    public void AddDataPoint()
    {
        if (currentIndex < dataPoints.Count)
        {
            Vector2 currentDataPoint = dataPoints[currentIndex];
            float xPosition = Mathf.InverseLerp(xMin, xMax, currentDataPoint.x) * graphContainer.sizeDelta.x;
            float yPosition = Mathf.InverseLerp(yMin, yMax, currentDataPoint.y) * graphContainer.sizeDelta.y;

            if (currentIndex > 0)
            {
                Vector2 prevDataPoint = dataPoints[currentIndex - 1];
                float prevXPosition = Mathf.InverseLerp(xMin, xMax, prevDataPoint.x) * graphContainer.sizeDelta.x;
                float prevYPosition = Mathf.InverseLerp(yMin, yMax, prevDataPoint.y) * graphContainer.sizeDelta.y;

                // Check if it's the last data point and not the second last data point
                if (currentIndex != dataPoints.Count - 1 || currentIndex == dataPoints.Count - 2)
                {
                    CreateLine(new Vector2(prevXPosition, prevYPosition), new Vector2(xPosition, yPosition), lineColor);
                }
            }

            CreatePoint(new Vector2(xPosition, yPosition));
            CreateText(new Vector2(xPosition + 75f, yPosition), "(" + currentDataPoint.x.ToString("F1") + ", " + currentDataPoint.y.ToString("F1") + ")", textColor);

            currentIndex++;
        }
    }








    public void RemoveDataPoint()
    {
        if (currentIndex > 0)
        {
            currentIndex--;

            Vector2 currentDataPoint = dataPoints[currentIndex];
            float xPosition = Mathf.InverseLerp(xMin, xMax, currentDataPoint.x) * graphContainer.sizeDelta.x;
            float yPosition = Mathf.InverseLerp(yMin, yMax, currentDataPoint.y) * graphContainer.sizeDelta.y;

            Transform graphContainerTransform = graphContainer.transform;
            int childCount = graphContainerTransform.childCount;
            Destroy(graphContainerTransform.GetChild(childCount - 1).gameObject);
            Destroy(graphContainerTransform.GetChild(childCount - 2).gameObject);
            if (currentIndex > 0)
            {
                Destroy(graphContainerTransform.GetChild(childCount - 3).gameObject);
            }
        }
    }
}
