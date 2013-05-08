using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.ServiceLocation;

namespace RssAlerter.Mvc.Web.Infrastructure
{
    public class SimpleServiceLocator : ServiceLocatorImplBase
    {
        private Dictionary<Type, Func<object>> _objectCreationMap = new Dictionary<Type, Func<object>>();

        public void Register<T>(Func<object> creationFunction)
        {
            _objectCreationMap[typeof(T)] = creationFunction;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            Func<object> creationFunction;
            if (_objectCreationMap.TryGetValue(serviceType, out creationFunction))
                if (creationFunction != null)
                    return new[] { creationFunction.Invoke() };

            return new object[] { };
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            Func<object> creationFunction;
            if (_objectCreationMap.TryGetValue(serviceType, out creationFunction))
                if (creationFunction != null)
                    return creationFunction.Invoke();

            return null;
        }
    }
}