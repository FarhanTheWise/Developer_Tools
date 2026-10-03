using FPSAssets._Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionWaypoint : MonoBehaviour
{
    public bool turnOnDistance;
    public FPS_Movement playerObject;
    public float screenPadding = 40f;
    public Image wayPointImage;
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
        if(playerObject == null) return;

        float halfWidth = wayPointImage.GetPixelAdjustedRect().width * 0.5f;
        float halfHeight = wayPointImage.GetPixelAdjustedRect().height * 0.5f;

        float minX = halfWidth + screenPadding;
        float maxX = Screen.width - halfWidth - screenPadding;

        float minY = halfHeight + screenPadding;
        float maxY = Screen.height - halfHeight - screenPadding;

        Vector2 worldPosition = cam.WorldToScreenPoint(transform.position + waypointOffset);
       

        if (Vector3.Dot((transform.position - cam.transform.position), cam.transform.forward) < 0)
        {


            if(worldPosition.x < Screen.width / 2)
            {
                worldPosition.x = maxX;
            }
            else
            {
                worldPosition.x = minX;
            }
        }

        worldPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);
        worldPosition.y = Mathf.Clamp(worldPosition.y, minY, maxY);

        int distanceR = (int)Vector3.Distance(transform.position, playerObject.transform.position);

        wayPointImage.transform.localScale = Vector3.Lerp(wayPointImage.transform.localScale, 
            new Vector3(distanceR > 10 ? 0.5f : 1f, distanceR > 10 ? 0.5f : 1f, distanceR > 10 ? 0.5f : 1f), 
            10 * Time.deltaTime);
        
        wayPointImage.transform.position = worldPosition;
        if(distanceRemaining != null) distanceRemaining.text = $"{distanceR}m";

    }

}
