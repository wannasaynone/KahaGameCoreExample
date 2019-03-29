using KahaGameCore.View;
using UnityEngine;

public class UFOController : View
{
    [SerializeField] private float m_moveSpeed = 0f;
    [SerializeField] private Rigidbody2D m_rigidBody = null;

    private void FixedUpdate()
    {
        float _moveHorizontal = Input.GetAxis("Horizontal");
        float _moveVertical = Input.GetAxis("Vertical");
        Vector2 _movement = new Vector2(_moveHorizontal, _moveVertical);
        m_rigidBody.AddForce(_movement * m_moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Bump>() != null)
        {
            collision.gameObject.SetActive(false); // 用關閉的而不使用摧毀，是為了讓這個團塊可以繼續被物件池管理器利用
            ScoreManager.Instance.AddScore(); // 碰撞到團塊時加一分
            BumpManager.Instance.SpawnBump(); // 碰撞到團塊時生產一個新的團塊
        }
    }
}
