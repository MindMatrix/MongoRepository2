namespace MongoRepository2.Tests.Entities
{
    using System;
    using MongoRepository2;
    using MongoDB.Bson.Serialization.Attributes;

    public class CustomIDEntity : IEntity
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }

    [CollectionName("MyTestCollection")]
    public class CustomIDEntityCustomCollection : CustomIDEntity { }
}
