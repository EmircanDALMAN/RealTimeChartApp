using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartBackend.Subscription
{
    public interface IDatabaseSubscription
    {
        void Configure(string tableName);
    }
}
