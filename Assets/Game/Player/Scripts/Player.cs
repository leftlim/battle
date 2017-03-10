using UnityEngine;
using System.Collections;

//first spawn point select in input script and playerID
[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    bool isFirst = true;
    bool canMove = true;
    bool isInvincible = false;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    bool onGround = false;
    Animator animator;
    int leftJumpCount = 0;
    int leftAttackCount = 0;

    public bool isTutorialOn = false;


    public Transform nameTransform;
    private Color playerColor = Color.blue;
    private GUIStyle guiStyle = new GUIStyle();
    private GUIContent guiName = new GUIContent("");
    private Rect nameRect = new Rect(0, 0, 0, 0);
    private string playerName = "songjo";


    public int playerID;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        animator = GetComponent<Animator>();

        SetName(playerName);
        SetColor(playerColor);

        canMove = false;
        Respawn();
    }


    void Update()
    {
        if (canMove == false)
            return; //below lines are running not attacking

        CalculateVelocity();
        //HandleWallSliding ();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (!onGround && controller.collisions.below)
        {
            onGround = true;
            animator.SetBool("ground", onGround);
            leftJumpCount = 2;
            leftAttackCount = 1;
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else {
                velocity.y = 0;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.CompareTag("Test"))
            return;

        if (collision.CompareTag("Dump") && isInvincible == false && canMove == true)
        {
            int attackerID = collision.transform.parent.GetComponent<Player>().playerID;
            if (playerID != attackerID)
            {
                canMove = false;
                animator.SetTrigger("die");
                Invoke("Respawn", 1.5f);
                LevelSettings.settings.SetScoreOneOnOne(1, attackerID);
            }
        }
    }

    public void WearHat(Transform hat)
    {
        transform.parent = hat;
        transform.localPosition = Vector3.zero;
        canMove = false;
    }


    public void Respawn()
    {
        if (isFirst == false)
        {
            transform.position = LevelSettings.settings.GetRandomSpawnPoint();
        }
        else
            isFirst = false;

        isInvincible = true;
        animator.SetTrigger("respawn");

    }

    void BubbleIdleStart()
    {
        animator.SetTrigger("bubble_idle_start");
        canMove = true;

        Invoke("SetNormal", 3f);
    }

    void SetNormal()
    {

        animator.SetTrigger("set_normal");
    }

    void SetNotInvincible()//  animator control this method : on respawn animatopn
    {
        isInvincible = false;
    }

    void AttackEnd()//  animator control this method
    {
        canMove = true;
        if (onGround)
            leftAttackCount = 1;
    }

    public void Attack()
    {
        if (leftAttackCount <= 0 || directionalInput.sqrMagnitude < 0.1f || canMove == false)
            return;
        if (Mathf.Abs(directionalInput.x) > Mathf.Abs(directionalInput.y))
        {
            animator.SetInteger("attack_dir", 0);
        }
        else if (directionalInput.y > 0)
        {
            animator.SetInteger("attack_dir", 1);
        }
        else
        {
            animator.SetInteger("attack_dir", 2);
        }

        if (onGround && animator.GetInteger("attack_dir") == 2)
            return;

        canMove = false;
        leftAttackCount--;
        velocity = Vector3.zero;
        animator.SetTrigger("attack");
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
        if (canMove && input.x != 0 && (input.x > 0 ^ transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        animator.SetBool("move", (Mathf.Abs(input.x) > 0.1f));
    }

    public void OnJumpInputDown()
    {
        if (leftJumpCount > 0 && canMove)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                { // not jumping against max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else {
                onGround = false;
                animator.SetBool("ground", onGround);
                animator.SetTrigger("jump");
                leftJumpCount--;
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }


    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = Mathf.Sign(directionalInput.x) * ((Mathf.Abs(directionalInput.x) > 0.1) ? 1 : 0) * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    public void SetColor(Color color)
    {
        playerColor = color;
        playerColor.a = 0.4f;

        Color[] pix = new Color[1];
        pix[0] = playerColor;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixels(pix);
        tex.Apply();
        guiStyle.normal.background = tex;
    }

    public void SetName(string name)
    {
        playerName = name;
        gameObject.name = "Player-" + playerName;
        guiName = new GUIContent(playerName);
        Vector2 size = guiStyle.CalcSize(guiName);
        nameRect.width = size.x + 12;
        nameRect.height = size.y + 5;
    }

    void OnGUI()
    {
        Vector2 size = guiStyle.CalcSize(guiName);
        Vector3 coords = Camera.main.WorldToScreenPoint(nameTransform.position);
        nameRect.x = coords.x - size.x * 0.5f - 5f;
        nameRect.y = Screen.height - coords.y;
        guiStyle.normal.textColor = Color.white;
        guiStyle.contentOffset = new Vector2(4, 2);
        GUI.Box(nameRect, playerName, guiStyle);
    }
}
