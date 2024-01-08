using System;
using System.Linq.Expressions;

namespace Compete.Mis.ViewModels
{
    public static class ViewModelExtension
    {
        /// <summary>
        /// 通知属性值改变扩展方法。
        /// </summary>
        /// <typeparam name="T">属性值改变基类（PropertyChangedBase）的子类。</typeparam>
        /// <typeparam name="TProperty">改变的属性。</typeparam>
        /// <param name="propertyChangedBase">属性值改变子类的对象。</param>
        /// <param name="expression">指示属性的Lambda表达式。</param>
        public static void NotifyPropertyChanged<T, TProperty>(this T propertyChangedBase, Expression<Func<T, TProperty>> expression) where T : ViewModelBase
        {
            if (expression.Body is MemberExpression memberExpression)
                propertyChangedBase.NotifyPropertyChanged(memberExpression.Member.Name);
            else
                throw new NotImplementedException();
        }
    }
}
