using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine($"[{streamId.ToString()}] Loading aggregate root");
            var aggregateType = Type.GetType(this.aggregateClass, true);
            dynamic aggregate = (Activator.CreateInstance(aggregateType)) as AggregateRoot;
            var streamEvents = new List<ResolvedEvent>();

            StreamEventsSlice currentSlice;
            var nextSliceStart = StreamPosition.Start;
            do
            {
                currentSlice =
                    this.connection.ReadStreamEventsForwardAsync(
                            streamId.ToString(), 
                            nextSliceStart, 
                            200, 
                            false).Result;

                nextSliceStart = (int) currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);
            
            foreach (var eventFromStream in streamEvents)
            {
                var eventType = Type.GetType(eventFromStream.Event.EventType,true);
                var json = Encoding.UTF8.GetString(
                    eventFromStream.Event.Data,
                    0,
                    eventFromStream.Event.Data.Length);

                dynamic loadedEvent = eventType.GetMethod("Deserialize").Invoke(null, new object[] {json});
                Console.WriteLine($"[{streamId.ToString()}] Applying {eventType.ToString()}");
                aggregate.Apply(loadedEvent);
            }
            return aggregate;
        }
    }
}