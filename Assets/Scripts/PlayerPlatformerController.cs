using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool is_Shooting = false; // PlayerPlatformerController 안에만 존재


    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    

    protected override void ComputeVelocity()
    {
        if (transform.position.y < -10f)
        {
            // "죽었다"고 판단하고 'GameManager'를 찾아 PlayerDied() 함수를 호출합니다.
            FindObjectOfType<GameManager>().PlayerDied();
            // 리스폰될 것이므로, 아래 이동 로직은 더 이상 실행할 필요가 없습니다.
            return;
        }
        
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis ("Horizontal");

        if (Input.GetKeyDown(KeyCode.W) && grounded)
        {
            StartCoroutine(Jump());

        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        
        // 🟩 스페이스바로 shoot 제어 (bool)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            is_Shooting = true;
            // animator.SetBool("shoot", true);
            animator.SetTrigger("shootCam");
            ShootRaycast();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            is_Shooting = false;
            // animator.SetBool("shoot", false);
        }

        if(move.x > 0.01f)
        {
            if(spriteRenderer.flipX == true)
            {
                spriteRenderer.flipX = false;
            }
        } 
        else if (move.x < -0.01f)
        {
            if(spriteRenderer.flipX == false)
            {
                spriteRenderer.flipX = true;
            }
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat ("velocityX", Mathf.Abs (velocity.x) / maxSpeed);
        animator.SetFloat("velocityY", velocity.y / maxSpeed);

        targetVelocity = move * maxSpeed;
    }
    IEnumerator Jump()
    {
        yield return new WaitForSeconds(0.1f);
        velocity.y = jumpTakeOffSpeed;
    }

    // [추가] 다른 오브젝트의 '트리거' 영역에 들어갔을 때 호출되는 함수입니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 부딪힌 오브젝트(other)의 태그가 "Enemy"인지 확인합니다.
        if (other.CompareTag("Enemy"))
        {
            // 2. 맞다면, GameManager를 찾아 PlayerDied() 함수를 즉시 호출합니다.
            FindObjectOfType<GameManager>().PlayerDied();
        }
        // 2. [추가] 부딪힌 게 "Finish" 태그인지 확인
        else if (other.CompareTag("Finish"))
        {
            // GameManager의 LevelComplete 함수를 호출
            FindObjectOfType<GameManager>().LevelComplete();
        }
    }

    void ShootRaycast()
    {
        Debug.Log("ShootRay");
        // 플레이어가 바라보는 방향 설정
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Debug.DrawRay(transform.position, direction * 1.5f, Color.red, 0.5f);

        // Raycast 쏘기 (거리: 1.5f 정도)
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.5f, LayerMask.GetMask("Obstacle", "Ignore Raycast"));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.5f, LayerMask.GetMask("Enemy", "Ignore Raycast"));
        // Ray가 무언가 맞았다면
        if (hit.collider != null)
        {
            Debug.Log("Hit object: " + hit.collider.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                Beetle beetle = hit.collider.GetComponent<Beetle>();
                if (beetle != null)
                {
                    beetle.StopMovement();
                    hit.collider.tag = "StunnedEnemy"; // 기절
                }

            }
        }
    }
}
