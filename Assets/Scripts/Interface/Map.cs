using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Map : MonoBehaviour {

    public MarkerList markerDatabase;
    public Interface Interface;
    public Camera cam;
    public Terrain terrain;
    public GUISkin skin;

    public Marker personalMarker;
    public bool setPersonal;
    public float markerHeightOffset; 

    public Vector2 dragDistance = new Vector2(0, 0);
    public Vector2 zoomDistance = new Vector2(0, 0);
    public float zoomFactor = 1f;

    public float markerSize = 30f;
    public float worldMarkerSize = 40f;

    public bool showLegend = false;

    public Vector2 mapCenter = new Vector2(400, 200);

    public Vector2 mapPos = new Vector2(25, 13);
    public Vector2 mapSize = new Vector2(800, 450);

    public Vector2 legendDistance = new Vector2(10, 10);
    public Vector2 legendInSize = new Vector2(130, 140);
    public Vector2 legendOutSize = new Vector2(80, 30);

    public Vector2 legendTextPos = new Vector2(35, 3);
    public Vector2 legendTextSize = new Vector2(95, 30);
    public float legendTextDistance = 5;

    public float legendIconSize = 30;
    public float legendIconPosX = 30;

    public Vector2 compassPos = new Vector2(20,20);
    public float compassSize = 150;
    public float directionSize = 30;
    public int directionCount = 4;
    public string[] directions = new string[] { "S", "W", "N", "O" };

    public Vector3 screenPos;


	// Use this for initialization
	void Awake () 
    {
	    markerDatabase = GameObject.FindWithTag("Database").GetComponent<MarkerList>();
        Interface = GetComponent<Interface>();
        cam = transform.root.Find("Camera").GetComponent<Camera>();
        terrain = GameObject.FindWithTag("Terrain").GetComponent<Terrain>();
        personalMarker = new Marker();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") && Interface.currentGUI == Interface.GUIType.Map)
        {
            setPersonal = true;
        }

        if (Vector3.Distance(markerDatabase.marker[0].GetPosition, personalMarker.GetPosition) < 5)
        {
            personalMarker = new Marker();
        }

        // Debug.Log(Mathf.Clamp(10, 1, 3));
    }

    public void DrawMap()
    {
        // Move Map
        if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
        {
            dragDistance = Event.current.mousePosition - mapPos;
        }
        if (Event.current.button == 1 && Event.current.type == EventType.MouseDrag)
        {
            mapPos = Event.current.mousePosition - dragDistance;
        }


        //Zoom
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheel != 0)
        {
            zoomDistance.x = (Event.current.mousePosition.x - mapPos.x) / (mapSize.x * zoomFactor);
            //Debug.Log("Mouse Distance: " + (Event.current.mousePosition.x - mapPos.x) + " MapSize: " + mapSize.x * zoomFactor + " ZoomDistance: " + zoomDistance.x);
            zoomDistance.y = (Event.current.mousePosition.y - mapPos.y) / (mapSize.y * zoomFactor);

            zoomFactor += mouseWheel;
            zoomFactor = Mathf.Clamp(zoomFactor, 1f, 2f);

            mapPos.x = Event.current.mousePosition.x - zoomDistance.x * mapSize.x * zoomFactor;
            mapPos.y = Event.current.mousePosition.y - zoomDistance.y * mapSize.y * zoomFactor;

        }

        // Draw Map
        Rect mapRect = new Rect(mapPos.x, mapPos.y, mapSize.x * zoomFactor, mapSize.y * zoomFactor);
        GUI.Box(mapRect, "", skin.GetStyle("Inventory"));
          
        //Draw Center
        Rect centerRect = new Rect(mapPos.x + mapCenter.x * zoomFactor, mapPos.y + mapCenter.y * zoomFactor, markerSize, markerSize);
        GUI.Box(centerRect, "", skin.GetStyle("Circle"));


        Marker nextPersonal = null;
        //LookForMarker
        for (int i = 0; i < markerDatabase.marker.Count; i++)
        {
            Marker curMarker = markerDatabase.marker[i];
            Rect markerRect = new Rect(centerRect.center.x + -curMarker.GetPosition.x * zoomFactor - markerSize / 2, centerRect.center.y + curMarker.GetPosition.z * zoomFactor - markerSize / 2, markerSize, markerSize);
            GUI.Box(markerRect, "", skin.GetStyle("Inventory"));
            if(curMarker.icon != null)
            {
                GUI.DrawTexture(markerRect, curMarker.icon);
            }
            if(setPersonal && markerRect.Contains(Event.current.mousePosition))
            nextPersonal = curMarker;
        }

        //Look for personalMarker
        Rect personalMarkerRect = new Rect();
        if(personalMarker.name != null)
        {
            personalMarkerRect = new Rect(centerRect.center.x + personalMarker.GetPosition.x * zoomFactor - markerSize / 2, centerRect.center.y + personalMarker.GetPosition.z * zoomFactor - markerSize / 2, markerSize, markerSize);
            //GUI.Box(personalMarkerRect, "", skin.GetStyle("Inventory"));
            if (personalMarker.icon != null)
            {
                GUI.DrawTexture(personalMarkerRect, personalMarker.icon);
            }
        }

        //Set personal Marker
        if (setPersonal)
        {
            if (mapRect.Contains(Event.current.mousePosition))
            {
                if (personalMarkerRect.Contains(Event.current.mousePosition))
                {
                    personalMarker = new Marker();
                    Debug.Log("reset");
                }
                else if (nextPersonal != null)
                {
                    personalMarker = new Marker("Personal", nextPersonal.GetPosition, Marker.Type.Personal, nextPersonal.type);
                }
                else
                {
                    // Mouse Position from Screen
                    Vector2 mousePosition = (Event.current.mousePosition - centerRect.center) / zoomFactor;
                    // 2D to 3D position
                    Vector3 worldPosition = new Vector3(mousePosition.x, 0, mousePosition.y);
                    // Get Terrain height
                    worldPosition.y = terrain.SampleHeight(worldPosition);
                    personalMarker = new Marker("Personal", worldPosition, Marker.Type.Personal);
                    Debug.Log("set");
                }
                setPersonal = false;
            }
        }

        DrawLegend();
    }


    void DrawLegend()
    {
        if (showLegend)
        {
            Rect legendRectLeft = new Rect(Screen.width - legendInSize.x * 2 - legendDistance.x, Screen.height - legendInSize.y - legendDistance.y, legendInSize.x, legendInSize.y);
            GUI.Box(legendRectLeft, "", skin.GetStyle("Inventory"));

            Rect legendRectRight = new Rect(Screen.width - legendInSize.x - legendDistance.x, Screen.height - legendInSize.y - legendDistance.y, legendInSize.x, legendInSize.y);
            GUI.Box(legendRectRight, "", skin.GetStyle("Inventory"));

            Rect legendHeading = new Rect(legendRectLeft.xMin + legendRectLeft.width - legendOutSize.x / 2, legendRectLeft.yMin - legendOutSize.y, legendOutSize.x, legendOutSize.y);
            GUI.Box(legendHeading, "Legend", skin.GetStyle("Inventory"));
            if (legendHeading.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    showLegend = !showLegend;
                }
            }

            for (int i = 0; i < markerDatabase.legend.Count; i++)
            {
                Marker curMarker = markerDatabase.legend[i];
                Rect textRect;
                if (i < 4)
                {
                    textRect = new Rect(legendRectLeft.xMin + legendTextPos.x, legendRectLeft.yMin + legendTextPos.y + legendTextSize.y * i + legendTextDistance * i, legendTextSize.x, legendTextSize.y);
                }
                else
                {
                    textRect = new Rect(legendRectRight.xMin + legendTextPos.x, legendRectRight.yMin + legendTextPos.y + legendTextSize.y * (i - 4) + legendTextDistance * (i - 4), legendTextSize.x, legendTextSize.y);

                }
                GUI.Box(textRect, curMarker.name, skin.GetStyle("Inventory"));

                Rect iconRect = new Rect(textRect.xMin - legendIconPosX, textRect.yMin + textRect.height / 2 - legendIconSize / 2, legendIconSize, legendIconSize);
                GUI.Box(iconRect, "", skin.GetStyle("Inventory"));
                if (curMarker.icon)
                {
                    GUI.DrawTexture(iconRect, curMarker.icon);
                }


            }
        }
        else
        {
            Rect legendRect = new Rect(Screen.width - legendOutSize.x - legendDistance.x, Screen.height - legendOutSize.y - legendDistance.y, legendOutSize.x, legendOutSize.y);
            GUI.Box(legendRect, "Legend", skin.GetStyle("Inventory"));
            if (legendRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    showLegend = !showLegend;
                }
            }
        }
    }


    public void DrawMinimap()
    {
        // Draw PersonalMarker
        if (personalMarker.name != null)
        {
            Vector3 markerPos = personalMarker.GetPosition;
            markerPos.y += markerHeightOffset;
            if (Vector3.Angle(cam.transform.forward, markerPos - cam.transform.position) < 90)
            {
                screenPos = cam.WorldToScreenPoint(markerPos);
                Rect personalMarkerRect = new Rect(screenPos.x, Screen.height - screenPos.y, worldMarkerSize, worldMarkerSize);
                GUI.Box(personalMarkerRect, "", skin.GetStyle("Inventory"));
                if (personalMarker.extendsIcon != null)
                {
                    GUI.DrawTexture(personalMarkerRect, personalMarker.extendsIcon);
                }
                GUI.DrawTexture(personalMarkerRect, personalMarker.icon);
            }
        }
        // Draw Compass
        Rect compassRect = new Rect(Screen.width - compassSize - compassPos.x, compassPos.y, compassSize, compassSize);
        GUI.Box(compassRect, "", skin.GetStyle("Circle"));

        // Rotate Compass
        Vector3 camDir = cam.transform.forward;
        camDir.y = 0;
        Vector3 worldDir = transform.forward;

        Vector3 axisSign = Vector3.Cross(camDir, worldDir);

        float angle = Vector3.Angle(camDir, worldDir) * (axisSign.y <= 0 ? -1 : 1);

        GUIUtility.RotateAroundPivot(angle, compassRect.center);

        // Draw Directions
        for (int i = 0; i < directionCount; i++)
        {
            Rect directionRect = new Rect(compassRect.center.x - directionSize / 2, compassRect.center.y - compassRect.height / 2, directionSize, directionSize);
            GUI.Box(directionRect, directions[i], skin.GetStyle("Inventory"));
            GUIUtility.RotateAroundPivot(360 / directionCount, compassRect.center);
        }

        GUIUtility.RotateAroundPivot(-angle - 360, compassRect.center);


    }
}
