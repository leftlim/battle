using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class LevelSettings : MonoBehaviour
{
    public enum GameMode { OneOnOne, Team, Six, Survival};
    public GameMode gameMode;

    public Transform bottomOfLevel;
    public Transform[] spawnPoints;

    public Text scoreText;

    static private LevelSettings s_settings;

    int[] scores = new int[6];

    int[] teamNums = new int[2];

    bool[,] usingSpawnPoint = new bool[2,3];
    

    public void DisconnectPlayer(int teamID, int spawnPointNum)
    {
        teamNums[teamID]--;
        usingSpawnPoint[teamID, spawnPointNum] = false;
    }
    public int GetTeamID()
    {
        if (teamNums[0] <= teamNums[1])
            teamNums[0]++;
        else if (teamNums[0] > teamNums[1])
            teamNums[1]++;
        return (teamNums[0] <= teamNums[1]) ? 0 : 1;
    }

    public int GetSpawnPoint(int teamID)
    {
        int arrIdx = 0;
        if (teamID == 0)//   0, 2, 4...
        {
            for (int i = 0; usingSpawnPoint[teamID, arrIdx] == true && i < 3; i++)
            {
                arrIdx = i;
            }
        }
        else if (teamID == 1)//   1, 3, 5...
        {
            for (int i = 1; usingSpawnPoint[teamID, arrIdx] == true && i < 6; i++)
            {
                arrIdx = i;
            }
        }
        usingSpawnPoint[teamID, arrIdx] = true;
        return arrIdx;
    }

    public void SetScoreOneOnOne(int plusScore, int playerNum)
    {
        scores[playerNum] += plusScore;
        scoreText.text = scores[0].ToString() + " : " + scores[1].ToString();
    }

    public Vector3 GetRandomSpawnPoint()
    {
        int ndx = Random.Range(0, spawnPoints.Length - 1);
        return spawnPoints[ndx].localPosition;
    }

    public Vector3 GetRandomSideSpawnPoint(bool isLeft, int idx)
    {
        int ndx = Random.Range(0, spawnPoints.Length - 1);
        return spawnPoints[ndx].localPosition;
    }

    public static LevelSettings settings
    {
        get
        {
            return s_settings;
        }
    }
    void Awake()
    {
        if (s_settings != null)
        {
            throw new System.InvalidProgramException("there is more than one level settings object!");
        }
        s_settings = this;
    }
    void Cleanup()
    {
        s_settings = null;
    }

    void OnDestroy()
    {
        Cleanup();
    }
    void OnApplicationExit()
    {
        Cleanup();
    }
}
