using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Conan.UnitTest.Utils
{
	static class JsonExtension
	{
		private static JsonSerializerSettings Settings => new JsonSerializerSettings
		{
			ContractResolver = new NonPublicResolver(),
			ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
		};

		public static T FromJson<T>(this string json)
		{
			return JsonConvert.DeserializeObject<T>(json, Settings);
		}

		public static string ToJson<T>(this T obj)
		{
			return JsonConvert.SerializeObject(obj, Settings);
		}
	}

	public class NonPublicResolver : DefaultContractResolver
	{
		protected override JsonProperty CreateProperty(
			MemberInfo member,
			MemberSerialization memberSerialization)
		{
			var prop = base.CreateProperty(member, memberSerialization);

			if (!prop.Writable)
			{
				var property = member as PropertyInfo;
				if (property != null)
				{
					var hasPrivateSetter = property.GetSetMethod(true) != null;
					prop.Writable = hasPrivateSetter;
				}
			}

			return prop;
		}
	}
}
