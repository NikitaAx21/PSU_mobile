using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PSU_Mobile_Server.Controllers;

namespace PSU_Mobile_Server
{
	internal class ApiControllersInitializer
	{
		public Dictionary<string, BaseApiController> RequestToControllersDictionary { get; }
		public static ApiControllersInitializer Instance { get; } = new ApiControllersInitializer();

		private ApiControllersInitializer()
		{
			var allTypes = Assembly.GetAssembly(typeof(BaseApiController)).DefinedTypes;
			var types = allTypes.Where(t => t.IsSubclassOf(typeof(BaseApiController)));
			var baseApiControllers = types.Select(Activator.CreateInstance).Cast<BaseApiController>();

			RequestToControllersDictionary = baseApiControllers.ToDictionary(c => c.RequestName.ToLowerInvariant(), c => c);
		}
	}
}
