using AdoDemo.Web.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AdoDemo.Web.Repository
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly CustomerDAO _customerDAO;
        public CustomerRepository(IConfiguration configuration)
        {
            _customerDAO = new CustomerDAO(configuration);
        }

        /// <summary>
        /// Add Customer
        /// </summary>
        /// <param name="item"></param>
        public void Add(Customer item)
        {
            var sql = "INSERT INTO customer (Name,Phone,Email,Address) VALUES("
                + _customerDAO.FilterString(item.Name)
                +","+ _customerDAO.FilterString(item.Phone)
                +"," + _customerDAO.FilterString(item.Email)
                +"," + _customerDAO.FilterString(item.Address)
                +")";

            _customerDAO.ExecuteDataRow(sql);
        }

        /// <summary>
        /// Get All Customer
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> FindAll()
        {
            var customerList = new List<Customer>();
            var sql = "SELECT * FROM customer";
            var result = _customerDAO.ExecuteDataTable(sql);
            if(result != null)
            {
                customerList = (from DataRow dr in result.Rows
                               select new Customer()
                               {
                                   Id = Convert.ToInt32(dr["Id"]),
                                   Name = dr["Name"].ToString(),
                                   Address = dr["Address"].ToString(),
                                   Phone = dr["Phone"].ToString(),
                                   Email = dr["Email"].ToString()
                               }).ToList();
            }

            return customerList;
        }

        /// <summary>
        /// Find Customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Customer FindByID(int id)
        {
            var customer = new Customer();
            var sql = "SELECT * FROM customer WHERE "
                 + "Id = " + id;
           var result = _customerDAO.ExecuteDataRow(sql);
            if(result != null)
            {
                customer.Id = Convert.ToInt64(result["Id"]);
                customer.Name = result["Name"].ToString();
                customer.Phone = result["Phone"].ToString();
                customer.Address = result["Address"].ToString();
                customer.Email = result["Email"].ToString();
            }
            return customer;
        }

        /// <summary>
        /// Delete Customer by Id
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            var sql = "DELETE FROM customer WHERE"
                +"Id="+ id;
            _customerDAO.ExecuteDataRow(sql);
        }

        /// <summary>
        /// Update Customer 
        /// </summary>
        /// <param name="item"></param>
        public void Update(Customer item)
        {
            var sql = "UPDATE customer SET "
                 + "name =" + _customerDAO.FilterString(item.Name)
                + ",phone =" + _customerDAO.FilterString(item.Phone)
                + ",email =" + _customerDAO.FilterString(item.Email)
                + ",address =" + _customerDAO.FilterString(item.Address)
                + " WHERE "
                +"id =" + item.Id;

            _customerDAO.ExecuteDataRow(sql);
        }
    }
}
