﻿using System;
using System.Text;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;
using Messages = TellagoStudios.Hermes.DataAccess.Messages;

namespace MongoDB.Bson
{
    public static class MongoDbBsonExtensions
    {
        public static BsonValue ToBsonValue(this string valueInJsonFormat)
        {
            if (string.IsNullOrWhiteSpace(valueInJsonFormat))
            {
                return BsonNull.Value;
            }

            try
            {
                using (var reader = BsonReader.Create(valueInJsonFormat))
                {
                    return BsonValue.ReadFrom(reader);
                }
            }
            catch (Exception e)
            {
                string text = String.Format(Messages.InvalidJsonValue, valueInJsonFormat ?? "is null");
                throw new ArgumentException(text, "valueInJsonFormat", e);
            }
        }

        public static QueryDocument ToQuery(this Identity? id)
        {
            Guard.Instance.ArgumentNotNull(() => id, id);

            return new QueryDocument("_id", new BsonObjectId((byte[]) id.Value));
        }

        public static BsonValue ToBson(this Identity? id)
        {
            if (id.HasValue) return new BsonObjectId((byte[])id);
            
            return BsonNull.Value;
        }

        public static BsonObjectId ToBson(this Identity id)
        {
            return new BsonObjectId((byte[])id);
        }

        public static string ToBsonString(this Identity id)
        {
            return "ObjectId(\"" + id + "\")";
        }

        public static string ToBsonString(this Identity? guid)
        {
            if (guid.HasValue) return guid.Value.ToBsonString();

            return "null";
        }
    }
}