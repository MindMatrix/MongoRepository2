namespace MongoRepository2.Tests.Entities
{
    using MongoRepository2;
    
    /// <summary>
    /// Business Entity for Product
    /// </summary>
    public class Product : Entity
    {
        public Product()
        {
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}
