using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.GUI.Windows.Managers;

namespace Game.GUI.Windows.Transitions
{
public class ParallelProxy : IWindowTransition
{
    private readonly List<IWindowTransition> _transitions;

    public ParallelProxy(params IWindowTransition[] transitions)
    {
        _transitions = transitions.ToList();
    }

    public Task Open(WindowProperties windowProperties) =>
        Task.WhenAll(_transitions.Select(x => x.Open(windowProperties)));

    public Task Close(WindowProperties windowProperties) =>
        Task.WhenAll(_transitions.Select(x => x.Close(windowProperties)));
}
}