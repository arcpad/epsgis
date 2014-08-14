using IBatisNet.DataMapper;
using System.Collections.Generic;
using System;

namespace EPS.Utils
{
    public static class DbUtils
    {
        public static int Insert<T>(String statementName, T t)
        {
            ISqlMapper iSqlMapper = Mapper.Instance();
            if (iSqlMapper != null)
            {
                return (int)iSqlMapper.Insert(statementName, t);
            }
            return 0;
        }

        public static int Update<T>(String statementName, T t)
        {
            ISqlMapper iSqlMapper = Mapper.Instance();
            if (iSqlMapper != null)
            {
                return iSqlMapper.Update(statementName, t);
            }
            return 0;
        }

        public static int Delete(String statementName, int primaryKeyId)
        {
            ISqlMapper iSqlMapper = Mapper.Instance();
            if (iSqlMapper != null)
            {
                return iSqlMapper.Delete(statementName, primaryKeyId);
            }
            return 0;
        }

        public static T Get<T>(String statementName, int primaryKeyId) where T : class
        {
            ISqlMapper iSqlMapper = Mapper.Instance();
            if (iSqlMapper != null)
            {
                return iSqlMapper.QueryForObject<T>(statementName, primaryKeyId);
            }
            return null;
        }

        public static IList<T> List<T>(String statementName, object parameterObject = null)
        {
            ISqlMapper iSqlMapper = Mapper.Instance();
            if (iSqlMapper != null)
            {
                return iSqlMapper.QueryForList<T>(statementName, parameterObject);
            }
            return null;
        }
    }
}