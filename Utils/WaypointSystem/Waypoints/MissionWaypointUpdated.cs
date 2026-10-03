using FPSAssets._Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionWaypointUpdated : MonoBehaviour
{
    public bool turnOnDistance;
    public FPS_Movement playerObject;
    public float screenPadding = 40f;
    public Image wayPointImage;
    public Image directionImage;
    //public Transform target;
    public TextMeshProUGUI distanceRemaining;
    public Vector3 waypointOffset;
    public Camera cam;
   

    public void Enable()
    {
        cam = Camera.main;
        if(distanceRemaining != null) distanceRemaining.gameObject.SetActive(turnOnDistance);
        Invoke(nameof(StartWaypoint) , 0.2f);
    }

    private void StartWaypoint()
    {
        wayPointImage.gameObject.SetActive(true);
    }

    public void SetImage(Image image)
    {
        if(wayPointImage != null) return;
        wayPointImage = image;
        directionImage = wayPointImage.transform.GetChild(0).GetComponent<Image>();
        //wayPointImage.gameObject.SetActive(true);
    }

    public void DisableWaypoint()
    {
        if (wayPointImage == null) return;
        
        WaypointManager.disableWaypoint?.Invoke(wayPointImage.gameObject);
    }

    private void Update()
{
    if (cam == null) return;
    if (playerObject == null) return;
    if (wayPointImage == null) return;

    float halfWidth = wayPointImage.GetPixelAdjustedRect().width * 0.5f;
    float halfHeight = wayPointImage.GetPixelAdjustedRect().height * 0.5f;

    float minX = halfWidth + screenPadding;
    float maxX = Screen.width - halfWidth - screenPadding;

    float minY = halfHeight + screenPadding;
    float maxY = Screen.height - halfHeight - screenPadding;

    // Target position in screen space
    Vector3 screenPosition =
        cam.WorldToScreenPoint(transform.position + waypointOffset);

    bool behindCamera = screenPosition.z < 0f;

    // Direction from center of screen toward target
    Vector2 screenCenter = new Vector2(
        Screen.width * 0.5f,
        Screen.height * 0.5f
    );

    Vector2 direction = new Vector2(
        screenPosition.x,
        screenPosition.y
    ) - screenCenter;

    // WorldToScreenPoint gets flipped when target is behind camera,
    // so reverse the direction.
    if (behindCamera)
    {
        direction *= -1f;
    }

    bool offScreen =
        behindCamera ||
        screenPosition.x < minX ||
        screenPosition.x > maxX ||
        screenPosition.y < minY ||
        screenPosition.y > maxY;

    Vector2 waypointPosition;

    if (offScreen)
    {
        direction.Normalize();

        directionImage.enabled = true;
        wayPointImage.enabled = false;

        // Find where this direction intersects the screen bounds
        float xLimit = direction.x != 0
            ? (direction.x > 0
                ? (maxX - screenCenter.x) / direction.x
                : (minX - screenCenter.x) / direction.x)
            : float.MaxValue;

        float yLimit = direction.y != 0
            ? (direction.y > 0
                ? (maxY - screenCenter.y) / direction.y
                : (minY - screenCenter.y) / direction.y)
            : float.MaxValue;

        float distanceToEdge = Mathf.Min(
            Mathf.Abs(xLimit),
            Mathf.Abs(yLimit)
        );

        waypointPosition =
            screenCenter + direction * distanceToEdge;

        // Rotate waypoint toward target
        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        /*
         * This assumes your arrow sprite points RIGHT by default.
         *
         * Right = 0
         * Up = 90
         * Left = 180
         * Down = -90
         */
        directionImage.rectTransform.rotation =
            Quaternion.Euler(0f, 0f, angle - 90f);
    }
    else
    {
        wayPointImage.enabled = false;
        directionImage.enabled = false;
        waypointPosition = screenPosition;

        // NPC is visible, so don't rotate waypoint
        directionImage.rectTransform.rotation =
            Quaternion.identity;
    }

    // Safety clamp
    waypointPosition.x =
        Mathf.Clamp(waypointPosition.x, minX, maxX);

    waypointPosition.y =
        Mathf.Clamp(waypointPosition.y, minY, maxY);

    int distanceR =
        (int)Vector3.Distance(
            transform.position,
            playerObject.transform.position
        );

    //float targetScale = distanceR > 10 ? 0.5f : 1f;

    // wayPointImage.transform.localScale =
    //     Vector3.Lerp(
    //         wayPointImage.transform.localScale,
    //         Vector3.one * targetScale,
    //         10f * Time.deltaTime
    //     );

    wayPointImage.transform.position = waypointPosition;

    if (distanceRemaining != null)
    {
        distanceRemaining.text = $"{distanceR}m";
    }
}

}
