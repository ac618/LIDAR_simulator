using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class LIDAR : MonoBehaviour
{
    /* Parameters */
    [SerializeField] private float precision = 1; // degree
    [SerializeField] private int dt = 100; // milliseconds
    [SerializeField] private float maxRange = 100; // meters
    [SerializeField] private float startAngle = 330; // degree
    [SerializeField] private float endAngle = 270; // degree
    [SerializeField] private string id = "LIDAR_1"; // LIDAR ID
    /* State Variables */
    private float currentAngle = 0; // degree
    private float currentTime = 0; // milliseconds
    private bool isScanning = false;
    /* Memory */
    private StringBuilder scanData = new StringBuilder(); // for storing scan data

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isScanning)
        {
            isScanning = true;
            currentAngle = startAngle;
            currentTime = 0;
            scanData.Clear();
            scanData.AppendLine("Time(s), Angle(degree), Distance(m)"); // header for the scan data
            StartCoroutine(ScanCoroutine());
        }
    }
    IEnumerator ScanCoroutine()
    {
        float angleDiff = endAngle - startAngle;
        if (angleDiff < 0)
        {
            angleDiff += 360;
        }
        float totalTime = angleDiff / precision * dt; // total time for the scan
        while (currentTime < totalTime)
        {
            Scan();
            yield return null;
            currentAngle += precision;
            currentTime += dt;
            if (currentAngle >= 360)
            {
                currentAngle -= 360;
            }
        }
        isScanning = false;
        SaveData();
    }

    void Scan()
    {
        // ray cast in the current angle
        float radians = Mathf.Deg2Rad * (currentAngle + transform.eulerAngles.y);
        Vector3 direction = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            float distance = hit.distance;
            scanData.AppendLine($"{currentTime / 1000:F2}, {currentAngle:F1}, {distance:F2}");
            // Draw a debug line to visualize the raycast (for debugging purposes)
            Debug.DrawLine(transform.position, hit.point, Color.green);
        }
        else
        {
            scanData.AppendLine($"{currentTime / 1000:F2}, {currentAngle:F1}, {maxRange:F2}");
            // Draw a debug line to visualize the raycast miss (for debugging purposes)
            Debug.DrawRay(transform.position, direction * maxRange, Color.red);
        }
    }

    void SaveData()
    {
        string directory = Path.Combine(Application.dataPath, "Outputs", $"{System.DateTime.Now:yyyyMMdd_HHmmss}");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        string filePath = Path.Combine(directory, $"{id}.csv");
        File.WriteAllText(filePath, scanData.ToString(), Encoding.UTF8);
        Debug.Log($"Scan data saved to {filePath}");
    }

    void OnDrawGizmos()
    {
        // degree to radian conversion
        float startRad = Mathf.Deg2Rad * (startAngle + transform.eulerAngles.y);
        float endRad = Mathf.Deg2Rad * (endAngle + transform.eulerAngles.y);
        // draw the circular section
        Gizmos.color = Color.cyan;
        float lineLength = 3f;
        int segments = 50;
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Lerp(startAngle, endAngle+360, (float)i / segments);
            float rad = Mathf.Deg2Rad * (angle + transform.eulerAngles.y);
            Vector3 direction = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
            Gizmos.DrawLine(transform.position, transform.position + direction * lineLength);
        }
    }
}
