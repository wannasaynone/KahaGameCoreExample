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
            Stop(new EndState());
        }
    }
}
