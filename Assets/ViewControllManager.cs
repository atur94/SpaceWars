using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class ViewControllManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isSelected;
    private bool isMovingCamera;
    public Vector3 startPosition;
    public Vector3 lastPosition;
    public Vector3 currentPosition;

    public Vector2 zoomRange;

    private Vector3 diff;

    private Camera mainCamera;

    public PlanetManager planetManager;
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        planetManager = FindObjectOfType<PlanetManager>();
        planetManager.PropertyChanged += PlanetManagerOnPropertyChanged;
    }

    

    // Update is called once per frame
    void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            isMovingCamera = true;
            Vector3 pos = Input.mousePosition;
            pos.z = 0;
            startPosition = Camera.main.ScreenToWorldPoint(pos);
            currentPosition = startPosition;
            lastPosition = startPosition;
        }


        if (Input.GetKey(KeyCode.Mouse2))
        {
            if (isMovingCamera)
            {
                Vector3 pos = Input.mousePosition;
                pos.z = 0;
                currentPosition = Camera.main.ScreenToWorldPoint(pos);
                diff = Vector3.Lerp(lastPosition - currentPosition, Vector3.zero, 0.1f);
                
                lastPosition = Vector3.Lerp(currentPosition, lastPosition, 0.3f);

                transform.position += diff;
            }
        }

        mainCamera.orthographicSize -= Input.mouseScrollDelta.y;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, zoomRange.x, zoomRange.y);

        if (Input.GetKeyUp(KeyCode.Mouse2))
        {
            isMovingCamera = false;
        }
    }

    private void zoomCamera()
    {

    }

    private void PlanetManagerOnPropertyChanged(object sender, PropertyChangedEventArgs property)
    {
        if (property.PropertyName.Equals("CurrentlySelectedPlanet"))
        {
            var planetManager = sender as PlanetManager;
            if (planetManager.CurrentlySelectedPlanet != null)
            {
                desiredCameraPosition = planetManager.CurrentlySelectedPlanet.transform.position;
                desiredCameraPosition.z = transform.position.z;
                StartCoroutine(CameraMove());
            }
        }
    }

    private Vector3 desiredCameraPosition;

    IEnumerator CameraMove()
    {
        while (Vector3.Distance(desiredCameraPosition, transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, desiredCameraPosition, Time.deltaTime * 550f);
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
    }
}
