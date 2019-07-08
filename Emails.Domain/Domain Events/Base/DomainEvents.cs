using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Domain.Domain_Events.Base
{
    public static class DomainEvents
    {
        private static List<Type> _handlers;


        public static void Init()
        {

                _handlers =
                Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType &&
                y.GetGenericTypeDefinition() == typeof(IHandler<>)))
                .ToList();
        }


        public static void Dispatch(IDomainEvent domainEvent)
        {

            foreach (Type handlerType in _handlers)
            {

                bool canHandleEvent = handlerType
                    .GetInterfaces()
                    .Any(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IHandler<>) &&
                    i.GetGenericArguments()[0] == typeof(IDomainEvent));

                if (canHandleEvent)
                {
                    dynamic handler = Activator.CreateInstance(handlerType);
                    handler.Handle(domainEvent);
                }
            }
        }
    }
}
