using Accuracy.Core.Models;
using Accuracy.Core.Repositories;
using CMC.Northwind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Accuracy.Web.Controllers
{
    public class SupplierController : ApiController
    {
        private IRepository<Supplier> SupplierRepository = new Repository<Supplier>();

        // GET: api/Supplier
        [System.Web.Http.HttpGet]
        public int GetSize()
        {
            return SupplierRepository.FindAll().Count();
        }

        // GET: api/Supplier
        [System.Web.Http.HttpGet]
        public IQueryable<SupplierModel> GetAll()
        {
            return SupplierRepository.FindAll().Select(x => new SupplierModel
            {
                SupplierID = x.SupplierID,
                CompanyName = x.CompanyName,
                ContactName = x.ContactName,
                ContactTitle = x.ContactTitle,
                Address = x.Address,
                City = x.City,
                Region = x.Region,
                PostalCode = x.PostalCode,
                Country = x.Country,
                Phone = x.Phone,
                Fax = x.Fax,
                HomePage = x.HomePage
            }).OrderBy(x => x.ContactName);
        }
    }
}
