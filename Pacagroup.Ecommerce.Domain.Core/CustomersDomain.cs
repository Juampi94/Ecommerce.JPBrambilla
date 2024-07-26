using System;
using Pacagroup.Ecommerce.Domain.Entity;
using Pacagroup.Ecommerce.Domain.Interface;
using Pacagroup.Ecommerce.Infraestructure.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Pacagroup.Ecommerce.Domain.Core
{
    public class CustomersDomain : ICustomersDomain
    {
        private readonly ICustomersRepository _customersRepository;

        public CustomersDomain(ICustomersRepository customersRepository)
        {
            _customersRepository = customersRepository;
        }

        #region Métodos síncronos
        public bool Insert(Customers customer)
        {
            return this._customersRepository.Insert(customer);
        }
        public bool Update(Customers customer)
        {
            return _customersRepository.Update(customer);
        }
        public bool Delete(string customerId)
        {
            return _customersRepository.Delete(customerId);
        }
        public Customers Get(string customerId)
        {
            return _customersRepository.Get(customerId);
        }
        public IEnumerable<Customers> GetAll()
        {
            return _customersRepository.GetAll();
        }
        #endregion

        #region Métodos asíncronos
        public async Task<bool> InsertAsync(Customers customer)
        {
            return await _customersRepository.InsertAsync(customer);
        }
        public async Task<bool> UpdateAsync(Customers customer)
        {
            return await _customersRepository.UpdateAsync(customer);
        }
        public async Task<bool> DeleteAsync(string customerId)
        {
            return await _customersRepository.DeleteAsync(customerId);
        }

        public async Task<Customers> GetAsync(string customerId)
        {
            return await _customersRepository.GetAsync(customerId);
        }
        public async Task<IEnumerable<Customers>> GetAllAsync()
        {
            return await _customersRepository.GetAllAsync();
        }


        #endregion
    }
}
