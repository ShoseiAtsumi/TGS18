using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
	public static GameObject Player;    //プレイヤーゲームオブジェクトへの参照を格納する Public 変数

    private Rigidbody2D PlayerRb;       // 剛体

    public static bool PlayerActive = true;    //キャラクターの操作対象が切り替わったかの判定

	float player_y;

    // Use this for initialization
    void Start()
	{
        PlayerActive = true;
        PlayerRb = Player.GetComponent<Rigidbody2D>();  // 剛体呼び出し
    }

    //Update is called once per frame
	void Update()
	{

		player_y = Player.transform.position.y;
		if (player_y <= 0) {
			player_y = 0;
		}
        if (PlayerActive == true )
        {
            // カメラをプレイヤーのいる位置を視点として等速横移動する
			transform.position = new Vector3(Player.transform.position.x, player_y, -10);
            //BackGround.cam = Player;
        }
        else
        {
            Move();
        }
	}

    //キャラクター切り替え時にカメラの動きを滑らかに
    public void Move()
    {
		iTween.MoveTo(this.gameObject, iTween.Hash("x", Player.transform.position.x,"y", player_y, "time", 1.0f));
    }
}
