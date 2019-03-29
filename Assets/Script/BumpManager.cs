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
