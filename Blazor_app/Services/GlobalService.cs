namespace Blazor_app.Services
{
    public class GlobalService
    {
        //## State text
        public string LastState { get; private set; } = "None";
        public enum LastStateUniform { Ok, Note, Error }
        public LastStateUniform LastStateType { get; private set; } = LastStateUniform.Ok;

        public event Action LastStateUpdated;
        public void SetLastState(string state, LastStateUniform type)
        {
            LastState = state;
            LastStateType = type;
            LastStateUpdated?.Invoke();
        }
        //---------------------------------------------

        
    }
}
