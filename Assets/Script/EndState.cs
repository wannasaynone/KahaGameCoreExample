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
