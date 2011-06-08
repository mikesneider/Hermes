using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;

namespace Business.Tests.Util
{
    public class StubMessageRepository: IMessageRepository
    {
        public StubMessageRepository(params Message[] entities)
        {
            Documents = new HashSet<Message>(entities);
            Updates = new HashSet<Message>();
        }

        public HashSet<Message> Documents { get; set; }

        public HashSet<Message> Updates { get; set; }

        public void MakePersistent(Message entity)
        {
            Documents.Add(entity);
        }

        public void MakeTransient(MessageKey key)
        {
            Documents.Remove(Documents.FirstOrDefault(e =>  e.TopicId == key.TopicId && e.Id == key.MessageId));
        }

        public void Update(Message entity)
        {
            Updates.Add(entity);
        }
    }
}