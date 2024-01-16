namespace Blazor_app.Services
{
    public class RefreshService
    {
        public event Action MemoryRefreshed;

        public void RefreshMemory()
        {
            MemoryRefreshed?.Invoke();
        }
    }
}
