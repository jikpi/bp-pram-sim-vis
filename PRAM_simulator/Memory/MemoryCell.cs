/*
 * Author: Jan Kopidol
 */

using System.ComponentModel;

namespace PRAM_lib.Memory
{
    public class MemoryCell : INotifyPropertyChanged
    {
        private int _value;
        public bool HasBeenWrittenTo { get; set; } = false;

        public int Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }

                HasBeenWrittenTo = true;
            }
        }

        public MemoryCell()
        {
            _value = 0;
        }

        public MemoryCell(int value)
        {
            _value = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
