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
