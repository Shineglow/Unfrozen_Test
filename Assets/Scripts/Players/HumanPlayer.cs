using System.Collections.Generic;

public class HumanPlayerBase : PlayerBase
{
    private GuiListner PlayerGUI;

    public HumanPlayerBase(List<Unit> enemys)
    {
        PlayerInit(enemys);
    }

    private void PlayerInit(List<Unit> Enemys)
    {
        PlayerGUI = FindObjectOfType<GuiListner>();
        PlayerGUI.GuiInit(Enemys);
        PlayerGUI.GuiCallback += GuiListener;
    }

    private void GuiListener(/*PlayerAction a, */Unit u, Animation uAction)
    {
        EndTurn(u, uAction);
    }

    public override void TakeTurn(Unit u)
    {
        PlayerGUI.active = true;
        PlayerGUI.ShowStats(u);
    }
}