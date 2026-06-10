using System;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SoundManager soundManager;

    [Header("Movement")]
    [SerializeField] private float playerForce = 1f;

    [Header("Buff Setting")]
    [SerializeField] private float jumpBoostDuration = 3f;
    [SerializeField] private float bonusLifeInvincibleDuration = 2f;

    private Rigidbody2D playerRb;
    private Animator playerAnim;
    private SpriteRenderer spriteRenderer;

    private Vector3 inputDir;
    private Vector2 savedVelocity;

    public bool ISDashAvail { get; private set; }
    public bool DidPlayerExit { get; private set; }
    public bool ISDead { get; private set; }
    public bool ISTempGround { get; set; }

    private bool bonusLife;
    private bool isLifeUsed;
    private bool isInvincibleAfterLife;
    private bool jumpBoost;

    private float buffTimer;

    private Color jumpBoostColor = new Color(2f, 2f, 0f, 1f);
    private Color invincibleColor = new Color(2f, 0f, 0f, 1f);

    public event Action<bool> OnPlayerDead;

    private void Awake()
    {
        playerAnim = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ResolveReferences();
        ResetPlayerState();
    }

    private void Update()
    {
        UpdateBuffState();
    }

    private void ResolveReferences()
    {
        if (uiManager == null)
            uiManager = UIManager.Instance;

        if (gameManager == null)
            gameManager = GameManager.Instance;
    }

    private void ResetPlayerState()
    {
        ISDashAvail = false;
        DidPlayerExit = false;
        ISDead = false;
        ISTempGround = false;

        bonusLife = false;
        isLifeUsed = false;
        isInvincibleAfterLife = false;
        jumpBoost = false;

        buffTimer = 0f;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    private void UpdateBuffState()
    {
        if (jumpBoost)
        {
            UpdateJumpBoost();
            return;
        }

        if (isInvincibleAfterLife)
        {
            UpdateLifeInvincible();
            return;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    private void UpdateJumpBoost()
    {
        buffTimer += Time.deltaTime;

        if (buffTimer <= jumpBoostDuration)
        {
            ISDashAvail = true;
            BlinkColor(jumpBoostColor);
            return;
        }

        EndJumpBoost();
    }

    private void EndJumpBoost()
    {
        jumpBoost = false;
        buffTimer = 0f;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    private void UpdateLifeInvincible()
    {
        buffTimer += Time.deltaTime;

        if (buffTimer <= bonusLifeInvincibleDuration)
        {
            BlinkColor(invincibleColor);
            return;
        }

        EndLifeInvincible();
    }

    private void EndLifeInvincible()
    {
        isInvincibleAfterLife = false;
        bonusLife = false;
        buffTimer = 0f;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    private void BlinkColor(Color targetColor)
    {
        if (spriteRenderer == null)
            return;

        float t = Mathf.PingPong(buffTimer * 2f, 1f);
        spriteRenderer.color = Color.Lerp(Color.white, targetColor, t);
    }

    public void InputJoyStick(float x, float y)
    {
        if (!CanDash())
            return;

        inputDir = new Vector3(x, y, 0f);

        if (inputDir == Vector3.zero)
            return;

        PlayDashAnimation();
        PlayDashSound();
        FlipByInputX();
        Dash();
    }

    private bool CanDash()
    {
        if (ISDead)
            return false;

        if (gameManager != null && gameManager.IsGameOver)
            return false;

        return ISDashAvail;
    }

    private void Dash()
    {
        Vector3 velocity = inputDir * playerForce;

        playerRb.linearVelocity = Vector2.zero;

        playerRb.AddForceX(velocity.x * 0.5f, ForceMode2D.Impulse);
        playerRb.AddForceY(velocity.y, ForceMode2D.Impulse);

        if (!jumpBoost)
            ISDashAvail = false;
        else
            ISDashAvail = true;
    }

    private void PlayDashAnimation()
    {
        if (playerAnim != null)
            playerAnim.SetTrigger("Dash");
    }

    private void PlayDashSound()
    {
        if (soundManager != null)
            soundManager.EffectSoundPlay("Dash");
    }

    private void FlipByInputX()
    {
        if (inputDir.x == 0)
            return;

        int direction = inputDir.x > 0 ? 1 : -1;
        transform.localScale = new Vector3(direction, 1f, 1f);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("TempGround"))
        {
            OnLand();
            return;
        }

        if (other.gameObject.CompareTag("DeadZone"))
        {
            OnEnterDeadZone();
        }
    }

    private void OnLand()
    {
        ISDashAvail = true;
        playerRb.AddForce(Vector2.zero);
    }

    private void OnEnterDeadZone()
    {
        if (CanUseBonusLife())
        {
            UseBonusLife();
            return;
        }

        Die();
    }

    private bool CanUseBonusLife()
    {
        return bonusLife && !isLifeUsed;
    }

    private void UseBonusLife()
    {
        playerRb.linearVelocity = Vector2.zero;

        float addX = transform.position.x >= 0 ? 2f : -2f;
        playerRb.AddForce(new Vector2(addX, 15f), ForceMode2D.Impulse);

        buffTimer = 0f;

        isLifeUsed = true;
        isInvincibleAfterLife = true;
        jumpBoost = false;
        ISDashAvail = true;

        if (uiManager != null)
            uiManager.ActiveExtraLife(false);

        if (soundManager != null)
            soundManager.EffectSoundPlay("ExtraLifeUse");
    }

    private void Die()
    {
        if (ISDead)
            return;

        ISDead = true;
        ISDashAvail = false;

        OnPlayerDead?.Invoke(true);

        if (soundManager != null)
        {
            soundManager.EffectSoundPlay("GameOver");
            soundManager.BGMSoundPlay("GameOverBGM");
        }

        gameObject.SetActive(false);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Ground"))
            return;

        ISDashAvail = false;
        DidPlayerExit = true;

        if (gameManager != null)
            gameManager.SetGameStarted(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Box"))
            return;

        Block block = other.gameObject.GetComponent<Block>();

        if (block == null)
            return;

        BoxCollider2D boxCollider = block.GetComponent<BoxCollider2D>();

        if (boxCollider != null)
            boxCollider.enabled = false;

        ApplyBlockItem(block);
        HitBlock(block);
    }

    private void ApplyBlockItem(Block block)
    {
        switch (block.itemIndex)
        {
            case Block.ItemIndex.Jump:
                StartJumpBoost();
                break;

            case Block.ItemIndex.Life:
                GainBonusLife();
                break;

            case Block.ItemIndex.Ground:
                RequestTempGround();
                break;
        }
    }

    private void StartJumpBoost()
    {
        buffTimer = 0f;
        jumpBoost = true;

        if (soundManager != null)
            soundManager.EffectSoundPlay("JumpBoost");
    }

    private void GainBonusLife()
    {
        if (isLifeUsed)
            return;

        bonusLife = true;

        if (uiManager != null)
            uiManager.ActiveExtraLife(true);

        if (soundManager != null)
            soundManager.EffectSoundPlay("ExtraLifeGain");
    }

    private void RequestTempGround()
    {
        if (gameManager != null)
            gameManager.CreateTempGround();
    }

    private void HitBlock(Block block)
    {
        block.OnHitByPlayer();

        ISDashAvail = true;
        inputDir = Vector3.zero;
        playerRb.AddForce(Vector2.zero);

        if (playerAnim != null)
            playerAnim.SetTrigger("AttWorked");
    }

    public void PausePlayer()
    {
        if (gameManager != null && !gameManager.IsGameStarted)
            return;

        savedVelocity = playerRb.linearVelocity;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.simulated = false;
    }

    public void ResumePlayer()
    {
        if (gameManager != null && !gameManager.IsGameStarted)
            return;

        playerRb.simulated = true;
        playerRb.linearVelocity = savedVelocity;
    }
}