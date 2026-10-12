using Newtonsoft.Json;
using System;

namespace DataExtractor.DataStructures.Converters {
	public class DataBlockAddressConverter : JsonConverter {
		public override bool CanRead => false;

		public override bool CanConvert(Type objectType) {
			return objectType == typeof(DataBlockAddress);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			var address = (DataBlockAddress) value;
			writer.WriteValue(address.ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			throw new NotImplementedException();
		}
	}
}