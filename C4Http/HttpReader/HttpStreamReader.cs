using System.Text;

namespace C4Http.HttpReader
{
    internal class HttpStreamReader
    {
        const string NEWLINE = "\n";

        const byte TEST_MULTIBYTE_MASK = 240;
        const byte TEST_MULTIBYTE_2 = 192;
        const byte TEST_MULTIBYTE_3 = 224;
        const byte TEST_MULTIBYTE_4 = 240;

        private readonly Stream _stream;
        private readonly Encoding _encoding;
        private readonly byte[] _newLineBytes;

        private readonly SemaphoreSlim _bufferLock;
        readonly byte[] _buffer;
        private int _bufferReadPointer;
        // number of bytes in the buffer that can be UTF-8 decoded safely
        int _bufferUsableByteCount;


        // number of bytes at the end of the buffer that must not be read. (incomplete UTF-8 character at the end of the buffer)
        int _bufferUnusableByteCount;

        private int BufferLastReadableIndex { get { return (_bufferUsableByteCount - 1); } }

        public HttpStreamReader(Stream stream, Encoding encoding)
        {
            _stream = stream;
            _encoding = encoding;
            _newLineBytes = encoding.GetBytes(NEWLINE);
            _buffer = new byte[2047]; // experiment with buffer size
            _bufferLock = new(1);
            _bufferUsableByteCount = 0;
        }



        private async Task AdvanceBuffer_UnlockedAsync()
        {
            // sanity check
            if (_bufferReadPointer < _bufferUsableByteCount)
            {
                throw new Exception("aad3d4 haven't read all the bytes");
            }
            if (_bufferReadPointer > _bufferUsableByteCount)
            {
                throw new Exception("aad3d5 have read some broken bytes");
            }


            // this many bytes remain unread at the end of the buffer.
            var unusedByteCount = _bufferUnusableByteCount;
            if (unusedByteCount > 0)
            {
                // move all unused bytes to the start of the buffer
                Array.Copy(_buffer, _buffer.Length - unusedByteCount, _buffer, 0, unusedByteCount);
            }

            // fill the rest of the buffer from the stream
            var nextUsableByteCount = unusedByteCount + await _stream.ReadAsync(_buffer, unusedByteCount, _buffer.Length - unusedByteCount);

            // https://de.wikipedia.org/wiki/UTF-8#Kodierung
            // 1b: 0xxxxxxx
            // 2b: 110xxxxx 10xxxxxx
            // 3b: 1110xxxx 10xxxxxx 10xxxxxx
            // 4b: 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
            var testBytes = _buffer.TakeLast(3).ToArray();
            if ((testBytes[0] & TEST_MULTIBYTE_MASK) == TEST_MULTIBYTE_4)
            {
                _bufferUnusableByteCount = 3;
            }
            if ((testBytes[1] & TEST_MULTIBYTE_MASK) >= TEST_MULTIBYTE_3)
            {
                _bufferUnusableByteCount = 2;
            }
            if ((testBytes[2] & TEST_MULTIBYTE_MASK) >= TEST_MULTIBYTE_2)
            {
                _bufferUnusableByteCount = 1;
            }

            _bufferReadPointer = 0;
            _bufferUsableByteCount = nextUsableByteCount - _bufferUnusableByteCount;
        }

        private int BufferIndexOf_Unlocked(byte[] value, int? startIndex = null)
        {
            startIndex ??= _bufferReadPointer;

            // sanity
            if (startIndex < _bufferReadPointer) { throw new Exception("a1c7cbb don't seek into the past, my friend."); }
            if (value.Length > 1) { throw new NotImplementedException("ac7950 maybe don't try this yet."); }

            var result = -1;
            for (int i = startIndex.Value; i < _bufferUsableByteCount; i++)
            {
                if (_buffer[i] == _newLineBytes[0])
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        private byte[] BufferReadToIndex_Unlocked(int endIndex)
        {
            var length = endIndex - (_bufferReadPointer - 1); // reading from index 0 to index 0 still reads that one byte at index 0.
            var buffer = new byte[length];

            var bytesRead = BufferReadToIndex_Unlocked(buffer, 0, endIndex);

            // sanity
            if (length != bytesRead) { throw new Exception("e3ca71 sorry, how did I get here?"); }

            return buffer;
        }

        private int BufferReadToIndex_Unlocked(byte[] destination, int destinationIndex, int readToIndex)
        {
            var length = readToIndex - (_bufferReadPointer - 1); // reading from index 0 to index 0 still reads that one byte at index 0.

            // sanity
            if (length < 1) { throw new Exception("fbef6e1 please read some more."); }
            if (readToIndex > BufferLastReadableIndex) { throw new Exception("9c8d28 please advance yourself."); }

            Array.Copy(_buffer, _bufferReadPointer, destination, destinationIndex, length);
            _bufferReadPointer += length;
            return length;
        }


        public async Task<string> ReadLineAsync()
        {
            await _bufferLock.WaitAsync();
            try
            {
                if (_bufferUsableByteCount - _bufferReadPointer <= 0)
                {
                    await AdvanceBuffer_UnlockedAsync();
                }

                var nextNewlineIndex = BufferIndexOf_Unlocked(_newLineBytes);
                if (nextNewlineIndex >= 0) // simple case: the buffer contains an entire line
                {
                    var line = _encoding.GetString(BufferReadToIndex_Unlocked(nextNewlineIndex));
                    return line;
                }

                // otherwise, we put the whole (rest of) the buffer into a linebuffer and continue reading the stream, until we find a newline.
                var lineBuilder = new StringBuilder();
                do
                {
                    lineBuilder.Append(_encoding.GetString(BufferReadToIndex_Unlocked(BufferLastReadableIndex)));
                    await AdvanceBuffer_UnlockedAsync();
                    nextNewlineIndex = BufferIndexOf_Unlocked(_newLineBytes);
                }
                while (nextNewlineIndex < 0);

                // then we flush the part of the buffer including the newline into the linebuffer
                lineBuilder.Append(_encoding.GetString(BufferReadToIndex_Unlocked(nextNewlineIndex)));

                // and BAM
                return lineBuilder.ToString();
            }
            finally
            {
                _bufferLock.Release();
            }
        }


        public async Task<(int bytesRead, string text)> ReadBytesAsync(int count)
        {
            // sanity
            if (count < 1) { throw new Exception("e4ce59 please read a little more."); }

            await _bufferLock.WaitAsync();
            try

            {
                if (_bufferUsableByteCount - _bufferReadPointer <= 0)
                {
                    await AdvanceBuffer_UnlockedAsync();
                }

                var bytesRead = 0; // aka. bufferWriteIndex
                var buffer = new byte[count];

                while (bytesRead < count)
                {
                    var bytesToGo = count - bytesRead;

                    var endIndex = (_bufferReadPointer + bytesToGo) - 1; // 1 bytes to go, read from byte 0 to byte 0 please
                    if (endIndex <= BufferLastReadableIndex)
                    {
                        bytesRead += BufferReadToIndex_Unlocked(buffer, bytesRead, endIndex);
                    }
                    else
                    {
                        bytesRead += BufferReadToIndex_Unlocked(buffer, bytesRead, BufferLastReadableIndex);
                        await AdvanceBuffer_UnlockedAsync();
                    }
                }

                return (bytesRead, _encoding.GetString(buffer));
            }
            finally
            {
                _bufferLock.Release();
            }
        }
    }
}
