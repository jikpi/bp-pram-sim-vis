namespace PRAM_lib.Processor
{
    public class InstructionPointer
    {
        private int value;

        public int Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnValueChanged();
                }
            }
        }

        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public InstructionPointer(int initialValue)
        {
            value = initialValue;
        }
    }
}
