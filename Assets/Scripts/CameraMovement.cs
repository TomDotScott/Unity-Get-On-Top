using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private FocusLevel focusLevel;

    [SerializeField] private List<GameObject> players;

    [SerializeField] private float zoomUpdateSpeed;
    [SerializeField] private float panUpdateSpeed;
    [SerializeField] private float positionUpdateSpeed;

    [SerializeField] private float zoomMax;
    [SerializeField] private float zoomMin;

    [SerializeField] private float panMax;
    [SerializeField] private float panMin;

    private float cameraEulerX;
    private Vector3 cameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        players.Add(focusLevel.gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CalculateCameraLocation();
        MoveCamera();
    }

    private void MoveCamera()
    {
        Vector3 position = gameObject.transform.position;
        if (position != cameraPosition)
        {
            Vector3 newPosition = Vector3.zero;
            newPosition.x = Mathf.MoveTowards(position.x, cameraPosition.x, positionUpdateSpeed * Time.deltaTime);
            newPosition.y = Mathf.MoveTowards(position.y, cameraPosition.y, positionUpdateSpeed * Time.deltaTime);
            newPosition.z = -10;

            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, cameraPosition.z, zoomUpdateSpeed * Time.deltaTime);

            gameObject.transform.position = newPosition;
        }

        Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
        if (localEulerAngles.x != cameraEulerX)
        {
            Vector3 targetEulerAngles = new Vector3(cameraEulerX, localEulerAngles.y, localEulerAngles.z);
            gameObject.transform.localEulerAngles = Vector3.MoveTowards(localEulerAngles, targetEulerAngles, panUpdateSpeed);
        }
    }

    private void CalculateCameraLocation()
    {
        Vector3 totalPositions = Vector3.zero;

        // Find the box that contains all the players
        Bounds playerBounds = new Bounds();

        foreach (var player in players)
        {
            Vector3 playerPosition = player.transform.position;

            // Find if the player is in focus
            if (!focusLevel.focusBounds.Contains(playerPosition))
            {
                float playerX = Mathf.Clamp(playerPosition.x, focusLevel.focusBounds.min.x, focusLevel.focusBounds.max.x);
                float playerY = Mathf.Clamp(playerPosition.y, focusLevel.focusBounds.min.y, focusLevel.focusBounds.max.y);
                float playerZ = Mathf.Clamp(playerPosition.z, focusLevel.focusBounds.min.z, focusLevel.focusBounds.max.z);

                playerPosition = new Vector3(playerX, playerY, playerZ);
            }

            totalPositions += playerPosition;
            playerBounds.Encapsulate(playerPosition);
        }

        Vector3 averageCentre = totalPositions / players.Count;

        float extents = playerBounds.extents.x + playerBounds.extents.y;
        float lerpPercent = Mathf.InverseLerp(0, (focusLevel.halfXBounds + focusLevel.halfYBounds) / 2, extents);

        // Zoom according to the players' positions
        float zoom = Mathf.Lerp(zoomMax, zoomMin, lerpPercent);

        float angle = Mathf.Lerp(panMax, panMin, lerpPercent);

        cameraEulerX = angle;
        cameraPosition = new Vector3(averageCentre.x, averageCentre.y, zoom);
    }
}
