using System;
using Business.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Groups;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace Business.Tests.Groups
{
    [TestFixture]
    public class UpdateGroupCommandTests
    {
        [Test]
        public void WhenGroupNameIsDuplicated_ThenThrowValidateException()
        {
            var id = Identity.Random();
            var groupCommand = CreateUpdateGroupCommand(Mock.Of<IExistGroupByGroupName>(q => q.Execute("test", id) == true),
                Mock.Of<IEntityById>(q => q.Exist<Group>(id)));

            groupCommand.Executing(gc => gc.Execute(new Group { Name = "test", Id = id }))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(Texts.GroupNameMustBeUnique);
        }
        [Test]
        public void WhenParentIdDoesNotExist_ThenThrowException()
        {
            var id = Identity.Random();
            var idParent = Identity.Random();
            var groupCommand = CreateUpdateGroupCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Group>(id)  && q.Exist<Group>(idParent) == false));

            var @group = new Group {Id = id, Name = "test", ParentId =idParent };
            groupCommand.Executing(gc => gc.Execute(@group))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(Texts.EntityNotFound);
        }

        [Test]
        public void WhenParentHasTheSameId_ThenThrowException()
        {
            var groupCommand = CreateUpdateGroupCommand(
                            entityById: Mock.Of<IEntityById>(q => q.Exist<Group>(It.IsAny<Identity>()) == true
                                                                            && q.Get<Group>(new Identity("4de7e38617b6c420a45a84c4")) == new Group()));

            var @group = new Group
                             {
                                 Name = "test", 
                                 Id = new Identity("4de7e38617b6c420a45a84c4"), 
                                 ParentId = new Identity("4de7e38617b6c420a45a84c4")
                             };

            groupCommand.Executing(gc => gc.Execute(@group))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Texts.GroupCircleReference, group.Id));
        }


        [Test]
        public void WhenParentHasParentWithTheSameId_ThenThrowException()
        {
            var @group = new Group
            {
                Name = "test",
                Id = new Identity("4de7e38617b6c420a45a84c4"),
                ParentId = new Identity("4fffffff17b6c420a45a84c4")
            };

            var queryEntityById = Mock.Of<IEntityById>(q => 
                            q.Exist<Group>(It.IsAny<Identity>()) == true
                         && q.Get<Group>(new Identity("4fffffff17b6c420a45a84c4")) == new Group { ParentId = new Identity("4de7e38617b6c420a45a84c4") }
                         && q.Get<Group>(new Identity("4de7e38617b6c420a45a84c4")) == @group);

            var groupCommand = CreateUpdateGroupCommand(entityById: queryEntityById);

            

            groupCommand.Executing(gc => gc.Execute(@group))
                                    .Throws<ValidationException>()
                                    .And
                                    .Exception.Message.Should().Be.EqualTo(string.Format(Texts.GroupCircleReference, group.Id));
        }

        [Test]
        public void WhenEverythingIsOK_ThenUpdateTheGroup()
        {
            var id = Identity.Random();
            var stubRepository = new StubRepository<Group>();
            var groupCommand = CreateUpdateGroupCommand(entityById: Mock.Of<IEntityById>(q => q.Exist<Group>(id)),
            cudGroup: stubRepository);
            var @group = new Group { Id=id, Name = "test" };
            groupCommand.Execute(@group);

            stubRepository.Updates.Should().Contain(@group);

        }

        private static IUpdateGroupCommand CreateUpdateGroupCommand(
            IExistGroupByGroupName existGroupByGroupName = null,
            IEntityById entityById = null,
            IRepository<Group> cudGroup = null)
        {
            return new UpdateGroupCommand(existGroupByGroupName ?? Mock.Of<IExistGroupByGroupName>(),
                                        entityById ?? Mock.Of<IEntityById>(),
                                        cudGroup ?? Mock.Of<IRepository<Group>>());
        }
    }
}