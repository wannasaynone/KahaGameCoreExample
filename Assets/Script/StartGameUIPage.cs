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

    public void Button_StartGame()
    {
        new GameState().Start();
    }
}
