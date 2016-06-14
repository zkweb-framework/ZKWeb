namespace System.Web {
	/// <summary>
	/// 这个类的用途如下
	/// - 不允许System.Web加载进来
	/// - 防止引用System.Web命名空间的插件出错
	/// </summary>
	public interface IHttpModule { }
}
