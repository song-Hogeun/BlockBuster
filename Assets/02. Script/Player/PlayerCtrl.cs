using System;
using System.Collections;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private UIManager gameManager;
    [SerializeField] private SoundManager soundManager;

    private Rigidbody2D playerRb;
    private Animator playerAnim;
    private SpriteRenderer spriteRenderer;

    private Vector3 inputDir;

    private Vector2 savedVelocity;

    [SerializeField] private float playerForce = 1f;

    public bool ISDashAvail { get;  private set; }
    public bool DidPlayerExit { get;  private set; }
    public bool ISDead { get; private set; }
    public bool ISTempGround { get; set; }

    private bool bonusLife;
    private bool isLifeUsed;
    private bool secondLife;
    private bool jumpBoost;
    private float timer;

    private Color color;


    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ISDashAvail = false;
        DidPlayerExit = false;
        ISDead = false;
        timer = 0;
        
        isLifeUsed = false;
        bonusLife = false;
        jumpBoost = false;
        secondLife = false;
    }

    void Update()
    {
        if (jumpBoost)
        {
            color = new Color(2f, 2f, 0f, 1f);
            timer += Time.deltaTime;

            if (timer <= 3f)
            {
                ISDashAvail = true;
                float t = Mathf.PingPong(timer * 2f, 1f);
                spriteRenderer.color = Color.Lerp(Color.white, color, t);
            }
            else
            {
                if (secondLife)
                {
                    bonusLife = false;
                }
                jumpBoost = false;
                timer = 0;
                spriteRenderer.color = Color.white;
            }
        }
        else if (bonusLife && isLifeUsed)
        {
            secondLife = true;
            color = new Color(2f, 0f, 0f, 1f);
            timer += Time.deltaTime;

            if (timer <= 2f)
            {
                float t = Mathf.PingPong(timer * 2f, 1f);
                spriteRenderer.color = Color.Lerp(Color.white, color, t);
            }
            else
            {
                bonusLife = false;
                timer = 0;
                spriteRenderer.color = Color.white;
            }
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void Dash()
    {
        Vector3 velocity = inputDir * playerForce;

        playerRb.linearVelocity = Vector3.zero;

        playerRb.AddForceX(velocity.x * 0.5f, ForceMode2D.Impulse);
        playerRb.AddForceY(velocity.y, ForceMode2D.Impulse);

        if (!jumpBoost)
        {
            ISDashAvail = false;
        }
        else
        {
            ISDashAvail = true;
        }
    }

    public void InputJoyStick(float x, float y)
    {
        if (!ISDashAvail)
        {
            return;
        }

        inputDir = Vector3.zero;
        playerRb.AddForce(Vector3.zero);

        inputDir = new Vector3(x, y, 0);

        playerAnim.SetTrigger("Dash");
        soundManager.EffectSoundPlay("Dash");

        if (inputDir.x != 0)
        {
            int isXPositive = inputDir.x > 0 ? 1 : -1;
            transform.localScale = new Vector3(isXPositive, 1, 1);
        }

        Dash();
    }

    void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("TempGround"))
        {
            ISDashAvail = true;
            playerRb.AddForce(Vector3.zero);
        }
        if (other.gameObject.CompareTag("DeadZone"))
        {
            if (bonusLife && !isLifeUsed)
            {
                playerRb.linearVelocity = Vector3.zero;
                float addX = transform.position.x >= 0 ? 2 : -2;
                
                playerRb.AddForce(new Vector2(addX, 15f), ForceMode2D.Impulse);
                
                timer = 0;
                isLifeUsed = true;
                ISDashAvail = true;
                jumpBoost = false;

                gameManager.ActiveExtraLife(false);
                soundManager.EffectSoundPlay("ExtraLifeUse");
            }
            else
            {
                ISDead = true;
                gameManager.IsGameOver = true;

                gameObject.SetActive(false);
                soundManager.EffectSoundPlay("GameOver");
                soundManager.BGMSoundPlay("GameOverBGM");
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            ISDashAvail = false;
            DidPlayerExit = true;

            gameManager.SetGameStarted(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            Block block = other.gameObject.GetComponent<Block>();
            block.GetComponent<BoxCollider2D>().enabled = false;

            if (block != null)
            {
                if (block.itemIndex == Block.ItemIndex.Jump)
                {
                    timer = 0;
                    jumpBoost = true;
                    soundManager.EffectSoundPlay("JumpBoost");
                }
                else if (block.itemIndex == Block.ItemIndex.Life && !isLifeUsed)
                {
                    bonusLife = true;
                    gameManager.ActiveExtraLife(bonusLife);
                    soundManager.EffectSoundPlay("ExtraLifeGain");
                }
                else if (block.itemIndex == Block.ItemIndex.Ground)
                {
                    gameManager.GroundItem();
                }

                    block.OnHitByPlayer();
            }

            ISDashAvail = true;
            inputDir = Vector3.zero;
            playerRb.AddForce(Vector3.zero);
            playerAnim.SetTrigger("AttWorked");
        }
    }

    public void PausePlayer()
    {
        if (!gameManager.IsGameStarted)
        {
            return;
        }
        savedVelocity = playerRb.linearVelocity; // 현재 속도 저장
        playerRb.linearVelocity = Vector2.zero;  // 속도 정지
        playerRb.simulated = false;
    }

    public void ResumePlayer()
    {
        if (!gameManager.IsGameStarted)
        {
            return;
        }
        playerRb.simulated = true;
        playerRb.linearVelocity = savedVelocity; // 저장한 속도로 복구
    }
}
