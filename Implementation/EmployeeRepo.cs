using System.Data;
using sample.Abstract;
using sample.Models;

namespace sample.Implementation
{
    public class EmployeeRepo : Repository<Employee>, IEmployee
    {

        private readonly IDbConnection _db;

        public EmployeeRepo(IDbConnection _db, string _tbl="tblEmployee") : base(_db, _tbl)
        {
            this._db = _db;
          
        }
    }
}