using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client
{
    public class Topic : ModelBase
    {
        private readonly Facade.Topic topic;
        private readonly Group @group;
        private readonly RestClient restClient;

        public Topic(
            Facade.Topic topic, 
            Group group,
            RestClient restClient)
            : base((string) topic.Id)
        {
            this.topic = topic;
            this.@group = @group;
            this.restClient = restClient;
        }


        public string Name
        {
            get { return topic.Name; }
            set { topic.Name = value; }
        }
        public string Description
        {
            get { return topic.Description; }
            set { topic.Description = value; }
        }
        public Group Group { get { return group; } }

        public void Delete()
        {
            restClient.Delete(topic.GetLinkForRelation("Delete"));
        }

        public void SaveChanges()
        {
            var topicPut = new TopicPut
                               {
                                   Description = this.Description,
                                   Name = this.Name,
                                   GroupId = (Identity) group.Id,
                                   Id = (Identity) Id
                               };
            restClient.Put(topic.GetLinkForRelation("Update"), topicPut);
        }
    }
}