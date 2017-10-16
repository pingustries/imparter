﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imparter.Sql
{
    public class SqlChannelCreator
    {
        private readonly SqlExecutor _sqlExecutor;

        public SqlChannelCreator(string connectionString)
        {
            _sqlExecutor = new SqlExecutor(connectionString);
        }

        public async Task CreateIfNotExists(string channelName)
        {
            string sql = $@"
IF OBJECT_ID('{channelName}', 'U') IS NULL
BEGIN
CREATE TABLE {channelName}(
    Id INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    MessageType NVARCHAR(512) NOT NULL,
    Tries INT NOT NULL DEFAULT 1,
    Timeout DateTime NULL,
    Data NVARCHAR(MAX));
END
";
            await _sqlExecutor.ExecuteNonQuery(sql);
        }

        public Task DoAQuery()
        {
            var sql = @"IF OBJECT_ID('@channelName', 'U') IS NULL";
            var param = new SqlParameter("channelName", SqlDbType.NVarChar);
            param.Value = "ello";
            return _sqlExecutor.ExecuteNonQuery(sql, param);
        }
    }
}
