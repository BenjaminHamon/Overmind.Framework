using System.Collections.Generic;

namespace Overmind.Core.Provider
{
	/// <summary>
	/// Abstracts interaction with a data store.
	/// Elements are identified by a unique key.
	/// </summary>
	/// <typeparam name="TKey">The data store key type.</typeparam>
	/// <typeparam name="TData">The stored object type.</typeparam>
	public interface IDataProvider<TKey, TData>
	{
		/// <summary>Retrieves a single element from the data store, identified by the passed key.</summary>
		/// <param name="key">The key identifying the element to retrieve.</param>
		/// <returns>The retrieved object, or null if none corresponded to the key.</returns>
		TData Get(TKey key);

		/// <summary>Retrieves all the elements from the data store.</summary>
		/// <returns>A collection containing the elements from the data store.</returns>
		IEnumerable<TData> GetAll();

		/// <summary>Sets the element at the passed key, replacing existing data.</summary>
		/// <param name="key">The key identifying the element in the data store.</param>
		/// <param name="data">The object to write in the data store.</param>
		void Set(TKey key, TData data);
	}
}