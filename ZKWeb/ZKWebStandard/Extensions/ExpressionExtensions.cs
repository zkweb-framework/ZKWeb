using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Linq expression extension methods<br/>
	/// Linq表达式的扩展函数<br/>
	/// </summary>
	public static class ExpressionExtensions {
		/// <summary>
		/// Get member information from lambda expression<br/>
		/// The expression should looks like: x =&gt; x.Name<br/>
		/// 获取表达式中的成员信息<br/>
		/// 表达式的格式应该是: x =&gt; x.Name<br/>
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// Expression&lt;Func&lt;TestClass, string&gt;&gt; expr = t =&gt; t.TestMember;
		/// var memberInfo = expr.GetMemberInfo();
		/// </code>
		/// </example>
		public static MemberInfo GetMemberInfo(this LambdaExpression expression) {
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null) {
				throw new ArgumentException("GetMemberAttribute require body of expression is MemberExpression");
			}
			return memberExpression.Member;
		}

		/// <summary>
		/// Get member's attribute from lambda expression<br/>
		/// The expression should looks like: x =&gt; x.Name<br/>
		/// 获取表达式中的成员信息上的属性<br/>
		/// 表达式的格式应该是: x =&gt; x.Name<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TAttribute">Attribute type</typeparam>
		/// <param name="expression">Lambda expression</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// Expression&lt;Func&lt;TestClass, string&gt;&gt; expr = t =&gt; t.TestMember;
		/// var attr = expr.GetMemberAttribute&lt;DescriptionAttribute&gt;();
		/// </code>
		/// </example>
		public static TAttribute GetMemberAttribute<TAttribute>(this LambdaExpression expression)
			where TAttribute : Attribute {
			return expression.GetMemberInfo().GetCustomAttributes(
				typeof(TAttribute), true).FirstOrDefault() as TAttribute;
		}

		/// <summary>
		/// Expression visitor used to replace node in expression<br/>
		/// 用于替换节点的表达式访问器<br/>
		/// </summary>
		public class ReplaceExpressionVisitor : ExpressionVisitor {
			private readonly Expression _oldValue;
			private readonly Expression _newValue;

			/// <summary>
			/// Initialize<br/>
			/// 初始化<br/>
			/// </summary>
			public ReplaceExpressionVisitor(Expression oldNode, Expression newNode) {
				_oldValue = oldNode;
				_newValue = newNode;
			}

			/// <summary>
			/// Replace node in expression<br/>
			/// 替换表达式中的节点<br/>
			/// </summary>
			public override Expression Visit(Expression node) {
				if (node == _oldValue) {
					return _newValue;
				}
				return base.Visit(node);
			}
		}

		/// <summary>
		/// Replace node in expression recursively<br/>
		/// 递归替换表达式中的节点<br/>
		/// </summary>
		/// <param name="expression">Expression</param>
		/// <param name="oldNode">Old node</param>
		/// <param name="newNode">New Node</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// Expression&lt;Func&lt;TestClass, bool&gt;&gt; exprA = a =&gt; a.TestMember.Contains("abc");
		/// Expression&lt;Func&lt;TestClass, bool&gt;&gt; exprB = a =&gt; a.TestMember.Contains("asd");
		/// var exprMerged = Expression.Lambda&lt;Func&lt;TestClass, bool&gt;&gt;(
		///		Expression.AndAlso(
		///			exprA.Body,
		///			exprB.Body.ReplaceNode(exprB.Parameters[0], exprA.Parameters[0])),
		///		exprA.Parameters[0]);
		/// var method = exprMerged.Compile();
		/// </code>
		/// </example>
		public static Expression ReplaceNode(
			this Expression expression, Expression oldNode, Expression newNode) {
			var visitor = new ReplaceExpressionVisitor(oldNode, newNode);
			return visitor.Visit(expression);
		}
	}
}
