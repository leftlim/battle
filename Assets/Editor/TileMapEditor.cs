using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor
{
    private static GUIStyle ToggleButtonStyleNormal = null;
    private static GUIStyle ToggleButtonStyleToggled = null;

    private TileMap tileMap = null;
    private Vector3 mousePos = Vector3.zero;
    private RaycastHit2D hit;
    private GameObject selectedObject;
    private Vector2 selectionPos = Vector2.zero;
    private Texture2D[] textureList;
    private Sprite[] spriteList;
    private int tileFilpX = 1;
    private int tileFilpY = 1;
    private int tileRotate = 0;
    public int selGridInt = 0;

    public enum BrushState
    {
        Idle,
        Select,
        Create,
        Erase
    }

    private BrushState brushState { get; set; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (textureList != null)
        {
            selGridInt = GUILayout.SelectionGrid(selGridInt, textureList, tileMap.gridSize);
        }
    }

    void OnEnable()
    {

        tileMap = target as TileMap;

        if (tileMap.sprites)
        {
            string path = AssetDatabase.GetAssetPath(tileMap.sprites);

            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            textureList = new Texture2D[sprites.Length];
            spriteList = new Sprite[sprites.Length];

            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] is Sprite)
                {
                    spriteList[i] = (Sprite)sprites[i];
                    Texture2D t = GenerateTextureFromSprite((Sprite)sprites[i]);

                    textureList[i] = t;

                }
            }
        }
        else
        {
            Debug.Log("Please Assign Sprite Sheet To The Tile Map Script");
        }



        brushState = BrushState.Idle;


        SceneView.onSceneGUIDelegate += GridUpdate;
        Debug.Log("Enabled");
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= GridUpdate;
        Debug.Log("Disabled");
    }

    void GridUpdate(SceneView sceneview)
    {
        if (ToggleButtonStyleNormal == null)
        {
            ToggleButtonStyleNormal = "Button";
            ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
            ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
        }

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        Event e = Event.current;

        Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
        mousePos = r.origin;

        mousePos -= tileMap.transform.position;
        mousePos.x = Mathf.Floor(mousePos.x / tileMap.tileSizeFloat) * tileMap.tileSizeFloat;
        mousePos.y = Mathf.Floor(mousePos.y / tileMap.tileSizeFloat) * tileMap.tileSizeFloat;
        mousePos += tileMap.transform.position + Vector3.one * tileMap.tileSizeFloat * 0.5f;
        mousePos.z = 0;

        Vector2 org = new Vector2(r.origin.x, r.origin.y);
        Vector2 dir = new Vector2(r.direction.x, r.direction.y);

        hit = Physics2D.Raycast(org, dir, Mathf.Infinity, LayerMask.GetMask("Tile"));


        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:

                if (e.button == 0)
                {
                    GUIUtility.hotControl = controlID;

                    if (CheckBounds())
                    {
                        if (brushState == BrushState.Create)
                        {
                            if (hit.collider == null)
                            {
                                GameObject obj = CreateSpriteGO(selGridInt, mousePos);
                                Undo.RegisterCreatedObjectUndo(obj, "Object");
                            }
                        }
                        else if (brushState == BrushState.Erase)
                        {
                            if (hit.collider != null)
                            {
                                DestroyImmediate(hit.collider.gameObject);
                            }
                        }
                        else if (brushState == BrushState.Select)
                        {
                            if (hit.collider != null)
                            {
                                selectedObject = hit.collider.gameObject;
                            }
                        }
                    }

                    Event.current.Use();
                }
                break;
            case EventType.MouseUp:
                if (e.button == 0)
                {
                    GUIUtility.hotControl = 0;

                    if (brushState == BrushState.Select)
                    {
                        if (selectedObject != null)
                        {
                            selectedObject = null;
                        }
                    }

                    Event.current.Use();
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    GUIUtility.hotControl = controlID;

                    if (CheckBounds())
                    {
                        if (brushState == BrushState.Create)
                        {
                            if (hit.collider == null)
                            {
                                GameObject obj = CreateSpriteGO(selGridInt, mousePos);
                                Undo.RegisterCreatedObjectUndo(obj, "Create");
                            }
                        }
                        else if (brushState == BrushState.Erase)
                        {
                            if (hit.collider != null)
                            {
                                DestroyImmediate(hit.collider.gameObject);
                            }
                        }
                        else if (brushState == BrushState.Select)
                        {
                            if (selectedObject != null)
                            {
                                Undo.RecordObject(selectedObject.transform, "Move " + selectedObject.name);
                                selectedObject.transform.position = mousePos;
                            }
                        }
                    }

                    Event.current.Use();
                }
                break;
            case EventType.KeyDown:
                if (e.isKey)
                {
                    if (e.keyCode.ToString() == "B")
                    {
                        if (brushState != BrushState.Create)
                            brushState = BrushState.Create;
                    }
                    if (e.keyCode.ToString() == "E")
                    {
                        if (brushState != BrushState.Erase)
                            brushState = BrushState.Erase;
                    }
                    if (e.keyCode.ToString() == "S")
                    {
                        if (brushState != BrushState.Select)
                            brushState = BrushState.Select;
                    }
                    if (e.keyCode.ToString() == "Escape")
                    {
                        if (brushState != BrushState.Idle)
                            brushState = BrushState.Idle;
                    }
                    if(e.keyCode.ToString() == "R")
                    {
                        if (++tileRotate > 3)
                            tileRotate = 0;
                    }
                    if(e.keyCode.ToString() == "T")
                    {
                        tileFilpX *= -1;
                    }
                    if (e.keyCode.ToString() == "G")
                    {
                        tileFilpY *= -1;
                    }
                }
                break;

        }




        Handles.BeginGUI();


        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(15);

        GUILayout.Button("PAINT(B)", brushState == BrushState.Create ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(150));


        GUILayout.Space(5);

        GUILayout.Button("ERASE(E)", brushState == BrushState.Erase ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(150));


        GUILayout.Space(5);

        GUILayout.Button("SELECT(S)", brushState == BrushState.Select ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(150));


        GUILayout.Space(5);

        GUILayout.TextField(string.Format("FX:{0}, FY:{1}, RT:{2}", tileFilpX, tileFilpY, tileRotate), GUILayout.Width(150));


        EditorGUILayout.EndHorizontal();

        Handles.EndGUI();

    }

    private bool CheckBounds()
    {
        Vector3 oriPos = tileMap.transform.position;
        Rect boundRect = new Rect(tileMap.transform.position, new Vector2(tileMap.mapWidth * tileMap.tileSizeFloat, tileMap.mapHeight * tileMap.tileSizeFloat));

        return boundRect.Contains(mousePos);
    }

    private Texture2D GenerateTextureFromSprite(Sprite aSprite)
    {
        var rect = aSprite.rect;
        var tex = new Texture2D((int)rect.width, (int)rect.height);
        var data = aSprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        tex.SetPixels(data);
        tex.Apply(true);
        return tex;
    }

    private GameObject CreateSpriteGO(int index, Vector3 pos)
    {
        if (spriteList[index] == null)
        {
            Debug.Log("INDEX " + index);
            return null;
        }

        GameObject go = new GameObject();

        go.name = spriteList[index].name;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();

        sr.sprite = spriteList[index];
        sr.sortingLayerName = tileMap.SortingLayer;
        sr.sortingOrder = tileMap.SortingOrderLayer;

        go.AddComponent<BoxCollider2D>();
        go.transform.position = pos;
        go.transform.parent = tileMap.tilesHolder;
        go.transform.localEulerAngles = Vector3.forward * (tileRotate * 90);
        go.transform.localScale = new Vector3(go.transform.localScale.x * tileFilpX, go.transform.localScale.y * tileFilpY, go.transform.localScale.z);
        go.layer = LayerMask.NameToLayer("Tile");

        return go;
    }

}
