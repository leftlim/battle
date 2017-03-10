using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuckHftInput : MonoBehaviour
{
    //***************HFT*******************//
    // this is the base color of the avatar.
    // we need to know it because we need to know what color
    // the avatar will become after its hsv has been adjusted
    // so we can color the controller and the names above
    // the player.
    public Color baseColor;
    private HFTGamepad gamepad;
    private HFTInput hftInput;
    
    private static int playerNumber = 0;
    //*************************************//
    Player player;
    int spawnPointNum = 0;


    void Start()
    {
        player = GetComponent<Player>();
        gamepad = GetComponent<HFTGamepad>();
        hftInput = GetComponent<HFTInput>();

        switch (LevelSettings.settings.gameMode)
        {
            case LevelSettings.GameMode.OneOnOne:
            case LevelSettings.GameMode.Survival:
            case LevelSettings.GameMode.Six:
                SetColor(playerNumber++);
                player.SetName(gamepad.Name);
                break;

            case LevelSettings.GameMode.Team:
                int teamID = LevelSettings.settings.GetTeamID();
                GetComponent<Player>().playerID = teamID;
                spawnPointNum = LevelSettings.settings.GetSpawnPoint(teamID);
                SetColor(teamID);

                transform.position = LevelSettings.settings.spawnPoints[spawnPointNum * 2 + teamID].position;
                player.SetName(gamepad.Name);
                break;

            default:
                break;
        }

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

    void Remove()
    {
        LevelSettings.settings.DisconnectPlayer(GetComponent<Player>().playerID, spawnPointNum);
        Destroy(gameObject);
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
        player.SetColor(playerColor);

        // Set the HSVA material of the character to the color adjustments.
        //m_material.SetVector("_HSVAAdjust", new Vector4(hueAdjust, satAdjust, valueAdjust, 0.0f));
    }

    void ChangeName(object sender, System.EventArgs e)
    {
        player.SetName(gamepad.Name);
    }
}

