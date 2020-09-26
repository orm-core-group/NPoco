﻿using Microsoft.Data.SqlClient;
using NPoco.SqlServer;
using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace NPoco.DatabaseTypes
{
    public class SqlServerDatabase : Database
    {
        private readonly IPollyPolicy _pollyPolicy;

        public SqlServerDatabase(string connectionString, IPollyPolicy pollyPolicy = null) 
            : this(connectionString, new SqlServer2012DatabaseType(), pollyPolicy)
        {
        }

        public SqlServerDatabase(string connectionString, SqlServerDatabaseType databaseType, IPollyPolicy pollyPolicy) 
            : base(connectionString, databaseType, SqlClientFactory.Instance)
        {
            _pollyPolicy = pollyPolicy;
        }

        protected override T ExecutionHook<T>(Func<T> action)
        {
            if (_pollyPolicy?.RetryPolicy != null)
            {
                return _pollyPolicy.RetryPolicy.Execute(action);
            }

            return base.ExecutionHook(action);
        }

        protected override async ValueTask<T> ExecutionHookAsync<T>(Func<Task<T>> action)
        {
            if (_pollyPolicy?.AsyncRetryPolicy != null)
            {
                return await _pollyPolicy.AsyncRetryPolicy.ExecuteAsync(action);
            }

            return await base.ExecutionHookAsync(action);
        }
    }
}
