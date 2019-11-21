using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using HangmanBackend.Domain;

namespace HangmanBackend.Infrastructure
{
    public class EventStoreRepository<TAggregate> where TAggregate : AggregateRoot
    {
        private IEventStoreConnection connection;

        public EventStoreRepository(IEventStoreConnection connection)
        {
            this.connection = connection;
        }

        public void Save(TAggregate aggregateRoot)
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

        private IEnumerable<ResolvedEvent> LoadEvents(string streamId) {
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

                foreach(var ev in currentSlice.Events) yield return ev;

            } while (!currentSlice.IsEndOfStream);
        }

        public AggregateRoot Load(Guid streamId)
        {
            Console.WriteLine($"[{streamId.ToString()}] Loading aggregate root");

            dynamic aggregate = Activator.CreateInstance<TAggregate>();

            foreach (var eventFromStream in LoadEvents(streamId.ToString()))
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