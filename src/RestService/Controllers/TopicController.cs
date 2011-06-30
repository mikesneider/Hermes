﻿using System;
using System.Web.Mvc;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Models;

namespace TellagoStudios.Hermes.RestService.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicsSortedByName topicsSortedByName;
        private readonly IGroupsSortedByName groupsSortedByName;
        private readonly IEntityById entityById;
        private readonly IRepository<Topic> topicRepository;

        public TopicController(
            ITopicsSortedByName topicsSortedByName, 
            IGroupsSortedByName groupsSortedByName,
            IEntityById entityById,
            IRepository<Topic> topicRepository)
        {
            this.topicsSortedByName = topicsSortedByName;
            this.groupsSortedByName = groupsSortedByName;
            this.entityById = entityById;
            this.topicRepository = topicRepository;
        }

        public ActionResult Index()
        {
            return View(topicsSortedByName.Execute());
        }

        public ActionResult Edit(string id)
        {
            var identity = new Identity(id);
            var entity = entityById.Get<Topic>(identity);
            if(entity == null) return new HttpStatusCodeResult(404);
            var model = new EditTopicModel(entity, groupsSortedByName.Execute());
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditTopicModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var entity = entityById.Get<Topic>((Identity) model.TopicId);
            ModelToEntity(model, entity);
            topicRepository.Update(entity);
            return RedirectToAction("Index");
        }

        private static void ModelToEntity(EditTopicModel model, Topic entity)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
            if (!string.IsNullOrEmpty(model.Group))
            {
                entity.GroupId = (Identity) model.Group;
            }
        }

        public ActionResult Delete(Identity id)
        {
            throw new NotImplementedException();
        }

        public ActionResult Create()
        {
            var model = new EditTopicModel(this.groupsSortedByName.Execute());
            return View(model);   
        }

        [HttpPost]
        public ActionResult Create(EditTopicModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var topic = new Topic();
            ModelToEntity(model, topic);
            topicRepository.MakePersistent(topic);
            return RedirectToAction("Index");
        }
    }
}