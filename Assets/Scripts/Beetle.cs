using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : MonoBehaviour
{
    public float xVelocity = 3f;
    Rigidbody2D beetleRb;
    SpriteRenderer spriteRend;
    public float castDist = 0.2f;
    Vector2 castDir;
    private bool isStopped = false;

    void Start()
    {
        beetleRb = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        beetleRb.velocity = new Vector2 (xVelocity, 0);
        castDir = Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopped)
        {
            beetleRb.velocity = Vector2.zero; // 계속 0으로 유지
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir, castDist);
        if (hit.collider != null && (hit.collider.tag != ("Player")))
        {
            spriteRend.transform.localScale = new Vector3(-spriteRend.transform.localScale.x, 1, 1);
            beetleRb.velocity *= -1;
            castDir.x *= -1;
        }
    }

    public void StopMovement()
    {
        isStopped = true;
        beetleRb.velocity = Vector2.zero;
        // 1. 태그 변경 (이미 하셨겠지만 확실하게)
        this.tag = "StunnedEnemy"; 

        // [핵심 추가] 2. 레이어를 'Obstacle'로 변경
        // 이제부터 이 녀석은 '적'이 아니라 '벽/땅' 취급을 받습니다.
        this.gameObject.layer = LayerMask.NameToLayer("Obstacle");

        // [핵심 추가] 3. 밟을 수 있게 단단한 물체로 변신
        // 혹시 IsTrigger가 켜져 있었다면 끄고, 일반 콜라이더로 만듭니다.
        GetComponent<BoxCollider2D>().isTrigger = false;
    }


}
