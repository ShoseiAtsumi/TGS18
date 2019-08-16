using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour {

    public static GameObject cam;    //cameraオブジェクトへの参照を格納する Public 変数

    private Rigidbody2D camRb;       // 剛体

	float Bplayer_y;
    // Use this for initialization
    void Start () {
        camRb = cam.GetComponent<Rigidbody2D>();  // 剛体呼び出し
    }
	
	// Update is called once per frame
	void Update () {

		Bplayer_y = cam.transform.position.y;
		if (Bplayer_y <= 0) {
			Bplayer_y = 0;
		}
        if (CameraMove.PlayerActive == true)
        {
			transform.position = new Vector3(cam.transform.position.x, Bplayer_y, 1);
        }
        else
        {
            Move();
        }
    }

    public void Move()
    {
		iTween.MoveTo(this.gameObject, iTween.Hash("x", cam.transform.position.x, "y", Bplayer_y, "time", 1.0f));
    }
}

