using System.Threading.Tasks;
using Game.GUI.Windows.Managers;

namespace Game.GUI.Windows.Transitions
{
public class EmptyTransition : IWindowTransition
{
    public Task Open(WindowProperties windowProperties)
    {
        windowProperties.rectTransform.localPosition = WindowTransitionStatic.startPoint;
        return Task.CompletedTask;
    }

    public Task Close(WindowProperties windowProperties)
    {
        windowProperties.rectTransform.localPosition = WindowTransitionStatic.startPoint;
        return Task.CompletedTask;
    }
}
}