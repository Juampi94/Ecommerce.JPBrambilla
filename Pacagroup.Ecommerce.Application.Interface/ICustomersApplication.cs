using System;
using System.Collections.Generic;
using System.Text;
using Pacagroup.Ecommerce.Application.DTO;
using Pacagroup.Ecommerce.Transversal.Common;
using System.Threading.Tasks;

namespace Pacagroup.Ecommerce.Application.Interface
{
    public interface ICustomersApplication
    {
        #region Métodos síncronos
        Response<bool> Insert(CustomersDTO customersDTO);
        Response<bool> Update(CustomersDTO customersDTO);
        Response<bool> Delete(string customerId);
        Response<CustomersDTO> Get(string customerId);
        Response<IEnumerable<CustomersDTO>> GetAll();

        #endregion

        #region Métodos asíncronos
        Task<Response<bool>> InsertAsync(CustomersDTO customersDTO);
        Task<Response<bool>> UpdateAsync(CustomersDTO customersDTO);
        Task<Response<bool>> DeleteAsync(string customerId);
        Task<Response<CustomersDTO>> GetAsync(string customerId);
        Task<Response<IEnumerable<CustomersDTO>>> GetAllAsync();

        #endregion
    }
}
