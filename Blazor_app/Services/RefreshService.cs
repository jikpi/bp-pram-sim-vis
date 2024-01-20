namespace Blazor_app.Services
{
    public class RefreshService
    {
        public event Action MemoryRefreshed;
        public event Action PramCodeRefreshed;

        public void RefreshMemory()
        {
            MemoryRefreshed?.Invoke();
        }

        public void RefreshPramCode()
        {
            PramCodeRefreshed?.Invoke();
        }
    }
}
