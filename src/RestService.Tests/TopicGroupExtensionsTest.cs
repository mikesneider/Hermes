﻿using System;
using SharpTestsEx;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.RestService.Extensions;
using TellagoStudios.Hermes.RestService.Resources;
using M = TellagoStudios.Hermes.Business.Model;
using NUnit.Framework;
using TellagoStudios.Hermes.Facade;

namespace RestService.Tests
{
    [TestFixture]
    public class GroupExtensionsTest
    {
        private string _description;
        private Identity _id;
        private string _name;
        private Identity _parentId;

        [SetUp]
        public void SetUp()
        {
            ResourceLocation.BaseAddress = new Uri("http://localhost");
            _description = "desc";
            _id = Identity.Random();
            _name = "Hey hey!";
            _parentId = Identity.Random();
        }

        [Test]
        public void FacadePutToModelMapsCorrectly()
        {
            var facade = new GroupPut
                             {
                                 Description = _description,
                                 Id = _id,
                                 Name = _name,
                                 ParentId = _parentId
                             };

            var model = facade.ToModel();

            Assert.That(model.Description, Is.EqualTo(_description));
            Assert.That(model.Name, Is.EqualTo(_name));
            Assert.That(model.Id, Is.EqualTo(_id.ToModel()));
            Assert.That(model.ParentId, Is.EqualTo(_parentId.ToModel()));
        }

        [Test]
        public void FacadePostToModelMapsCorrectly()
        {
            var facade = new GroupPost
            {
                Description = _description,
                Name = _name,
                ParentId = _parentId
            };

            var model = facade.ToModel();

            Assert.That(model.Id, Is.Null);
            Assert.That(model.Description, Is.EqualTo(_description));
            Assert.That(model.Name, Is.EqualTo(_name));
            Assert.That(model.ParentId, Is.EqualTo(_parentId.ToModel()));
        }

        [Test]
        public void ModelToFacadeMapsCorrectly()
        {
            var model = new M.Group
            {
                Description = _description,
                Id = _id.ToModel(),
                Name = _name,
                ParentId = _parentId.ToModel()
            };

            var facade = model.ToFacade();

            Assert.That(facade.Description, Is.EqualTo(_description));
            Assert.That(facade.Name, Is.EqualTo(_name));
            Assert.That(facade.Id, Is.EqualTo(_id));
            facade.Links.Should().Contain(new Link(ResourceLocation.OfGroup(_parentId.ToModel()),
                                                   TellagoStudios.Hermes.RestService.Constants.Relationships.Parent));
        }

        [Test]
        public void GroupsToLinksMapCorrectly()
        {
            var group = new Group { Id = _id };
            var link = group.Id.ToModel().ToLink(TellagoStudios.Hermes.RestService.Constants.Relationships.Group);

            Assert.That(link.Relation, Is.EqualTo(TellagoStudios.Hermes.RestService.Constants.Relationships.Group));
            Assert.That(link.Uri, Is.EqualTo(ResourceLocation.OfGroup(_id.ToModel())));
        }
    }
}