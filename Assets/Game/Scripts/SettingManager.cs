using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class SettingManager : MonoBehaviour
{
    #region fucking singleton
    static private SettingManager s_settings;

    public static SettingManager settings
    {
        get
        {
            return s_settings;
        }
    }

    void Awake()
    {
        if(s_settings != null)
        {
            throw new System.InvalidProgramException("there is more than one level settings object!");
        }
        s_settings = this;
    }

    void Cleanup()
    {
        s_settings = null;
    }

    public void OnDestroy()
    {
        Cleanup();
    }

    public void OnApplicationQuit()
    {
        Cleanup();
    }
    #endregion

    public Player[] playerPrefabs;
    public List<Player> players;
    public FloorInfo[] floorInfos;
    public Vector3[] spawnPoints;
    public InputField playerKey0NameInput;
    public InputField playerKey1NameInput;

    int maxHftNum = 4;
    public InputField maxHftNumInput;

    public string[] randomNames;

    [HideInInspector]
    public int curFloorNum = 0;
    [HideInInspector]
    public bool isTutorialOn = false;
    [HideInInspector]
    public int gameMode = 0; //0:vursus, 1: survibal

    public Text inputSelectText;
    [HideInInspector]
    public int inputIndex = 0;

    public int playerCount = 0;

    public bool canGoDown = false;


    List<GamePad> gamepads;
    bool[] gamepadsConnected = new bool[4];


    public DynamicScrollView gamepadScrollView;

    public HappyFunTimes.PlayerSpawner hftPlayerSpawner;

    public void Start()
    {
        SpawnPlayer(0);
        for (int i = 0; i < 4; i++) {
            gamepadsConnected[i] = false;
        }
    }

    public void Update()
    {
        if(curFloorNum == 3)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (gamepadsConnected[i] == false && testState.IsConnected)
                {
                    gamepadsConnected[i] = true;
                    SpawnPlayer(2, i);
                }
            }
        }

        if(curFloorNum == 1)
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    InputLeftBtn();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow))
                {
                    InputRightBtn();
                }
        }
    }


    #region inputSelectFloor
    void SetInputText()
    {
        switch(inputIndex)
        {
            case 0:
                inputSelectText.text = "Keyboard";
                break;
            case 1:
                inputSelectText.text = "Gamepad";
                break;
            case 2:
                inputSelectText.text = "happy fun times";
                break;
        }
    }
    public void InputLeftBtn()
    {
        inputIndex = (int)Mathf.Repeat(inputIndex - 1, 3);
        SetInputText();
    }
    public void InputRightBtn()
    {
        inputIndex = (int)Mathf.Repeat(inputIndex + 1, 3);
        SetInputText();
    }
    #endregion



    public void SpawnPlayer(int spawnPointIndex, int playerIndex = 0)// 0first, 1key, 2xbox, 3htf
    {
        Player player;
        if(spawnPointIndex <= 2)
             player = Instantiate(playerPrefabs[0], spawnPoints[spawnPointIndex], Quaternion.identity) as Player;
        else
             player = Instantiate(playerPrefabs[1], spawnPoints[spawnPointIndex], Quaternion.identity) as Player;
        players.Add(player);

        player.playerID = playerCount;
        string randomName = randomNames[Random.Range(0, randomNames.Length-1)];
        player.SetName(randomName);

        switch (spawnPointIndex)
        {
            case 0://왼쪽키
                playerKey0NameInput.text = randomName;
                playerKey0NameInput.onEndEdit.AddListener(player.SetName);
                break;
            case 1://오른쪽키
                playerKey1NameInput.text = randomName;
                playerKey1NameInput.onEndEdit.AddListener(player.SetName);
                break;
            case 2://게임패드
                player.GetComponent<PlayerInput>().SetGamepadIndex(playerIndex);
                InputField nameInput = gamepadScrollView.AddNewElement().GetComponent<InputField>();
                nameInput.text = randomName;
                nameInput.onEndEdit.AddListener(player.SetName);
                break;
        }

        playerCount++;
    }

    public void SetMaxHftNumber(string numText)
    {
        maxHftNum = System.Convert.ToInt32(numText);
        hftPlayerSpawner.maxPlayers = maxHftNum;
    }
    
    public void LoadScene(int sceneNum)
    {
        AutoFade.LoadScene(1, 0.3f, 0.3f, Color.black);
    }

    public void SetFloor(int floorNum)
    {
        curFloorNum = floorNum;
        if (curFloorNum == 4)
        {
            maxHftNumInput.text = string.Format("{0}", maxHftNum);
            maxHftNumInput.onEndEdit.AddListener(SetMaxHftNumber);
            hftPlayerSpawner.maxPlayers = maxHftNum;
            hftPlayerSpawner.transform.position = spawnPoints[3];
            hftPlayerSpawner.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {

    }

    [System.Serializable]
    public struct FloorInfo
    {
        public string floorName;
        public float cameraHeight;
        public float liftHeight;
    }
}
