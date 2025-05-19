using System;
using System.Collections.Generic;
using System.Linq;
using Game.GUI.Windows.Factories;
using UnityEngine;

namespace Game.GUI.Windows.Managers
{
    public interface ITabSwitcher<T> where T : struct, Enum
    {
        float Height { get; }
        event Action<T> OnClickOpenTab;
        void SetAsLastSibling();
        void OpenTabSmooth(T tab);
    }
    
    public class WindowsManagerTab<TTab> : IWindowsManager where TTab : struct, Enum
    {
        private WindowConstructorTab<IMediator, TTab> _constructor;
        private readonly IWindowFactory _windowFactory;
        private readonly Transform _root;
        
        public ITabSwitcher<TTab> Switcher => _constructor.Switcher;
 
        public WindowsManagerTab(Transform root, IWindowFactory windowFactory, MediatorToTabBinder<Type, TTab> mediatorTabBinder)
        {
            _root = root;
            _windowFactory = windowFactory;
            
            _windowFactory.TryCreateTabSwitcher<TTab>(_root, out var switcher);
            _constructor = new WindowConstructorTab<IMediator, TTab>(switcher, mediatorTabBinder);

            GenerateTabs(mediatorTabBinder.Select(x => x.Key).ToArray());
        }

        private void GenerateTabs(Type[] mediators)
        {
            foreach (var type in mediators)
            {
                if (_windowFactory.TryCreateWindow(type, _root, out var mediator, out var window) == false)
                    throw new ArgumentNullException(type.Name, $"Can't create mediator {type}");
                _constructor.InsertTab(mediator, window);
            }
        }
        
        public bool TryGetWindows<TMediator>(out TMediator[] mediator) where TMediator : class, IMediator
        {
            throw new NotImplementedException();
        }

        public bool TryGetFirstWindow<TMediator>(out TMediator mediator) where TMediator : class, IMediator
        {
            throw new NotImplementedException();
        }

        public TMediator OpenWindow<TMediator>(Action<TMediator> initWindow = null, OpenMode mode = OpenMode.Overlay, int priority = 0) where TMediator : class, IMediator
        {
            if (_constructor.Has<TMediator>() == false)
                throw new ArgumentException($"{typeof(TMediator).Name} not found!");

            _constructor.OpenWindowSilently<TMediator>();
            return null;
        }

        public bool CloseWindow<TMediator>() where TMediator : class, IMediator
        {
            throw new NotImplementedException();
        }

        public bool CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator
        {
            throw new NotImplementedException();
        }

        public void CloseWindows()
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}