using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class MapController_Dynamic : MonoBehaviour
{
   [Header("UI References")]
   public RectTransform mapParent;
   public GameObject areaPrefab;
      public RectTransform playerIcon;


      [Header("Colours")]
   public Color defaultColour = Color.gray;
   public Color CurrentAreaColor = Color.green;

   [Header("Map Settings")]
   public GameObject MapBounds;
   public PolygonCollider2D initialArea;
   public float mapScale = 10f;//size of map in UI

   private PolygonCollider2D[] mapAreas;
   private Dictionary<string,RectTransform> uiAreas  = new Dictionary<string,RectTransform>();

public static MapController_Dynamic Instance {get; set;}


private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

        mapAreas = MapBounds.GetComponentsInChildren<PolygonCollider2D>();
    }

    //Generate Map
    public void GenerateMap(PolygonCollider2D newCurrentArea = null)
    {
        PolygonCollider2D currentArea = newCurrentArea != null ? newCurrentArea: initialArea;

        ClearMap();

        foreach (PolygonCollider2D area in mapAreas)
        {
            //CreateAreaUI

            CreateAreaUI(area,area == currentArea);
        }


        MovePlayerIcon(currentArea.name);
    }

    //clear map

    private void ClearMap()
    {
        foreach(Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }

        uiAreas.Clear();
    }

    private void CreateAreaUI(PolygonCollider2D area, bool isCurrent)
    {
        //Instantiate prefab for object
        GameObject areaImage = Instantiate(areaPrefab, mapParent);
        RectTransform rectTransform = areaImage.GetComponent<RectTransform>();

        //GetBounds
        Bounds bounds = area.bounds;

        //ScaleUI image to fit map and bounds

        rectTransform.sizeDelta = new Vector2(bounds.size.x*mapScale, bounds.size.y*mapScale);
        rectTransform.anchoredPosition = bounds.center * mapScale;

        //Set colur based on current or not
        areaImage.GetComponent<Image>().color = isCurrent? CurrentAreaColor : defaultColour;

        //Addd to ditionary
        uiAreas[area.name]= rectTransform;
    
    
    
    }


    //UpdateCurrentArea
public void UpdateCurrentArea(string newCurrentArea)
    {
//Update Colour
foreach(KeyValuePair<string, RectTransform> area in uiAreas)
        {
            area.Value.GetComponent<Image>().color = area.Key == newCurrentArea? CurrentAreaColor :defaultColour;
        }     
        MovePlayerIcon(newCurrentArea);   
    }








private void MovePlayerIcon(string newCurrentArea)
{
    if (playerIcon == null)
    {
        Debug.LogWarning("Player icon is NULL");
        return;
    }

    if (uiAreas == null || uiAreas.Count == 0)
    {
        Debug.LogWarning("UI Areas not generated yet");
        return;
    }

    if (uiAreas.TryGetValue(newCurrentArea, out RectTransform areaUI))
    {
        if (areaUI != null)
        {
            playerIcon.anchoredPosition = areaUI.anchoredPosition;
        }
    }
    else
    {
        Debug.LogWarning("Area UI not found: " + newCurrentArea);
    }
}


    //MovePlayerIcon


}
