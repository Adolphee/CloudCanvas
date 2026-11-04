using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudCanvas.Application.Common.Constants;
using Microsoft.Extensions.Configuration;

namespace CloudCanvas.Infrastructure.Common
{
    public class ConfigTool
    {
        public static Dictionary<string, string> GetConnectionStrings(IConfiguration config)
        {
            var sql = config.GetConnectionString(SQLServer.ConnectionString) ?? throw new ArgumentNullException(nameof(SQLServer.ConnectionString));
            var projectionUri = config.GetConnectionString(Application.Common.Constants.Projection.Uri) ?? throw new ArgumentNullException(nameof(Application.Common.Constants.Projection.Uri));
            var bsConnection = config.GetConnectionString(BStorage.BSConnection) ?? throw new ArgumentNullException(nameof(BStorage.BSConnection));
            var sendpoint = config.GetConnectionString(Secrets.FUMSGO) ?? throw new ArgumentNullException(nameof(Secrets.FUMSGO));
            
            var conn = new Dictionary<string, string>
            {
                { SQLServer.ConnectionString, sql },
                { Application.Common.Constants.Projection.Uri, projectionUri },
                { BStorage.BSConnection, bsConnection },
                { Secrets.FUMSGO, sendpoint }
            };
            return conn;
        } 
    }
}