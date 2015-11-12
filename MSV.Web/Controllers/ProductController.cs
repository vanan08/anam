using Accuracy.Core.Models;
using Accuracy.Core.Repositories;
using Accuracy.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;

namespace Accuracy.Web.Controllers
{
    [Authorize]
    public class ProductController : System.Web.Http.ApiController
    {
        private IRepository<Product> ProductRepository = new Repository<Product>();

        // GET: api/Product
        [System.Web.Http.HttpGet]
        public async Task<int> GetSize()
        {
            /*
            var user = (ClaimsIdentity)User.Identity;
            if (!user.IsAuthenticated)
            {
                
            }
            */
            var products = await ProductRepository.FindAllAsync();

            return products.Count();
        }

        // GET: api/Product
        [System.Web.Http.HttpGet]
        public async Task<IQueryable<ProductModel>> GetAll()
        {
            var products = await ProductRepository.FindAllAsync();
            
            return products.Select(x => new ProductModel
            {
                ProductID = x.ProductID,
                ProductName = x.ProductName,
                SupplierID = x.SupplierID.HasValue ? x.SupplierID.Value : int.MinValue,
                CategoryID = x.CategoryID.HasValue ? x.CategoryID.Value : int.MinValue,
                QuantityPerUnit = x.QuantityPerUnit,
                UnitPrice = x.UnitPrice.HasValue ? x.UnitPrice.Value : decimal.MinValue,
                UnitsInStock = x.UnitsInStock.HasValue ? x.UnitsInStock.Value : int.MinValue,
                UnitsOnOrder = x.UnitsOnOrder.HasValue ? x.UnitsOnOrder.Value : int.MinValue,
                ReorderLevel = x.ReorderLevel.HasValue ? x.ReorderLevel.Value : int.MinValue,
                Discontinued = x.Discontinued
            });
        }

        // GET: api/Product
        [System.Web.Http.HttpGet]
        public async Task<IQueryable<ProductModel>> GetAllBy(int page, int pageSize, string sortBy, bool isAsc)
        {
            var products = await ProductRepository.FindAllAsync();

            switch (sortBy.ToLower())
            {
                case "productid":
                    products = isAsc ? products.OrderBy(p => p.ProductID) : products.OrderByDescending(p => p.ProductID);
                    break;
                case "productname":
                    products = isAsc ? products.OrderBy(p => p.ProductName) : products.OrderByDescending(p => p.ProductName);
                    break;
                case "supplierid":
                    products = isAsc ? products.OrderBy(p => p.SupplierID) : products.OrderByDescending(p => p.SupplierID);
                    break;
                case "categoryid":
                    products = isAsc ? products.OrderBy(p => p.CategoryID) : products.OrderByDescending(p => p.CategoryID);
                    break;
                case "quantityperunit":
                    products = isAsc ? products.OrderBy(p => p.QuantityPerUnit) : products.OrderByDescending(p => p.QuantityPerUnit);
                    break;
                case "unitprice":
                    products = isAsc ? products.OrderBy(p => p.UnitPrice) : products.OrderByDescending(p => p.UnitPrice);
                    break;
                case "unitsinstock":
                    products = isAsc ? products.OrderBy(p => p.UnitsInStock) : products.OrderByDescending(p => p.UnitsInStock);
                    break;
                case "unitsonorder":
                    products = isAsc ? products.OrderBy(p => p.UnitsOnOrder) : products.OrderByDescending(p => p.UnitsOnOrder);
                    break;
                case "reorderlevel":
                    products = isAsc ? products.OrderBy(p => p.ReorderLevel) : products.OrderByDescending(p => p.ReorderLevel);
                    break;
                case "discontinued":
                    products = isAsc ? products.OrderBy(p => p.Discontinued) : products.OrderByDescending(p => p.Discontinued);
                    break;
            }

            return products.Select(x => new ProductModel
            {
                ProductID = x.ProductID,
                ProductName = x.ProductName,
                SupplierID = x.SupplierID.HasValue ? x.SupplierID.Value : int.MinValue,
                CategoryID = x.CategoryID.HasValue ? x.CategoryID.Value : int.MinValue,
                QuantityPerUnit = x.QuantityPerUnit,
                UnitPrice = x.UnitPrice.HasValue ? x.UnitPrice.Value : decimal.MinValue,
                UnitsInStock = x.UnitsInStock.HasValue ? x.UnitsInStock.Value : int.MinValue,
                UnitsOnOrder = x.UnitsOnOrder.HasValue ? x.UnitsOnOrder.Value : int.MinValue,
                ReorderLevel = x.ReorderLevel.HasValue ? x.ReorderLevel.Value : int.MinValue,
                Discontinued = x.Discontinued
            }).Skip((page - 1) * pageSize).Take(pageSize);
        }

        // GET: api/Product/5
        [ResponseType(typeof(ProductModel))]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            var product = await ProductRepository.FindOneByAsync(x => x.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductModel
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                SupplierID = product.SupplierID.HasValue ? product.SupplierID.Value : int.MinValue,
                CategoryID = product.CategoryID.HasValue ? product.CategoryID.Value : int.MinValue,
                QuantityPerUnit = product.QuantityPerUnit,
                UnitPrice = product.UnitPrice.HasValue ? product.UnitPrice.Value : decimal.MinValue,
                UnitsInStock = product.UnitsInStock.HasValue ? product.UnitsInStock.Value : int.MinValue,
                UnitsOnOrder = product.UnitsOnOrder.HasValue ? product.UnitsOnOrder.Value : int.MinValue,
                ReorderLevel = product.ReorderLevel.HasValue ? product.ReorderLevel.Value : int.MinValue,
                Discontinued = product.Discontinued
            };

            return Ok(model);
        }

        // POST: api/Product
        [ResponseType(typeof(Product))]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var model = new Product()
                {
                    ProductName = product.ProductName,
                    SupplierID = product.SupplierID,
                    CategoryID = product.CategoryID,
                    QuantityPerUnit = product.QuantityPerUnit,
                    UnitPrice = product.UnitPrice,
                    UnitsInStock = product.UnitsInStock,
                    UnitsOnOrder = product.UnitsOnOrder,
                    ReorderLevel = product.ReorderLevel,
                    Discontinued = product.Discontinued
                };

                ProductRepository.Save(model);
            }
            catch (Exception)
            {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);
        }

        // DELETE: api/Product/5
        [ResponseType(typeof(Product))]
        [System.Web.Http.HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var product = ProductRepository.FindOneBy(x => x.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            await ProductRepository.DeleteAsync(product);

            return Ok(product);
        }

        // PUT: api/Product/5
        [ResponseType(typeof(void))]
        [System.Web.Http.HttpPut]
        public async Task<IHttpActionResult> Edit(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductID)
            {
                return BadRequest();
            }

            try
            {
                var model = ProductRepository.FindOneBy(x => x.ProductID == id);

                if (model != null)
                {
                    model.ProductName = product.ProductName;
                    model.SupplierID = product.SupplierID;
                    model.CategoryID = product.CategoryID;
                    model.QuantityPerUnit = product.QuantityPerUnit;
                    model.UnitPrice = product.UnitPrice;
                    model.UnitsInStock = product.UnitsInStock;
                    model.UnitsOnOrder = product.UnitsOnOrder;
                    model.ReorderLevel = product.ReorderLevel;
                    model.Discontinued = product.Discontinued;
                }

                await ProductRepository.UpdateAsync(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                var productToEdit = ProductRepository.FindOneBy(x => x.ProductID == id);

                if (productToEdit != null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
