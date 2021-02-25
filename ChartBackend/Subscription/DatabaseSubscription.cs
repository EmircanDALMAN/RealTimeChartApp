using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartBackend.Hubs;
using ChartBackend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using TableDependency.SqlClient;

namespace ChartBackend.Subscription
{

    public class DatabaseSubscription<T> : IDatabaseSubscription where T : class, new()
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChartHub> _hubContext;

        public DatabaseSubscription(IConfiguration configuration, IHubContext<ChartHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;
        }
        private SqlTableDependency<T> _sqlTableDependency;

        public void Configure(string tableName)
        {
            _sqlTableDependency = new SqlTableDependency<T>(_configuration.GetConnectionString
                ("SQL"), tableName); _sqlTableDependency.OnChanged += async (o, e) =>
            {
                var context = new DbChartContext();
                var data = (from employee in context.Employees
                            join sale in context.Sales on employee.Id equals sale.PersonelId
                            select new
                            {
                                employee,
                                sale
                            }).ToList();
                List<object> datas = new List<object>();
                var employeeNames = data.Select(d => d.employee.Name).Distinct().ToList();
                employeeNames.ForEach(e =>
                {
                    datas.Add(new
                    {
                        name = e,
                        data = data.Where(s => s.employee.Name == e).Select(s => s.sale.Price).ToList()
                    });
                });
                await _hubContext.Clients.All.SendAsync("receiveMessage", datas);
            };
            _sqlTableDependency.OnError += (o, e) =>
                {

                };
            _sqlTableDependency.Start();
        }

        ~DatabaseSubscription()
        {
            _sqlTableDependency.Stop();
        }
    }
}
