using Newtonsoft.Json;
using System;
using System.Reflection;

namespace DataExtractor.DataStructures.Converters {
	public class DataBlockRefConverter : JsonConverter {
		public override bool CanRead => false;

		public override bool CanConvert(Type objectType) {
			return objectType.Name.StartsWith("DataBlockRef");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			var address = (DataBlockAddress) value.GetType().GetField("m_address", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);
			writer.WriteValue(address.ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			throw new NotImplementedException();
		}
	}
}