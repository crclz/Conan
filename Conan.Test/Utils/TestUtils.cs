using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Conan.UnitTest.Utils
{
	static class TestUtils
	{
		public static T DeepClone<T>(this T obj)
		{
			var json = obj.ToJson();
			var newObject = json.FromJson<T>();
			return newObject;
		}

		public static void AssertJsonEqual(this object a, object b)
		{
			var ajson = a.ToJson();
			var bjson = b.ToJson();

			Assert.Equal(ajson, bjson);
		}
	}
}
