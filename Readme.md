# KahaGameCore Example
這是一個用來示範如何使用KahaGameCore開發遊戲的簡易範例。  
在這個遊戲中，包含以下規則：  
```
- 遊戲開始30秒後結束
- 每吃掉一個黃色團塊，獲得一分，並在隨機位置產生新的素材
- 遊戲結束後結算玩家分數，並顯示出來
```  

KahaGameCore：https://github.com/wannasaynone/KahaGameCore
## 範例
1. 首先至官方下載相關專案，本次示範只須要使用相關美術素材，因此只輸入Sprite內的圖片素材。  
網址：https://assetstore.unity.com/packages/essentials/tutorial-projects/2d-ufo-tutorial-52143  
2. 完成場景布置：背景圖片設置、碰撞牆。  
![alt text](https://raw.githubusercontent.com/wannasaynone/KahaGameCoreExample/master/README/1.png)
3. 參考官方的教學，製作UFO的控制器。  
```C#
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
}
```
4. 製作黃色團塊，但因為它目前沒有任何功能，所以先保持空內容。  
```C#
using KahaGameCore.View;

public class Bump : View
{

}
```  
5. 製作當UFO碰撞到團塊時的邏輯，我們分別須要：加一分、消滅團塊、製造新的團塊，這代表我們至少需要一個記分器。此外考慮到團塊的產生應有範圍、數量限制，這代表我們也需要一個團塊管理器處理產生邏輯。 
```C#
using KahaGameCore.Manager;

public class ScoreManager : Manager
{
    public static ScoreManager Instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = new ScoreManager();
            }
            return m_instance;
        }
    }
    private static ScoreManager m_instance = null;

    public int Score { get { return m_currentScore; } }
    private int m_currentScore = 0;

    public void AddScore()
    {
        m_currentScore++;
    }

    public void ResetScroe()
    {
        m_currentScore = 0;
    }
}
```  
```C#
using KahaGameCore.Manager;
using KahaGameCore;
using UnityEngine;

public class BumpManager : Manager
{
    public static BumpManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new BumpManager();
            }
            return m_instance;
        }
    }
    private static BumpManager m_instance = null;

    public void SpawnBump()
    {
        // 用物件池管理器取得團塊的複製品
        Bump _clone = GameObjectPoolManager.GetUseableObject<Bump>("Bump");
        // 這裡先寫死，可以透過GameDataManager.LoadGameData(path)的方式取得靜態資料填入
        // 也可以透過GameResourceManager.LoadResource(path)的方式取得ScriptableObject取得靜態資料填入
        _clone.transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        _clone.transform.localScale = Vector3.one;
    }
}

```  
6. 在UFOController中追加碰撞相關的邏輯。  
```C#
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Bump>() != null)
        {
            collision.gameObject.SetActive(false); // 用關閉的而不使用摧毀，是為了讓這個團塊可以繼續被物件池管理器利用
            ScoreManager.Instance.AddScore(); // 碰撞到團塊時加一分
            BumpManager.Instance.SpawnBump(); // 碰撞到團塊時生產一個新的團塊
        }
    }
```  
接著在Bump和UFO上分別添加Collider2D，並製作成Prefab備用。  
![alt text](https://raw.githubusercontent.com/wannasaynone/KahaGameCoreExample/master/README/2.png)![alt text](https://raw.githubusercontent.com/wannasaynone/KahaGameCoreExample/master/README/3.png)  
7. 製作一個遊戲開始用的選單UI，並為它添加UIView。  
![alt text](https://raw.githubusercontent.com/wannasaynone/KahaGameCoreExample/master/README/4.png)
```C#
using KahaGameCore.Manager;
using KahaGameCore.View.UI;
using UnityEngine;

public class StartGameUIPage : UIView
{
    [SerializeField] private RectTransform m_root = null;

    public override bool IsEnabled
    {
        get
        {
            return m_root.gameObject.activeSelf;
        }
    }

    public override void EnablePage(Manager manager, bool enable)
    {
        // 這邊可以透過傳入的manager做為流程保護的鑰匙，藉由檢查傳入的manager是否同一來保證這個UI一次只被一個流程操作
        // 但這個範例沒有這個需求，因此直接做開關處理
        m_root.gameObject.SetActive(enable);
    }
}
```  
8. 在StartGameUIPage內添加供按鈕使用的遊戲開始功能之前，我們需要先撰寫遊戲流程的邏輯才能「開始」。因此先撰寫遊戲邏輯。
```C#
using KahaGameCore.Manager.State;
using KahaGameCore;
using System.Collections.Generic;
using UnityEngine;

public class GameState : StateBase
{
    private float m_gameTimer = 0f;

    // 在這個class被實例化後，被下達Start()時執行
    protected override void OnStart()
    {
        UFOController _player = GameObjectPoolManager.GetUseableObject<UFOController>("UFO"); // 創建玩家
        BumpManager.Instance.SpawnBump(); // 創建第一個團塊
        ScoreManager.Instance.ResetScroe(); // 重設分數
        GetPage<StartGameUIPage>().EnablePage(this, false); // 關閉開始頁面UI
        m_gameTimer = 0f; // 重設計時器
    }

    // 被下達Stop()時執行
    protected override void OnStop()
    {
        GetViews<UFOController>()[0].gameObject.SetActive(false); // 只會有一個玩家控制器，所以用[0]，然後關掉他

        //關閉所有團塊
        List<Bump> _bumps = GetViews<Bump>();
        for(int i = 0; i < _bumps.Count; i++)
        {
            _bumps[i].gameObject.SetActive(false); 
        }

        GetPage<StartGameUIPage>().EnablePage(this, true); // 關閉開始頁面UI
    }

    // 被下達Start()後，每個UnityEngine.Update()時執行
    protected override void OnTick()
    {
        m_gameTimer += Time.deltaTime;
        if(m_gameTimer >= 30f) // 30秒後遊戲結束
        {
            Stop();
        }
    }
}
```  
9. 再回頭於StartGameUIPage內添加供按鈕使用的遊戲開始功能，並指定給Button的OnClick。
```C#
    public void Button_StartGame()
    {
        new GameState().Start();
    }
```  
亦即在場景上不須指定、製作額外的Component當作Entry Point。  
10. 到這邊核心流程就算完成了，接著追加一些遊戲表現，製作提示剩餘時間與目前分數的UI。
```C#
using KahaGameCore.Manager;
using KahaGameCore.View.UI;
using UnityEngine;
using UnityEngine.UI;

public class GamingUIPage : UIView
{
    [SerializeField] private RectTransform m_root = null;
    [SerializeField] private Text m_scoreText = null;
    [SerializeField] private Text m_remainingTimeText = null;

    private Manager m_currentManager = null;

    public override bool IsEnabled
    {
        get
        {
            return m_root.gameObject.activeSelf;
        }
    }

    public override void EnablePage(Manager manager, bool enable)
    {
        if(manager == null)
        {
            // 如果沒有傳入的manager，擋掉後面的流程
            return;
        }

        if (m_currentManager != null && m_currentManager != manager)
        {
            // 如果這個UI正在被使用，而且操作它的人不是傳入的manager，擋掉後面流程
            return;
        }

        if (enable)
        {
            // 有人開啟UI，記錄開啟者是誰
            m_currentManager = manager;
        }
        else
        {
            // 是關閉UI，清空正在使用者
            m_currentManager = null;
        }

        m_root.gameObject.SetActive(enable);
    }

    public void SetScore(int score)
    {
        m_scoreText.text = "分數: " + score.ToString();
    }

    public void SetRemainingTime(float value)
    {
        m_remainingTimeText.text = "剩餘時間: " + value.ToString("00");
    }
}
```  
並回頭修改GameState。
```C#
using KahaGameCore.Manager.State;
using KahaGameCore;
using System.Collections.Generic;
using UnityEngine;

public class GameState : StateBase
{
    private float m_gameTimer = 0f;

    private StartGameUIPage m_startGameUI = null;
    private GamingUIPage m_gamingUI = null;

    // 在這個class被實例化後，被下達Start()時執行
    protected override void OnStart()
    {
        m_startGameUI = GetPage<StartGameUIPage>() as StartGameUIPage;
        m_gamingUI = GetPage<GamingUIPage>() as GamingUIPage;

        UFOController _player = GameObjectPoolManager.GetUseableObject<UFOController>("UFO"); // 創建玩家
        BumpManager.Instance.SpawnBump(); // 創建第一個團塊
        ScoreManager.Instance.ResetScroe(); // 重設分數
        m_startGameUI.EnablePage(this, false); // 關閉開始頁面UI
        m_gamingUI.EnablePage(this, true); // 開啟遊戲UI
        m_gameTimer = 0f; // 重設計時器
    }

    // 被下達Stop()時執行
    protected override void OnStop()
    {
        GetViews<UFOController>()[0].gameObject.SetActive(false); // 只會有一個玩家控制器，所以用[0]，然後關掉他

        //關閉所有團塊
        List<Bump> _bumps = GetViews<Bump>();
        for(int i = 0; i < _bumps.Count; i++)
        {
            _bumps[i].gameObject.SetActive(false); 
        }

        m_startGameUI.EnablePage(this, true); // 開啟開始頁面UI
        m_gamingUI.EnablePage(this, false); // 關閉遊戲UI
    }

    // 被下達Start()後，每個UnityEngine.Update()時執行
    protected override void OnTick()
    {
        m_gameTimer += Time.deltaTime;
        m_gamingUI.SetRemainingTime(30f - m_gameTimer);
        m_gamingUI.SetScore(ScoreManager.Instance.Score);
        if (m_gameTimer >= 30f) // 30秒後遊戲結束
        {
            Stop();
        }
    }
}
```  
11. 但因為目前遊戲結束之後會直接關閉全部東西、跳出「開始遊戲」，我們試著修改之，追加結算流程進去遊戲流程。  
先製作遊戲結束流程和相關UI：
```C#
using KahaGameCore.Manager.State;
using KahaGameCore.View.UI;

public class EndState : StateBase
{
    private UIView m_endUI = null;

    protected override void OnStart()
    {
        m_endUI = GetPage<EndUIPage>();
        m_endUI.EnablePage(this, true);
    }

    protected override void OnStop()
    {
        m_endUI.EnablePage(this, false);
        GetPage<StartGameUIPage>().EnablePage(this, true);
    }

    protected override void OnTick()
    {
    }
}
```  
```C#
using UnityEngine;
using UnityEngine.UI;
using KahaGameCore.View.UI;
using KahaGameCore.Manager;

public class EndUIPage : UIView
{
    [SerializeField] private RectTransform m_root = null;
    [SerializeField] private Text m_scoreText = null;

    private EndState m_endState = null;

    public override bool IsEnabled
    {
        get
        {
            return m_root.gameObject.activeSelf;
        }
    }

    public override void EnablePage(Manager manager, bool enable)
    {
        if (manager == null)
        {
            // 如果沒有傳入的manager，擋掉後面的流程
            return;
        }

        if (m_endState != null && m_endState != manager)
        {
            // 如果這個UI正在被使用，而且操作它的人不是傳入的manager，擋掉後面流程
            return;
        }

        if (enable)
        {
            // 流程設定必須是EndState才能使用這個UI，先檢查，如果傳入的Manager不是EndState，擋掉後面流程
            m_endState = manager as EndState;
            if(m_endState == null)
            {
                return;
            }

            // 開啟前更新文字資訊
            m_scoreText.text = "得分：" + ScoreManager.Instance.Score;
        }
        else
        {
            // 是關閉UI，清空正在使用者
            m_endState = null;
        }

        m_root.gameObject.SetActive(enable);
    }

    public void Button_ResetGame()
    {
        // 結束流程
        m_endState.Stop();
    }
}
```
12. 將流程接入原本的GameState，簡單修改。  
將GameState內原本的
```C#
    // 被下達Start()後，每個UnityEngine.Update()時執行
    protected override void OnTick()
    {
        m_gameTimer += Time.deltaTime;
        m_gamingUI.SetRemainingTime(30f - m_gameTimer);
        m_gamingUI.SetScore(ScoreManager.Instance.Score);
        if (m_gameTimer >= 30f) // 30秒後遊戲結束
        {
            Stop();
        }
    }
```
修改為
```C#
    // 被下達Start()後，每個UnityEngine.Update()時執行
    protected override void OnTick()
    {
        m_gameTimer += Time.deltaTime;
        m_gamingUI.SetRemainingTime(30f - m_gameTimer);
        m_gamingUI.SetScore(ScoreManager.Instance.Score);
        if (m_gameTimer >= 30f) // 30秒後遊戲結束
        {
            Stop(new EndState());
        }
    }
```  
以類似狀態機模式的設計保持遊戲開發的彈性與容易。
13. 完成。