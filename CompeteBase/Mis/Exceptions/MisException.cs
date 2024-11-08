using System;

namespace Compete.Mis.Exceptions
{
    public abstract class MisException : Exception
    {
        public MisException()
            : base()
        {
        }

        public MisException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 结构方法。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public MisException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
