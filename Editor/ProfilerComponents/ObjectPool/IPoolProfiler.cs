using UnityEngine.UIElements;

namespace GameEditor.Pools
{
    internal interface IPoolProfiler
{
    void DrawStatus(VisualElement root);

    void OnPoolDataUpdated();
}
}