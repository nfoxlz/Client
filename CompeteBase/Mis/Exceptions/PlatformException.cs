using System;

namespace Compete.Mis.Exceptions
{
    public class PlatformException : MisException
    {
        /// <summary>
        /// 结构方法。
        /// </summary>
        public PlatformException()
            : base()
        {
        }

        /// <summary>
        /// 结构方法。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public PlatformException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 结构方法。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public PlatformException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 结构方法。
        /// </summary>
        /// <param name="info">包含有关所引发异常的序列化对象数据。</param>
        /// <param name="context">包含关于源或目标的上下文信息。</param>
        //[SecuritySafeCritical]
        //public PlatformException(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //}
    }
}
