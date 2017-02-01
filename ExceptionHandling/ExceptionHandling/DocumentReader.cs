using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionHandling
{
    class DocumentReader : IDisposable, IEnumerable<string>
    {
        private FileStream _fileStream;
        private StreamReader _streamReader;
        private bool _isDisposed = false;
        private bool _isOpen = false;
        private uint _numberOfChunks;
        private List<string> chunk;

        public Predicate<string> Validator { get; private set; } = (str) => true;
        public Func<string, string> Processor { get; private set; } = (str) => str;
        
        public DocumentReader SetValidator(Predicate<string> predicate)
        {
            Validator = predicate;
            return this;
        }

        public DocumentReader SetProcessor(Func<string, string> function)
        {
            Processor = function;
            return this;
        }

        public uint NumberOfChunks
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("object is already disposed");
                }
                if (!_isOpen)
                {
                    throw new FileNotFoundException("file is not opened");
                }
                return _numberOfChunks;
            }
            set
            {
                _numberOfChunks = value;
            }
        }

        public void Open(string fileName)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("object is already disposed");
            }
            _fileStream = new FileStream(fileName, FileMode.Open);
            _streamReader = new StreamReader(_fileStream);
            try
            {
                var firstLine = _streamReader.ReadLine();
                _numberOfChunks = uint.Parse(firstLine);
                _isOpen = true;
            }
            catch (FormatException ex)
            {
                throw new CustomDocumentFormatException("first line should be uint", ex);
            }
        }
   
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _isOpen = false;
            _fileStream?.Dispose();
            _fileStream = null;
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("object is already disposed");
            }
            if (!_isOpen)
            {
                throw new FileNotFoundException("file is not opened");
            }
            for (int i = 0; i < _numberOfChunks; i++)
            {
                var chunkToProcess = _streamReader.ReadLine();
                
                if (!Validator(chunkToProcess))
                {
                    throw new BusinessLogicViolationException("validator error");
                }
                yield return Processor?.Invoke(chunkToProcess) ?? chunkToProcess;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class CustomDocumentFormatException : Exception
    {
        public CustomDocumentFormatException(string message, Exception innerException):base(message, innerException)
        {
        }
    }

    public class BusinessLogicViolationException : Exception
    {
        public BusinessLogicViolationException(string message) : base(message)
        {
        }
        public BusinessLogicViolationException(string message, Exception innerException):base(message, innerException)
        {
        }
    }


}
