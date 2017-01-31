using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace INotifyProperyChangedDemo
{
    public interface IFormatter<T>
    {
        T FormatField(T source);
    }

    public class ValueToUpperCaseFormatter : IFormatter<string>
    {
        public string FormatField(string source)
        {
            if (source == null) throw new ArgumentNullException("Invalid value for formatter");
            return source.ToUpper();
        }
    }

    public class DefaultFormatter<T> : IFormatter<T>
    {
        public T FormatField(T source) => source;
    }

    public class CustomPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public string ModifiedField { get; set; }

        public CustomPropertyChangedEventArgs(string propertyName) : base(propertyName)
        {
        }

        public override string ToString()
        {
            return $"Field changed: {PropertyName}\nFormatter: {ModifiedField}";
        }
    }

    class ExampleObject<TFormatter> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null, string modifiedValue = "default")
        {
            PropertyChanged?.Invoke(this,
                new CustomPropertyChangedEventArgs(propertyName) { ModifiedField = modifiedValue });
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            var formatter = new ValueToUpperCaseFormatter();
            OnPropertyChanged(propertyName, formatter.FormatField(value as string));
            return true;
        }

        public ExampleObject()
        {
            _data = "default value";
        }

        private string _data;
        public string Data
        {
            get { return _data; }
            set { SetField(ref _data, value, nameof(Data)); }
        }
    }
}
