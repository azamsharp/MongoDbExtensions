using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Collections;
using System.Reflection;
using System.Web.Script.Serialization;

namespace MongoDbExtensions
{

    public static class MongoExtensions
    {
        public static List<T> ToList<T>(this IEnumerable<Document> documents)
        {
            var list = new List<T>();

            var enumerator = documents.GetEnumerator();

            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current.ToClass<T>());
            }

            return list;
        }

        public static string BetterInsert(this IMongoCollection collection, Document document)
        {

            collection.Insert(document);
            return document["_id"].ToString().Replace("\"", "");
        }

        public static T ToClass<T>(this Document source)
        {
            try
            {
                var json = new JavaScriptSerializer();

                if (source == null) throw new ArgumentNullException("Document is null!");

                return json.Deserialize<T>(source.ToString());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static Document ToDocument(this object source)
        {
            var document = SerializeMember(source) as Document;
            return document;
        }

        public static Oid ToOid(this string str)
        {
            if (str.Length == 24) return new Oid(str);

            return new Oid(str.Replace("\"", ""));
        }

        private static object SerializeMember(object source)
        {
            // get the properties 

            if (!Type.GetTypeCode(source.GetType()).Equals(TypeCode.Object))
                return source;

            if (source is Oid)
                return source;

            // if the object is IEnumerable 

            var enumerable = source as IEnumerable;

            if (enumerable != null)
            {
                return (from object doc in enumerable select SerializeMember(doc)).ToArray();
            }

            var properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var document = new Document();

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(source, null);

                if (propertyValue == null) continue;

                if (property.Name.Equals("_id"))
                    document.Add(property.Name, (((string)propertyValue).ToOid()));
                else
                    document.Add(property.Name, SerializeMember(propertyValue));

            }

            return document;
        }

    }
}
