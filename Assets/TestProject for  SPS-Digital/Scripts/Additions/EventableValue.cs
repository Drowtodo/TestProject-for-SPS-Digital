using System;

public class EventableValue<T>
{
    private T _value;

    public T Value
    {
        get => _value;
        private set
        {
            if(!Equals(value, _value))
            {
                OnValueChange?.Invoke(value);
                _value = value;
            }
        }
    }

    public event Action<T> OnValueChange;

    public EventableValue(T initValue = default)
    {
        _value = initValue;
    }

    public void Set(T value)
    {
        Value = value;
    }

}
