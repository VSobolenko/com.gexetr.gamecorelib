using System;
using System.Linq;

namespace Game.GUI.Windows.Factories
{
    internal sealed class DefaultMediatorInstantiator : IMediatorInstantiator
    {
        public TMediator Instantiate<TMediator>(WindowUI windowUI, params object[] extraArgs) 
            where TMediator : class, IMediator
        {
            return (TMediator)Instantiate(typeof(TMediator), windowUI, extraArgs);
        }

        public IMediator Instantiate(Type mediatorType, WindowUI windowUI, params object[] extraArgs)
        {
            var args = new object[1 + extraArgs.Length];
            args[0] = windowUI;
            Array.Copy(extraArgs, 0, args, 1, extraArgs.Length);

            try
            {
                return (IMediator)Activator.CreateInstance(mediatorType, args);
            }
            catch (MissingMethodException)
            {
                throw new InvalidOperationException(
                    $"No suitable constructor found for {mediatorType.Name}. " +
                    $"Arguments: {string.Join(", ", args.Select(a => a?.GetType().Name ?? "null"))}");
            }
        }
    }
}