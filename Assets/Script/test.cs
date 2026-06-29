using System.Collections;
using UnityEngine;


public class test: MonoBehaviour
{
    [Header("Stones (in jump order)")]
    public Transform[] stones;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;      // how high the arc goes
    public float jumpDuration = 0.6f;  // time to travel between stones
    public float landingOffsetY = 0.5f; // sphere radius / offset above stone surface

    [Header("Lava Settings")]
    public float lavaY = -5f;          // y-position below which sphere is "in lava"

    private int currentStoneIndex = 0;
    private bool isJumping = false;
    private bool isDead = false;

    void Start()
    {
        if (stones.Length > 0)
        {
            // Snap sphere to the first stone at the start
            PlaceOnStone(0);
        }
    }

    void Update()
    {
        if (isDead) return;

        // Check if fallen into lava
        if (transform.position.y < lavaY)
        {
            Die();
            return;
        }

        // Press Space to jump to the next stone
        if (!isJumping && Input.GetKeyDown(KeyCode.Space))
        {
            TryJumpToNextStone();
        }
    }

    void TryJumpToNextStone()
    {
        int nextIndex = currentStoneIndex + 1;

        if (nextIndex >= stones.Length)
        {
            Debug.Log("No more stones to jump to!");
            return;
        }

        StartCoroutine(JumpArc(stones[currentStoneIndex].position, GetLandingPosition(stones[nextIndex])));
        currentStoneIndex = nextIndex;
    }

    IEnumerator JumpArc(Vector3 startPos, Vector3 landingPos)
    {
        isJumping = true;

        Vector3 start = transform.position;
        Vector3 end = landingPos;

        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            // Linear interpolation for X and Z
            Vector3 currentPos = Vector3.Lerp(start, end, t);

            // Parabolic arc for Y using a sine curve
            float arc = jumpHeight * Mathf.Sin(Mathf.PI * t);
            currentPos.y += arc;

            transform.position = currentPos;

            yield return null;
        }

        transform.position = end;
        isJumping = false;
    }

    void PlaceOnStone(int index)
    {
        transform.position = GetLandingPosition(stones[index]);
        currentStoneIndex = index;
    }

    // Calculates the correct landing spot ON TOP of a stone,
    // using its actual rendered height instead of a guessed offset.
    Vector3 GetLandingPosition(Transform stone)
    {
        float stoneTopY = stone.position.y;
        Renderer rend = stone.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            stoneTopY = rend.bounds.max.y; // top surface of the stone's actual mesh
        }

        float sphereRadius = 0.5f;
        SphereCollider sc = GetComponent<SphereCollider>();
        if (sc != null)
        {
            sphereRadius = sc.radius * transform.localScale.x;
        }

        Vector3 pos = stone.position;
        pos.y = stoneTopY + sphereRadius;
        return pos;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Sphere fell into the lava! Game Over.");

        // Option 1: Respawn at first stone after a short delay
        StartCoroutine(RespawnAfterDelay(1.5f));

        // Option 2: Or call your own GameOver/GameManager logic here instead:
        // GameManager.Instance.GameOver();
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isDead = false;
        PlaceOnStone(0);
    }
}
