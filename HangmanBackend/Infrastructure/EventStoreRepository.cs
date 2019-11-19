using System;
using System.Text;
using EventStore.ClientAPI;
using HangmanBackend.Domain;

namespace HangmanBackend.Infrastructure
{
    public class EventStoreRepository
    {
        private IEventStoreConnection connection;
        private string aggregateClass;
        
        public EventStoreRepository(IEventStoreConnection connection, string aggregateClass)
        {
            this.connection = connection;
            this.aggregateClass = aggregateClass;
        }

        public void Save(AggregateRoot aggregateRoot)
        {
            var uncommittedEvents = aggregateRoot.getUncommittedEvents();
            var originalPlayHead = aggregateRoot.getPlayHead() - uncommittedEvents.Count;
            foreach (var domainEvent in uncommittedEvents)
            {
                this.connection.AppendToStreamAsync(aggregateRoot.getAggregateRootId().ToString(),
                    originalPlayHead, domainEvent).Wait();
                originalPlayHead++;
            }
        }

        public AggregateRoot Load(Guid streamId)
        {
            var aggregateType = Type.GetType(this.aggregateClass, true);
            dynamic aggregate = (Activator.CreateInstance(aggregateType)) as AggregateRoot;
            var streamEvents =
                this.connection.ReadStreamEventsForwardAsync(
                    streamId.ToString(), 0, 1, false).Result;

            foreach (var eventFromStream in streamEvents.Events)
            {
                var eventName = eventFromStream.Event.EventType;
                var eventType = Type.GetType(eventName,true);
                var domainEvent = (Activator.CreateInstance(eventType));
                var method = domainEvent.GetType().GetMethod("deserialize");
                var args = new object[] { 
                    Encoding.UTF8.GetString(
                        streamEvents.Events[0].Event.Data, 
                        0, 
                        streamEvents.Events[0].Event.Data.Length) 
                };
                aggregate.Apply(method.Invoke(domainEvent, args));
            }
            return aggregate;
        }
    }
}