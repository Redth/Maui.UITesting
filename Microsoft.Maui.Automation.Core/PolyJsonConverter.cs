// https://gist.github.com/Ilchert/4854a20f790ca963d1ab17d99433c7da

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
	public class JsonKnownTypeAttribute : Attribute
	{
		public Type NestedType { get; }
		public string Discriminator { get; }

		public JsonKnownTypeAttribute(Type nestedType, string discriminator)
		{
			NestedType = nestedType;
			Discriminator = discriminator;
		}
	}

	public class PolyJsonConverter : JsonConverterFactory
	{
		private static readonly Type converterType = typeof(PolyJsonConverter<>);

		public override bool CanConvert(Type typeToConvert)
		{
			var attr = typeToConvert.GetCustomAttributes<JsonKnownTypeAttribute>(false);
			return attr.Any();
		}

		public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
		{
			var attr = typeToConvert.GetCustomAttributes<JsonKnownTypeAttribute>();
			var concreteConverterType = converterType.MakeGenericType(typeToConvert);
			return (JsonConverter)Activator.CreateInstance(concreteConverterType, attr);
		}
	}

	public class ElementConverter : JsonConverter<IElement>
	{
		private static readonly JsonEncodedText TypeProperty = JsonEncodedText.Encode("$type");

		public override bool CanConvert(Type typeToConvert)
			=> typeToConvert.IsAssignableTo(typeof(IElement));

		public override IElement? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using var doc = JsonDocument.ParseValue(ref reader);

			return (IElement)doc.Deserialize(typeToConvert);
		}

		public override void Write(Utf8JsonWriter writer, IElement value, JsonSerializerOptions options)
		{
			var type = value.GetType();
			
			writer.WriteStartObject();
			writer.WritePropertyName(TypeProperty.EncodedUtf8Bytes);
			writer.WriteStringValue(type.FullName);
			using var doc = JsonSerializer.SerializeToDocument(value, type, options);
			foreach (var prop in doc.RootElement.EnumerateObject())
				prop.WriteTo(writer);

			writer.WriteEndObject();
		}
	}

	internal class PolyJsonConverter<T> : JsonConverter<T>
	{
		private readonly Dictionary<string, Type> _discriminatorCache;
		private readonly Dictionary<Type, string> _typeCache;

		private static readonly JsonEncodedText TypeProperty = JsonEncodedText.Encode("$type");

		public PolyJsonConverter(IEnumerable<JsonKnownTypeAttribute> resolvers)
		{
			_discriminatorCache = resolvers.ToDictionary(p => p.Discriminator, p => p.NestedType);
			_typeCache = resolvers.ToDictionary(p => p.NestedType, p => p.Discriminator);
		}

		public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using var doc = JsonDocument.ParseValue(ref reader);
			if (!doc.RootElement.TryGetProperty(TypeProperty.EncodedUtf8Bytes, out var typeElement))
				throw new JsonException();

			var discriminator = typeElement.GetString();
			if (discriminator is null || !_discriminatorCache.TryGetValue(discriminator, out var type))
				throw new JsonException();


			return (T)doc.Deserialize(type, options);
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			var type = value.GetType();
			if (!_typeCache.TryGetValue(type, out var discriminator))
				throw new JsonException();

			writer.WriteStartObject();
			writer.WritePropertyName(TypeProperty.EncodedUtf8Bytes);
			writer.WriteStringValue(discriminator);
			using var doc = JsonSerializer.SerializeToDocument(value, type, options);
			foreach (var prop in doc.RootElement.EnumerateObject())
				prop.WriteTo(writer);

			writer.WriteEndObject();
		}
	}

	public class JsonProtectedResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
	{
		protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
		{
			var prop = base.CreateProperty(member, memberSerialization);
			if (!prop.Writable)
			{
				var property = member as PropertyInfo;
				var hasPrivateSetter = property?.GetSetMethod(true) != null;
				prop.Writable = hasPrivateSetter;
			}
			return prop;
		}
	}
}
