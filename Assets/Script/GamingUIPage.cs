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
