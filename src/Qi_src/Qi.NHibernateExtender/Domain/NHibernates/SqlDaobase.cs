using System.Collections.Generic;
using System.Data;
using System.Text;
using Qi.NHibernateExtender;

namespace Qi.Domain.NHibernates
{
    /// <summary>
    /// </summary>
    public class SqlDaobase : AbstractDao
    {
        private IDbCommand _parameterCommand;

        /// <summary>
        /// </summary>
        /// <param name="sessionWrapper"></param>
        protected SqlDaobase(SessionWrapper sessionWrapper)
            : base(sessionWrapper)
        {
        }

        protected SqlDaobase(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }

        /// <summary>
        /// </summary>
        protected SqlDaobase()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool Insert(IDictionary<string, object> values, string table)
        {
            var sb = new StringBuilder("insert into ");
            sb.Append(table);
            sb.Append("(");
            sb.Append(string.Join(",", values.Keys)).Append(") values(");
            sb.Append("@").Append(string.Join(",@", values.Keys));

            var command = CreateCommand();

            foreach (var key in values.Keys)
            {
                var pa = CreateParameter("@" + key, values[key]);
                command.Parameters.Add(pa);
            }
            return command.ExecuteNonQuery() != 0;
        }
        
        /// <summary>
        /// </summary>
        protected IDbConnection CurrentConnection
        {
            get { return CurrentSession.Connection; }
        }

        private IDbCommand ParameterCommand
        {
            get { return _parameterCommand ?? (_parameterCommand = CurrentConnection.CreateCommand()); }
        }

        /// <summary>
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IDataParameter CreateParameter(string parameterName, object value)
        {
            IDbDataParameter result = ParameterCommand.CreateParameter();
            result.Value = value;
            result.ParameterName = parameterName;
            return result;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected IDbCommand CreateCommand()
        {
            return CurrentConnection.CreateCommand();
        }
    }
}