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
