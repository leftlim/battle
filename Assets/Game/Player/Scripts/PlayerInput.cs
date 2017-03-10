using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;

    //XInput
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    bool isGamepadConnect = false;


    void Start()
    {
        player = GetComponent<Player>();
    }


    void Update()
    {
        if (isGamepadConnect == false)
        {
            switch (player.playerID)
            {
                case 0:
                    Player0KeyInput();
                    break;
                case 1:
                    Player1KeyInput();
                    break;
            }
        }
        else
        {
            playerGamepadInput();
        }
    }

    public void SetGamepadIndex(int index)
    {
        isGamepadConnect = true;
        playerIndex = (PlayerIndex)index;
    }

    void PlayerTestInput()
    {
        float speedX = Mathf.Clamp(Input.GetAxisRaw("L_Horizontal") + state.ThumbSticks.Left.X, -1, 1);
        float speedY = Mathf.Clamp(Input.GetAxisRaw("L_Vertical") + state.ThumbSticks.Left.Y, -1, 1);
        player.SetDirectionalInput(new Vector2(speedX, speedY));

        bool padInput = prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed;
        if (Input.GetKeyDown(KeyCode.Alpha1) || padInput)
        {
            player.OnJumpInputDown();
        }
        padInput = prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released;
        if (Input.GetKeyUp(KeyCode.Alpha1) || padInput)
        {
            player.OnJumpInputUp();
        }
        padInput = prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed;
        if (Input.GetKeyDown(KeyCode.BackQuote) || padInput)
        {
            player.Attack();
        }
    }

    void playerGamepadInput()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);

        float speedX = Mathf.Clamp(state.ThumbSticks.Left.X, -1, 1);
        float speedY = Mathf.Clamp(state.ThumbSticks.Left.Y, -1, 1);
        player.SetDirectionalInput(new Vector2(speedX, speedY));

        bool padInput = prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed;
        if (padInput)
        {
            player.OnJumpInputDown();
        }
        padInput = prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released;
        if (padInput)
        {
            player.OnJumpInputUp();
        }
        padInput = prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed;
        if (padInput)
        {
            player.Attack();
        }
    }

    void Player0KeyInput()
    {
        float speedX = Mathf.Clamp(Input.GetAxisRaw("R_Horizontal"), -1, 1);
        float speedY = Mathf.Clamp(Input.GetAxisRaw("R_Vertical"), -1, 1);
        player.SetDirectionalInput(new Vector2(speedX, speedY));

        if (Input.GetKeyDown(KeyCode.Slash))
        {
            player.OnJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.Slash))
        {
            player.OnJumpInputUp();
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            player.Attack();
        }
    }

    void Player1KeyInput()
    {
        float speedX = Mathf.Clamp(Input.GetAxisRaw("L_Horizontal"), -1, 1);
        float speedY = Mathf.Clamp(Input.GetAxisRaw("L_Vertical"), -1, 1);
        player.SetDirectionalInput(new Vector2(speedX, speedY));

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.OnJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            player.OnJumpInputUp();
        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            player.Attack();
        }
    }
}
