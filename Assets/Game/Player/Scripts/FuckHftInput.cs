
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerHftInput : MonoBehaviour
{
    Player player;
    HFTInput hftInput;
    HFTGamepad gamepad;

    public Color baseColor;
    public Transform nameTransform;

    private GUIStyle guiStyle = new GUIStyle();
    private GUIContent guiName = new GUIContent("");
    private Rect nameRect = new Rect(0, 0, 0, 0);
    private string playerName;

    private static int playerNumber = 0;

    int spawnPointNum = 0;



    void Start()
    {
        player = GetComponent<Player>();
        hftInput = GetComponent<HFTInput>();
        gamepad = GetComponent<HFTGamepad>();


        SetColor(playerNumber++);
        SetName(gamepad.Name);

        // Notify us if the name changes.
        gamepad.OnNameChange += ChangeName;

        // Delete ourselves if disconnected
        gamepad.OnDisconnect += Remove;

        if(transform.CompareTag("Test"))
        {
            SetColor(playerNumber);
        }
        else if (LevelSettings.settings.gameMode == LevelSettings.GameMode.Survival || LevelSettings.settings.gameMode == LevelSettings.GameMode.Six)
        {
            SetColor(playerNumber);
            transform.position = LevelSettings.settings.spawnPoints[playerNumber].position;
        }
        else if (LevelSettings.settings.gameMode == LevelSettings.GameMode.Team)
        {
            int teamID = LevelSettings.settings.GetTeamID();
            GetComponent<Player>().playerID = teamID;
            SetColor(teamID);
            spawnPointNum = LevelSettings.settings.GetSpawnPoint(teamID);
            transform.position = LevelSettings.settings.spawnPoints[spawnPointNum * 2 + teamID].position;
        }
        playerNumber++;



        SetName(gamepad.Name);
        gamepad.OnNameChange += ChangeName;

        gamepad.OnDisconnect += Remove;
    }

    void Update()
    {
        Vector2 directionalInput = new Vector2(hftInput.GetAxisRaw("Horizontal"), -hftInput.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (hftInput.GetButtonDown("fire2"))
        {
            player.OnJumpInputDown();
        }
        if (hftInput.GetButtonUp("fire2"))
        {
            player.OnJumpInputUp();
        }

        if (hftInput.GetButtonDown("fire1"))
        {
            player.Attack();
        }
    }

    void SetName(string name)
    {
        playerName = name;
        gameObject.name = "Player-" + playerName;
        guiName = new GUIContent(playerName);
        Vector2 size = guiStyle.CalcSize(guiName);
        nameRect.width = size.x + 12;
        nameRect.height = size.y + 5;
    }

    void ChangeName(object sender, System.EventArgs e)
    {
        SetName(gamepad.Name);
    }

    void SetColor(int colorNdx)
    {
        // Pick a color
        float hueAdjust = (((colorNdx & 0x01) << 5) |
                           ((colorNdx & 0x02) << 3) |
                           ((colorNdx & 0x04) << 1) |
                           ((colorNdx & 0x08) >> 1) |
                           ((colorNdx & 0x10) >> 3) |
                           ((colorNdx & 0x20) >> 5)) / 64.0f;
        float valueAdjust = (colorNdx & 0x20) != 0 ? -0.5f : 0.0f;
        float satAdjust = (colorNdx & 0x10) != 0 ? -0.5f : 0.0f;

        // get the hsva for the baseColor
        Vector4 hsva = HFTColorUtils.ColorToHSVA(baseColor);

        // adjust that base color by the amount we picked
        hsva.x += hueAdjust;
        hsva.y += satAdjust;
        hsva.z += valueAdjust;

        // now get the adjusted color.
        Color playerColor = HFTColorUtils.HSVAToColor(hsva);

        // Tell the gamepad to change color
        gamepad.color = playerColor;

        // Create a 1 pixel texture for the OnGUI code to draw the label
        Color[] pix = new Color[1];
        pix[0] = playerColor;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixels(pix);
        tex.Apply();
        guiStyle.normal.background = tex;

        // Set the HSVA material of the character to the color adjustments.
        //material.SetVector("_HSVAAdjust", new Vector4(hueAdjust, satAdjust, valueAdjust, 0.0f));
    }

    void OnGUI()//    later: world space canvas;
    {
        // If someone knows a better way to do
        // names in Unity3D please tell me!
        Vector2 size = guiStyle.CalcSize(guiName);
        Vector3 coords = Camera.main.WorldToScreenPoint(nameTransform.position);
        nameRect.x = coords.x - size.x * 0.5f - 5f;
        nameRect.y = Screen.height - coords.y;
        guiStyle.normal.textColor = Color.black;
        guiStyle.contentOffset = new Vector2(4, 2);
        GUI.Box(nameRect, playerName, guiStyle);
    }

    void Remove()
    {
        LevelSettings.settings.DisconnectPlayer(GetComponent<Player>().playerID, spawnPointNum);
        playerNumber--;
        Destroy(gameObject);
    }
}
