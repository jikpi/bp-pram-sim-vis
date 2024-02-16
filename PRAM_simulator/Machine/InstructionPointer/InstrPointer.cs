namespace PRAM_lib.Machine.InstructionPointer
{
    /// <summary>
    /// A class representing an instruction pointer.
    /// </summary>
    public class InstrPointer
    {
        private int _value;

        public InstrPointer(int initialValue)
        {
            _value = initialValue;
            ValueChanged = delegate { };
        }

        public int Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnValueChanged();
                }
            }
        }

        public event EventHandler ValueChanged;

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }


    }
}
