using Dapper;
using Pacagroup.Ecommerce.Domain.Entity;
using Pacagroup.Ecommerce.Infraestructure.Interface;
using Pacagroup.Ecommerce.Transversal.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Pacagroup.Ecommerce.Infraestructure.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public UsersRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Users Authenticate(string username, string password)
        {
            using (var connection = _connectionFactory.GetConnection)
            {
                var query = "UsersGetByUserAndPassword";
                var param = new DynamicParameters();
                param.Add("UserName", username);
                param.Add("Password", password);

                return connection.QuerySingle<Users>(query, param, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
