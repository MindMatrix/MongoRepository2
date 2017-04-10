namespace MongoRepository2.Tests
{
    using Xunit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoRepository2;
    using MongoRepository2.Tests.Entities;

    public abstract class AbstractRepositoryTests : AbstractRepository
    {

        #region Add

        [Fact]
        public void AddSync()
        {
            IRepository<Customer> _customerRepo = CreateRandomRepository<Customer>();
            var customer = new Customer()
            {
                FirstName = "Bob",
                LastName = "Dillon",
                Phone = "0900999899",
                Email = "Bob.dil@snailmail.com",
                HomeAddress = new Address
                {
                    Address1 = "North kingdom 15 west",
                    Address2 = "1 north way",
                    PostCode = "40990",
                    City = "George Town",
                    Country = "Alaska"
                }
            };

            _customerRepo.Add(customer);
            Assert.NotNull(customer.Id);
            Assert.Equal(1, _customerRepo.Count());
        }

        [Fact]
        public async void AddAsync()
        {
            IRepository<Customer> _customerRepo = CreateRandomRepository<Customer>();
            var customer = new Customer()
            {
                FirstName = "Bob",
                LastName = "Dillon",
                Phone = "0900999899",
                Email = "Bob.dil@snailmail.com",
                HomeAddress = new Address
                {
                    Address1 = "North kingdom 15 west",
                    Address2 = "1 north way",
                    PostCode = "40990",
                    City = "George Town",
                    Country = "Alaska"
                }
            };
            await _customerRepo.AddAsync(customer);
            Assert.NotNull(customer.Id);
            Assert.Equal(1, await _customerRepo.CountAsync());
        }

        #endregion Add

        #region Update

        [Fact]
        public void UpdateEntitySync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            //var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 1 };

            _products.Add(product1);
            //_products.Add(new Product[] { product1, product2 });

            Assert.Equal(1, _products.Count());
            var product2 = new Product() { Id = product1.Id, Name = product1.Name, Description = product1.Description, Price = 2 };
            //var product4 = new Product() { Id = product2.Id, Name = product2.Name, Description = product2.Description, Price = 4 };

            _products.Update(product2);
            Assert.Equal(1, _products.Count());

            {
                var actual = _products.GetById(product2.Id);
                Assert.Equal(product2.Price, actual.Price);
            }
            {
                var actual = _products.GetById(product1.Id);
                Assert.Equal(product2.Price, actual.Price);
            }
        }

        [Fact]
        public void UpdateEntityNoUpsertSync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Id = ObjectId.GenerateNewId().ToString(), Name = "Product1", Description = "Product1", Price = 1 };
            //var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 1 };

            _products.Update(product1);
            //_products.Add(new Product[] { product1, product2 });

            Assert.Equal(0, _products.Count());
            //var product2 = new Product() { Id = product1.Id, Name = product1.Name, Description = product1.Description, Price = 2 };
            ////var product4 = new Product() { Id = product2.Id, Name = product2.Name, Description = product2.Description, Price = 4 };

            //_products.Update(product2);
            //Assert.Equal(1, _products.Count());

            //{
            //    var actual = _products.GetById(product2.Id);
            //    Assert.Equal(product2.Price, actual.Price);
            //}
            //{
            //    var actual = _products.GetById(product1.Id);
            //    Assert.Equal(product2.Price, actual.Price);
            //}
        }

        [Fact]
        public async void UpdateEntityAsync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 1 };

            await _products.AddAsync(new Product[] { product1, product2 });

            Assert.Equal(2, await _products.CountAsync());
            product1.Price = 2;
            await _products.UpdateAsync(product1);
            Assert.Equal(2, await _products.CountAsync());

            var actual = await _products.GetByIdAsync(product1.Id);
            Assert.Equal(product1.Price, actual.Price);
        }

        [Fact]
        public void UpdateManySync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 1 };

            _products.Add(new Product[] { product1, product2 });

            Assert.Equal(2, _products.Count());
            product1.Price = 2;
            product2.Price = 2;
            _products.Update(new Product[] { product1, product2 });
            Assert.Equal(2, _products.Count());

            var actual1 = _products.GetById(product1.Id);
            Assert.Equal(product1.Price, actual1.Price);

            var actual2 = _products.GetById(product2.Id);
            Assert.Equal(product2.Price, actual2.Price);
        }

        [Fact]
        public async void UpdateManyAsync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 1 };

            await _products.AddAsync(new Product[] { product1, product2 });

            Assert.Equal(2, await _products.CountAsync());
            product1.Price = 2;
            product2.Price = 2;
            await _products.UpdateAsync(new Product[] { product1, product2 });
            Assert.Equal(2, await _products.CountAsync());

            var actual = await _products.GetByIdAsync(product1.Id);
            Assert.Equal(product1.Price, actual.Price);

            var actual2 = await _products.GetByIdAsync(product2.Id);
            Assert.Equal(product2.Price, actual2.Price);
        }

        #endregion Update

        #region Exists

        [Fact]
        public void ExistsSync()
        {
            IRepository<Customer> _customerRepo = CreateRandomRepository<Customer>();
            var customer = new Customer()
            {
                FirstName = "Exists",
                LastName = "Sync",
                Phone = "0900999899",
                Email = "Bob.dil@snailmail.com",
                HomeAddress = new Address
                {
                    Address1 = "North kingdom 15 west",
                    Address2 = "1 north way",
                    PostCode = "40990",
                    City = "George Town",
                    Country = "Alaska"
                }
            };

            _customerRepo.Add(customer);
            Assert.True(_customerRepo.Exists(c => c.FirstName == "Exists" && c.LastName == "Sync"));
        }

        [Fact]
        public async void ExistsAsync()
        {
            IRepository<Customer> _customerRepo = CreateRandomRepository<Customer>();
            var customer = new Customer()
            {
                FirstName = "Exists",
                LastName = "Async",
                Phone = "0900999899",
                Email = "Bob.dil@snailmail.com",
                HomeAddress = new Address
                {
                    Address1 = "North kingdom 15 west",
                    Address2 = "1 north way",
                    PostCode = "40990",
                    City = "George Town",
                    Country = "Alaska"
                }
            };

            await _customerRepo.AddAsync(customer);
            Assert.True(await _customerRepo.ExistsAsync(c => c.FirstName == "Exists" && c.LastName == "Async"));
        }

        #endregion Exists

        #region Delete

        [Fact]
        public void DeleteOneEntitySync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 1 };

            _products.Add(new Product[] { product1, product2 });

            Assert.Equal(2, _products.Count());
            _products.Delete(product1);
            Assert.Equal(1, _products.Count());
        }

        [Fact]
        public void DeleteOneIdSync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 1 };

            _products.Add(new Product[] { product1, product2 });

            Assert.Equal(2, _products.Count());
            _products.Delete(product1.Id);
            Assert.Equal(1, _products.Count());
        }

        [Fact]
        public void DeleteManyPredicateSync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 2 };
            var product3 = new Product() { Name = "Product3", Description = "Product3", Price = 3 };
            var product4 = new Product() { Name = "Product4", Description = "Product4", Price = 4 };
            var product5 = new Product() { Name = "Product5", Description = "Product5", Price = 5 };

            _products.Add(new Product[] { product1, product2, product3, product4, product5 });

            Assert.Equal(5, _products.Count());
            _products.Delete(p => p.Price >= 3);
            Assert.Equal(2, _products.Count());
        }

        [Fact]
        public void DeleteAllSync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 2 };
            var product3 = new Product() { Name = "Product3", Description = "Product3", Price = 3 };
            var product4 = new Product() { Name = "Product4", Description = "Product4", Price = 4 };
            var product5 = new Product() { Name = "Product5", Description = "Product5", Price = 5 };

            _products.Add(new Product[] { product1, product2, product3, product4, product5 });

            Assert.Equal(5, _products.Count());
            _products.DeleteAll();
            Assert.Equal(0, _products.Count());
        }

        [Fact]
        public async void DeleteOneIdAsync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 1 };

            await _products.AddAsync(new Product[] { product1, product2 });

            Assert.Equal(2, await _products.CountAsync());
            await _products.DeleteAsync(product1.Id);
            Assert.Equal(1, await _products.CountAsync());
        }

        [Fact]
        public async void DeleteManyPredicateAsync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 2 };
            var product3 = new Product() { Name = "Product3", Description = "Product3", Price = 3 };
            var product4 = new Product() { Name = "Product4", Description = "Product4", Price = 4 };
            var product5 = new Product() { Name = "Product5", Description = "Product5", Price = 5 };

            await _products.AddAsync(new Product[] { product1, product2, product3, product4, product5 });

            Assert.Equal(5, await _products.CountAsync());
            await _products.DeleteAsync(p => p.Price >= 3);
            Assert.Equal(2, await _products.CountAsync());
        }

        [Fact]
        public async void DeleteAllAsync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 2 };
            var product3 = new Product() { Name = "Product3", Description = "Product3", Price = 3 };
            var product4 = new Product() { Name = "Product4", Description = "Product4", Price = 4 };
            var product5 = new Product() { Name = "Product5", Description = "Product5", Price = 5 };

            await _products.AddAsync(new Product[] { product1, product2, product3, product4, product5 });

            Assert.Equal(5, await _products.CountAsync());
            await _products.DeleteAllAsync();
            Assert.Equal(0, await _products.CountAsync());
        }

        [Fact]
        public async void DeleteOneEntityAsync()
        {
            IRepository<Product> _products = CreateRandomRepository<Product>();
            var product1 = new Product() { Name = "Product1", Description = "Product1", Price = 1 };
            var product2 = new Product() { Name = "Product2", Description = "Product2", Price = 1 };

            await _products.AddAsync(new Product[] { product1, product2 });

            Assert.Equal(2, await _products.CountAsync());
            await _products.DeleteAsync(product1);
            Assert.Equal(1, await _products.CountAsync());
        }
        #endregion Delete

        [Fact]
        public void ComplexEntityTest()
        {
            IRepository<Customer> _customerRepo = CreateRandomRepository<Customer>();
            IRepository<Product> _productRepo = CreateRandomRepository<Product>();

            var customer = new Customer();
            customer.FirstName = "Erik";
            customer.LastName = "Swaun";
            customer.Phone = "123 99 8767";
            customer.Email = "erick@mail.com";
            customer.HomeAddress = new Address
            {
                Address1 = "Main bulevard",
                Address2 = "1 west way",
                PostCode = "89560",
                City = "Tempare",
                Country = "Arizona"
            };

            var order = new Order();
            order.PurchaseDate = DateTime.Now.AddDays(-2);
            var orderItems = new List<OrderItem>();

            var shampoo = _productRepo.Add(new Product() { Name = "Palmolive Shampoo", Price = 5 });
            var paste = _productRepo.Add(new Product() { Name = "Mcleans Paste", Price = 4 });


            var item1 = new OrderItem { Product = shampoo, Quantity = 1 };
            var item2 = new OrderItem { Product = paste, Quantity = 2 };

            orderItems.Add(item1);
            orderItems.Add(item2);

            order.Items = orderItems;

            customer.Orders = new List<Order>
            {
                order
            };

            _customerRepo.Add(customer);

            Assert.NotNull(customer.Id);
            Assert.NotNull(customer.Orders[0].Items[0].Product.Id);

            // get the orders  
            var theOrders = _customerRepo.Where(c => c.Id == customer.Id).Select(c => c.Orders).ToList();
            var theOrderItems = theOrders[0].Select(o => o.Items);

            Assert.NotNull(theOrders);
            Assert.NotNull(theOrderItems);
        }


        [Fact]
        public void BatchTest()
        {
            IRepository<Customer> _customerRepo = CreateRandomRepository<Customer>();

            var custlist = new List<Customer>(new Customer[] {
                new Customer() { FirstName = "Customer A" },
                new Customer() { FirstName = "Client B" },
                new Customer() { FirstName = "Customer C" },
                new Customer() { FirstName = "Client D" },
                new Customer() { FirstName = "Customer E" },
                new Customer() { FirstName = "Client F" },
                new Customer() { FirstName = "Customer G" },
            });

            //Insert batch
            _customerRepo.Add(custlist);

            var count = _customerRepo.Count();
            Assert.Equal(7, count);
            foreach (Customer c in custlist)
                Assert.NotEqual(new string('0', 24), c.Id);

            //Update batch
            foreach (Customer c in custlist)
                c.LastName = c.FirstName;
            _customerRepo.Update(custlist);

            foreach (Customer c in _customerRepo)
                Assert.Equal(c.FirstName, c.LastName);

            //Delete by criteria
            _customerRepo.Delete(f => f.FirstName.StartsWith("Client"));

            count = _customerRepo.Count();
            Assert.Equal(4, count);

            //Delete specific object
            _customerRepo.Delete(custlist[0]);

            //Test AsQueryable
            var selectedcustomers = from cust in _customerRepo
                                    where cust.LastName.EndsWith("C") || cust.LastName.EndsWith("G")
                                    select cust;

            Assert.Equal(2, selectedcustomers.ToList().Count);

            count = _customerRepo.Count();
            Assert.Equal(3, count);

            ////Drop entire repo
            //new MongoRepositoryManager<Customer>().Drop();

            //count = _customerRepo.Count();
            //Assert.Equal(0, count);
        }

        [Fact]
        public void CollectionNamesTest()
        {
            var a = CreateRepository<Animal>();
            //var am = new MongoRepositoryManager<Animal>();
            var va = new Dog();
            //Assert.False(am.Exists);
            a.AddOrUpdate(va);
            //Assert.True(am.Exists);
            Assert.IsType<Dog>(a.GetById(va.Id));
            //Assert.Equal(am.Name, "AnimalsTest");
            Assert.Equal(a.CollectionName, "AnimalsTest");

            var cl = CreateRepository<CatLike>();
            //var clm = new MongoRepositoryManager<CatLike>();
            var vcl = new Lion();
            //Assert.False(clm.Exists);
            cl.AddOrUpdate(vcl);
            //Assert.True(clm.Exists);
            Assert.IsType<Lion>(cl.GetById(vcl.Id));
            //Assert.Equal(clm.Name, "Catlikes");
            Assert.Equal(cl.CollectionName, "Catlikes");

            var b = CreateRepository<Bird>();
            //var bm = new MongoRepositoryManager<Bird>();
            var vb = new Bird();
            //Assert.False(bm.Exists);
            b.AddOrUpdate(vb);
            //Assert.True(bm.Exists);
            Assert.IsType<Bird>(b.GetById(vb.Id));
            //Assert.Equal(bm.Name, "Birds");
            Assert.Equal(b.CollectionName, "Birds");

            var l = CreateRepository<Lion>();
            //var lm = new MongoRepositoryManager<Lion>();
            var vl = new Lion();
            //Assert.False(lm.Exists);   //Should already exist (created by cl)
            l.AddOrUpdate(vl);
            //Assert.True(lm.Exists);
            Assert.IsType<Lion>(l.GetById(vl.Id));
            //Assert.Equal(lm.Name, "Catlikes");
            Assert.Equal(l.CollectionName, "Catlikes");

            var d = CreateRepository<Dog>();
            //var dm = new MongoRepositoryManager<Dog>();
            var vd = new Dog();
            //Assert.False(dm.Exists);
            d.AddOrUpdate(vd);
            //Assert.True(dm.Exists);
            Assert.IsType<Dog>(d.GetById(vd.Id));
            //Assert.Equal(dm.Name, "AnimalsTest");
            Assert.Equal(d.CollectionName, "AnimalsTest");

            var m = CreateRepository<Bird>();
            //var mm = new MongoRepositoryManager<Bird>();
            var vm = new Macaw();
            //Assert.False(mm.Exists);
            m.AddOrUpdate(vm);
            //Assert.True(mm.Exists);
            Assert.IsType<Macaw>(m.GetById(vm.Id));
            //Assert.Equal(mm.Name, "Birds");
            Assert.Equal(m.CollectionName, "Birds");

            var w = CreateRepository<Whale>();
            //var wm = new MongoRepositoryManager<Whale>();
            var vw = new Whale();
            //Assert.False(wm.Exists);
            w.AddOrUpdate(vw);
            // Assert.True(wm.Exists);
            Assert.IsType<Whale>(w.GetById(vw.Id));
            //Assert.Equal(wm.Name, "Whale");
            Assert.Equal(w.CollectionName, "Whale");
        }

        [Fact]
        public void CustomIDTest()
        {
            var x = CreateRepository<CustomIDEntity>();
            //var xm = new MongoRepositoryManager<CustomIDEntity>();

            x.Add(new CustomIDEntity() { Id = "aaa" });

            //Assert.True(xm.Exists);
            Assert.IsType<CustomIDEntity>(x.GetById("aaa"));

            Assert.Equal("aaa", x.GetById("aaa").Id);

            x.Delete("aaa");
            Assert.Equal(0, x.Count());

            var y = CreateRepository<CustomIDEntityCustomCollection>();
            //var ym = new MongoRepositoryManager<CustomIDEntityCustomCollection>();

            y.Add(new CustomIDEntityCustomCollection() { Id = "xyz" });

            //Assert.True(ym.Exists);
            //Assert.Equal(ym.Name, "MyTestCollection");
            Assert.Equal(y.CollectionName, "MyTestCollection");
            Assert.IsType<CustomIDEntityCustomCollection>(y.GetById("xyz"));

            y.Delete("xyz");
            Assert.Equal(0, y.Count());
        }

        [Fact]
        public void CustomIDTypeTest()
        {
            var xint = CreateRepository<IntCustomer, int>();
            xint.Add(new IntCustomer() { Id = 1, Name = "Test A" });
            xint.Add(new IntCustomer() { Id = 2, Name = "Test B" });

            var yint = xint.GetById(2);
            Assert.Equal(yint.Name, "Test B");

            xint.Delete(2);
            Assert.Equal(1, xint.Count());
        }

        [Fact]
        public void OverrideCollectionName()
        {
            IRepository<Customer> _customerRepo = CreateRepository<Customer>("TestCustomers123");
            _customerRepo.Add(new Customer() { FirstName = "Test" });
            Assert.True(_customerRepo.Single().FirstName.Equals("Test"));
            Assert.Equal("TestCustomers123", _customerRepo.CollectionName);
            //            Assert.Equal("TestCustomers123", ((MongoRepository<Customer>)_customerRepo).CollectionName);

            //IRepositoryManager<Customer> _curstomerRepoManager = new MongoRepositoryManager<Customer>("mongodb://localhost/MongoRepositoryTests", "TestCustomers123");
            //Assert.Equal("TestCustomers123", _curstomerRepoManager.Name);
        }

        #region Reproduce issue: https://mongorepository.codeplex.com/discussions/433878
        public abstract class BaseItem : IEntity
        {
            public string Id { get; set; }
        }

        public abstract class BaseA : BaseItem
        { }

        public class SpecialA : BaseA
        { }

        [Fact]
        public void Discussion433878()
        {
            var specialRepository = CreateRandomRepository<SpecialA>();
        }
        #endregion

        #region Reproduce issue: https://mongorepository.codeplex.com/discussions/572382
        public abstract class ClassA : MongoRepository2.Entity
        {
            public string Prop1 { get; set; }
        }

        public class ClassB : ClassA
        {
            public string Prop2 { get; set; }
        }

        public class ClassC : ClassA
        {
            public string Prop3 { get; set; }
        }

        [Fact]
        public void Discussion572382()
        {
            var repo = CreateRandomRepository<ClassA>();
            repo.Add(new ClassB() { Prop1 = "A", Prop2 = "B" });
            repo.Add(new ClassC() { Prop1 = "A", Prop3 = "C" });

            Assert.Equal(2, repo.Count());

            Assert.Equal(2, repo.OfType<ClassA>().Count());
            Assert.Equal(1, repo.OfType<ClassB>().Count());
            Assert.Equal(1, repo.OfType<ClassC>().Count());
        }
        #endregion

    }
}
