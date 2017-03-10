using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LiftCtrl : MonoBehaviour 
{
    bool isPlayerOn = false;
    bool isLifting = false;

    public GameObject firstHelpText;
    public Transform lift;
    public Transform camera;
    public float moveSpeed = 1;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerOn == false && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();
            player.WearHat(transform);
            isPlayerOn = true;

            firstHelpText.SetActive(true);
        }
    }

    public void Update()
    {
        if (isPlayerOn == false) return;

        if(Input.GetKeyDown(KeyCode.Return) && isLifting == false)
        {
            switch(SettingManager.settings.curFloorNum)
            {
                case 0:
                    NextFloor(1);
                    break;
                case 1:
                    if(SettingManager.settings.inputIndex == 0)
                    {
                        NextFloor(2);
                    }
                    else if (SettingManager.settings.inputIndex == 1)
                    {
                        NextFloor(3);
                    }
                    else if (SettingManager.settings.inputIndex == 2)
                    {
                        NextFloor(4);
                    }
                    break;
                case 2:
                case 3:
                case 4:
                    if (SettingManager.settings.isTutorialOn)
                    {
                        NextFloor(5);
                    }
                    else
                    {
                        NextFloor(7);
                    }
                    break;
                case 5://tutorial jump
                    NextFloor(6);
                    break;
                case 6://tutorial burp
                    NextFloor(7);
                    break;
                case 7://select mode
                    if(SettingManager.settings.gameMode == 0)
                    {
                        NextFloor(8);
                    }
                    else if (SettingManager.settings.gameMode == 1)
                    {
                        NextFloor(9);
                    }
                    break;
                case 8:
                    NextFloor(9);
                    break;
                case 9:
                    SettingManager.settings.StartGame();
                    break;


            }
            
        }
    }

    void NextFloor(int nextFloorNum)
    {
        isLifting = true;
        float liftMoveTime = SettingManager.settings.floorInfos[SettingManager.settings.curFloorNum].liftHeight
                - SettingManager.settings.floorInfos[nextFloorNum].liftHeight;
        //liftMoveTime = Mathf.Abs(liftMoveTime) / 13f;
        //liftMoveTime *= 0.25f;
        //liftMoveTime = 1;

        lift.DOMoveY(SettingManager.settings.floorInfos[nextFloorNum].liftHeight, liftMoveTime);
        camera.DOMoveY(SettingManager.settings.floorInfos[nextFloorNum].cameraHeight, liftMoveTime).OnComplete(LiftingEnd);
        SettingManager.settings.SetFloor(nextFloorNum);
    }

    void LiftingEnd()
    {
        isLifting = false;
        if(SettingManager.settings.curFloorNum == 2)
        {
            SettingManager.settings.SpawnPlayer(1);
        }
        else if(SettingManager.settings.curFloorNum == 2)
        {

        }
        else if (SettingManager.settings.curFloorNum == 3)
        {

        }
    }
}
