// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/26 8:47:41 LeeZheng  新建。
// ===================================================================
using System;

namespace Compete.Mis.Exceptions
{
    /// <summary>
    /// BusinessException 类。
    /// </summary>
    public class BusinessException : MisException
    {
        /// <summary>
        /// 结构方法。
        /// </summary>
        public BusinessException()
            : base()
        {
        }

        /// <summary>
        /// 结构方法。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public BusinessException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 结构方法。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 结构方法。
        /// </summary>
        /// <param name="info">包含有关所引发异常的序列化对象数据。</param>
        /// <param name="context">包含关于源或目标的上下文信息。</param>
        //[SecuritySafeCritical]
        //public BusinessException(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //}
    }
}
