using System.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sample.Abstract.IREPO;
using Dapper;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace sample.Implementation
{
    public class Repository<T> : IRepo<T> where T : class
    {

        private readonly IDbConnection _db;
        private string _tbl;
        public Repository(IDbConnection _db, string _tbl)
        {
            this._db = _db ?? throw new ArgumentException(nameof(_db));
            this._tbl = _tbl;
        }
        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }
        public async Task Delete(object id)
        {
            using (var db = _db)
            {
                db.Open();
                var cmd = $"DELETE FROM {_tbl} where id = {id}";
                await db.ExecuteAsync(cmd);
            }
        }

        public async Task<IEnumerable<T>> Filter(Func<T, bool> predicate)
        {
            using (var db = _db)
            {
                db.Open();
                var list = await GetAll();
                return list.Where(predicate).ToList();
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (var db = _db)
            {
                db.Open();
                var cmd = $"SELECT * FROM {_tbl}";
                return await SqlMapper.QueryAsync<T>(db, cmd);

            }
        }

        public async Task<T> GetById(object id)
        {
            using (var db = _db)
            {
                db.Open();
                var cmd = $"SELECT * FROM {_tbl} where id= {id}";
                return await SqlMapper.QueryFirstAsync<T>(db, cmd);

            }
        }

        public async Task Save(T entity)
        {
            var insertQuery = GenerateInsertQuery();

            using (var db = _db)
            {
                await db.ExecuteAsync(insertQuery, param: entity);
            }
        }

        public async Task Update(T entity)
        {
            var updateQuery = GenerateUpdateQuery();

            using (var connection = _db)
            {
                await connection.ExecuteAsync(updateQuery, entity);
            }
        }

        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tbl} ");

            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(prop =>
            {
                if (prop != "id")
                {
                    insertQuery.Append($"[{prop}],");

                }
            });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");


            properties.ForEach(prop =>
            {
                if (prop != "id")
                {
                    insertQuery.Append($"@{prop},");
                }
            });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString();
        }

        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tbl} SET ");
            var properties = GenerateListOfProperties(GetProperties);

            properties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });

            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            if (properties.Any(p => p == "Id"))
            {
                updateQuery.Append("WHERE Id=@Id");
            }
            else
            {
                updateQuery.Append("WHERE id=@id");
            }


            return updateQuery.ToString();
        }
    }
}

