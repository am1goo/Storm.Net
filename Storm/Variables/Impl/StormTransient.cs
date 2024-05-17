using System;
using System.Collections.Generic;

namespace Storm
{
    public class StormTransient : IStormVariableRW, IDisposable
    {
        private bool _isDisposed;
        public bool isDisposed => _isDisposed;

        private string _name;
        public string name => _name;

        private Type _type;
        public Type type => _type;

        private object _value;

        public StormTransient(Type type) : this(type, null)
        {
        }

        public StormTransient(Type type, object value)
        {
            _name = string.Empty;
            _type = type;
            _value = value;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _name = null;
            _type = null;
            _value = null;
        }

        public void GetAttributes(List<Attribute> result)
        {
            //do nothing
        }

        public void SetValue(object value)
        {
            _value = value;
        }

        public object GetValue()
        {
            return _value;
        }
    }
}
