using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overmind.Core.Provider
{
	// TODO: Add lock to handle concurrent write (several providers on the same file).

	/// <summary>Handles interactions with a file based data store.</summary>
	/// <remarks>This is intended as a dummy replacing a true database for test or development, it should not be used in production.</remarks>
	/// <typeparam name="TKey">The data store key type.</typeparam>
	/// <typeparam name="TData">The stored object type.</typeparam>
	public class FileDataProvider<TKey, TData> : IDataProvider<TKey, TData>
	{
		/// <summary></summary>
		/// <param name="storeDirectory">Base directory for the data store in the file system.</param>
		/// <param name="typeName">Name identifying the data type (equivalent to a database table name). If null, use TData name.</param>
		public FileDataProvider(string storeDirectory, string typeName = null)
		{
			if (String.IsNullOrEmpty(typeName))
				typeName = typeof(TData).Name;

			directory = Path.Combine(storeDirectory, typeName);
			this.typeName = typeName;
		}

		private readonly JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
		private readonly string directory;
		private readonly string typeName;
		
		/// <summary>Retrieves a single element from the data store, identified by the passed key.</summary>
		/// <param name="key">The key identifying the element to retrieve.</param>
		/// <returns>The retrieved object, or null if none corresponded to the key.</returns>
		public TData Get(TKey key)
		{
			TData data;
			IDictionary<TKey, TData> dataCollection = Load();
			dataCollection.TryGetValue(key, out data);
			return data;
		}

		/// <summary>Retrieves all the elements from the data store.</summary>
		/// <returns>A collection containing the elements from the data store.</returns>
		public IEnumerable<TData> GetAll()
		{
			return Load().Values;
		}
		
		/// <summary>Sets the element at the passed key, replacing existing data.</summary>
		/// <param name="key">The key identifying the element in the data store.</param>
		/// <param name="data">The object to write in the data store.</param>
		public void Set(TKey key, TData data)
		{
			IDictionary<TKey, TData> dataCollection = Load();
			dataCollection[key] = data;
			Save(dataCollection);
		}

		/// <summary>Loads the data from the file system.</summary>
		/// <returns>A dictionary containing the data.</returns>
		protected IDictionary<TKey, TData> Load()
		{
			string filePath = Path.Combine(directory, typeName + ".json");
			if (File.Exists(filePath) == false)
				return new Dictionary<TKey, TData>();
			using (StreamReader reader = new StreamReader(filePath))
				return (IDictionary<TKey, TData>)serializer.Deserialize(reader, typeof(IDictionary<TKey, TData>));

		}

		/// <summary>Saves the data to the file system. This replaces all existing data.</summary>
		/// <param name="dataCollection">The dictionary containing the data to save.</param>
		protected void Save(IDictionary<TKey, TData> dataCollection)
		{
			string filePath = Path.Combine(directory, typeName + ".json");
			if (Directory.Exists(directory) == false)
				Directory.CreateDirectory(directory);
			using (StreamWriter writer = new StreamWriter(filePath))
				serializer.Serialize(writer, dataCollection, typeof(IDictionary<TKey, TData>));
		}
	}
}
