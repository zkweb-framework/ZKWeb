using ZKWebStandard.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// Key-value cache factory interface
	/// </summary>
	public interface ICacheFactory {
		/// <summary>
		/// Create cache by given options
		/// </summary>
		/// <typeparam name="TKey">Key type</typeparam>
		/// <typeparam name="TValue">Value type</typeparam>
		/// <param name="options">Cache factory options</param>
		/// <returns></returns>
		IKeyValueCache<TKey, TValue> CreateCache<TKey, TValue>(object options = null);
	}
}
