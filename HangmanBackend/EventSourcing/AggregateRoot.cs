using System;
using System.Collections.Generic;
using EventStore.ClientAPI;

namespace HangmanBackend.Domain
{
    public interface AggregateRoot
    {
        Guid getAggregateRootId();
        List<EventData> getUncommittedEvents();
        int getPlayHead();
    }
}