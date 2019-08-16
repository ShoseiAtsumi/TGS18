using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class S_Player_3 : MonoBehaviour
{

    // オブジェクトに付随するコンポーネント
    Rigidbody2D rb;             // 物理剛体(物理空間でどんな動きをするか(重力、摩擦、反射)
    Animator animator;          // オブジェクトのアニメーション管理

    float jumpForce = 350f;       // ジャンプ時に加える力
    float jumpThreshold = 0.3f;  // ジャンプ中か判定するための閾値
    float runForce = 7.0f;       // 走り始めに加える力
    float runSpeed = 2.0f;       // 走っている間の速度
    float runThreshold = 2.2f;   // 速度切り替え判定のための閾値
    float XSize;                 // localScaleを変更した際の保存先
    bool isGround = true;        // 地面と接地しているか管理するフラグ
    int key = 0;                 // 左右の入力管理
    public static bool Active = false;

    string state;                // プレイヤーの状態管理
    string prevState;            // 前の状態を保存
    float stateEffect = 1;       // 状態に応じて横移動速度を変えるための係数

    float sensitivity = 0.1f;   // マウスの移動感度


    // Use this for initialization
    void Start()
    {
        Active = false;
        // コンポーネントの取得
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        animator.SetBool("PowerLevel", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            GetInputKey();          // ① 入力を取得
            ChangeState();          // ② 状態を変更する
            ChangeAnimation();      // ③ 状態に応じてアニメーションを変更する
            Move();                 // ④ 入力に応じて移動する
            if (transform.position.y < -10)
            {
                Loadstageselect();
            }
        }
        else
        {
            NonActive();
        }
    }

    void GetInputKey()
    {

        key = 0;
        // 右方向入力
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            key = 1;
        }
        // 左方向入力
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            key = -1;
        }
        /* マウス左クリック
        if (Input.GetMouseButton(0))
        {
            key = -1;
        }
        // マウス右クリック
        if (Input.GetMouseButton(1))
        {
            key = 1;
        }*/
    }

    void ChangeState()
    {
        // 空中にいるかどうかの判定。上下の速度(rigidbody.velocity)が一定の値を超えている場合、空中とみなす
        if (Mathf.Abs(rb.velocity.y) > jumpThreshold)
        {
            isGround = false;
        }

        // 接地している場合
        if (isGround)
        {
            // 走行中
            if (key != 0)
            {
                state = "RUN";
                //待機状態
            }
            else
            {
                state = "IDLE";
            }
            // 空中にいる場合
        }
        else
        {
            // 上昇中
            if (rb.velocity.y > 0)
            {
                state = "JUMP";
                // 下降中
            }
            else if (rb.velocity.y < 0)
            {
                state = "FALL";
            }
        }
    }

    void ChangeAnimation()
    {
        // 状態が変わった場合のみアニメーションを変更する
        // Animatorで設定されているアニメのフラグを切り替えてアニメの再生を管理する
        if (prevState != state)
        {
            switch (state)
            {
                case "JUMP":
                    animator.SetBool("isJump", true);
                    animator.SetBool("isFall", false);
                    animator.SetBool("isRun", false);
                    animator.SetBool("isIdle", false);
                    stateEffect = 0.5f;
                    break;
                case "FALL":
                    animator.SetBool("isFall", true);
                    animator.SetBool("isJump", false);
                    animator.SetBool("isRun", false);
                    animator.SetBool("isIdle", false);
                    stateEffect = 0.5f;
                    break;
                case "RUN":
                    animator.SetBool("isRun", true);
                    animator.SetBool("isFall", false);
                    animator.SetBool("isJump", false);
                    animator.SetBool("isIdle", false);
                    stateEffect = 1f;
                    //GetComponent<SpriteRenderer> ().flipX = true;
                    break;
                default:    // ジャンプ、落下、走るアニメ以外の状態は立ち(アイドル)状態にする
                    animator.SetBool("isIdle", true);
                    animator.SetBool("isFall", false);
                    animator.SetBool("isRun", false);
                    animator.SetBool("isJump", false);
                    stateEffect = 1f;
                    break;
            }
            // 状態の変更を判定するために状態を保存しておく
            prevState = state;
        }
        switch (state)
        {
            case "RUN":
                transform.localScale = new Vector3(key, 1, 1); // 向きに応じてキャラクターのspriteを反転
                break;
        }
    }

    void Move()
    {
        // 設置している時にSpaceキー押下でジャンプ
        if (isGround)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                this.rb.AddForce(transform.up * this.jumpForce);
                isGround = false;
            }

            // マウスの移動量を取得
            float mouse_move_x = Input.GetAxis("Mouse X") * sensitivity;
            float mouse_move_y = Input.GetAxis("Mouse Y") * sensitivity;

            /* 上方向にマウスをフリックしたら
            if (mouse_move_y > 0.1f)
            {
                this.rb.AddForce(transform.up * this.jumpForce);
                isGround = false;
            }*/

        }

        // 左右の移動。一定の速度に達するまではAddforceで力を加え、それ以降はtransform.positionを直接書き換えて同一速度で移動する
        float speedX = Mathf.Abs(this.rb.velocity.x);
        if (speedX < this.runThreshold)
        {
            this.rb.AddForce(transform.right * key * this.runForce * stateEffect); //未入力の場合は key の値が0になるため移動しない
        }
        else
        {
            this.transform.position += new Vector3(runSpeed * Time.deltaTime * key * stateEffect, 0, 0);
        }

    }

    //キャラが切り替えできる距離かの判定および切り替え
    void OnTriggerStay2D(Collider2D col)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (col.gameObject.tag == "p_2")
            {
                CameraMove.Player = this.gameObject; //カメラスクリプトに追従させるキャラにする
                BackGround.cam = this.gameObject;
                Invoke("ActiveOn", 2.0f);
            }
            if (col.gameObject.tag == "p_4")
            {
                animator.SetBool("PowerLevel", false);
                Active = false;                     //操作を受け付けないように
                CameraMove.PlayerActive = false;    //キャラ操作対象が切り替わったためfalse
            }
        }
    }

    //Invoke用
    void ActiveOn()
    {
        Active = true;
        animator.SetBool("PowerLevel", true);
        CameraMove.PlayerActive = true;
    }

    // 着地中判定
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Floor")
        {
            if (!isGround)
                isGround = true;
        }
        // 衝突した面の向いている方向が上向きなら床として判定
        else if (col.contacts[0].normal.y >= 1.0f)
        {
            if (!isGround)
                isGround = true;
        }
    }

    void NonActive()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isFall", false);
        animator.SetBool("isRun", false);
        animator.SetBool("isJump", false);
        stateEffect = 1f;
    }
    void Loadstageselect()
    {
        SceneManager.LoadScene("Stage_select");
    }
}
