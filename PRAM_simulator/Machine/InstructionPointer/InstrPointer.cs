namespace PRAM_lib.Machine.InstructionPointer
{
    public class InstrPointer
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

        public InstrPointer(int initialValue)
        {
            value = initialValue;
        }
    }
}
