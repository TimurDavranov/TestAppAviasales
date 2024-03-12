using AS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Core.Helpers
{
    public interface IDispatcher
    {
        void RegisterHandler<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
            where TRequest : BaseEvent
            where TResponse : class;
        void RegisterNoContentHandler<T>(Func<T, Task> handler) where T : BaseEvent;
        Task<dynamic> SendAsync(BaseEvent command);
        Task SendNoContentAsync(BaseEvent command);
    }

    public class Dispatcher<E> : IDispatcher where E : class
    {
        private readonly Dictionary<Type, Func<BaseEvent, Task>> _noContentHandlers = new();

        public void RegisterNoContentHandler<T>(Func<T, Task> handler) where T : BaseEvent
        {
            if (_noContentHandlers.ContainsKey(typeof(T)))
            {
                throw new IndexOutOfRangeException("You cannot register the same handler twice!");
            }

            _noContentHandlers.Add(typeof(T), x => handler((T)x));
        }

        public Task SendNoContentAsync(BaseEvent command)
        {
            if (_noContentHandlers.TryGetValue(command.GetType(), out var handler))
            {
                return handler(command);
            }
            else
            {
                throw new ArgumentNullException(nameof(handler), "No handler was registered!");
            }
        }

        
        
        private readonly Dictionary<Type, Func<BaseEvent, Task<dynamic>>> _handlers = new();

        public void RegisterHandler<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler) where TRequest : BaseEvent
                                                                                                  where TResponse : class
        {
            if (_handlers.ContainsKey(typeof(TRequest)))
            {
                throw new IndexOutOfRangeException("You cannot register the same command handler twice!");
            }

            _handlers.Add(typeof(TRequest), async x => await handler((TRequest)x));
        }

        public Task<dynamic> SendAsync(BaseEvent command)
        {
            if (_handlers.TryGetValue(command.GetType(), out var handler))
            {
                return handler(command);
            }
            else
            {
                throw new ArgumentNullException(nameof(handler), "No command handler was registered!");
            }
        }
    }
}
