#if NETCORE
namespace System {
	/// <summary>
	/// 支持克隆的对象接口
	/// 参考:
	/// https://github.com/dotnet/corefx/blob/master/src/System.Net.Http/src/Internal/ICloneable.cs
	/// 进度:
	/// 可能需要一直保留
	/// </summary>
	public interface ICloneable {
		/// <summary>
		/// 克隆对象
		/// </summary>
		/// <returns></returns>
		object Clone();
	}
}
#endif
