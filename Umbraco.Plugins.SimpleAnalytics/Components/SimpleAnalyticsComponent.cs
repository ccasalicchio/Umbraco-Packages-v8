using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Plugins.SimpleAnalytics.Dashboards;
using Umbraco.Plugins.SimpleAnalytics.Models;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;
using System.Windows.Forms;
using Castle.Facilities.Logging;
using Umbraco.Core.Scoping;
using Umbraco.Core.Migrations;
using Umbraco.Core.Services;

namespace Umbraco.Plugins.SimpleAnalytics.Components
{
    public class SimpleAnalyticsComposer : ComponentComposer<SimpleAnalyticsComponent>
    {
    }
    public class SimpleAnalyticsComponent : IComponent 
    {
        private IScopeProvider _scopeProvider;
        private IMigrationBuilder _migrationBuilder;
        private IKeyValueService _keyValueService;
        private ILogger _logger;

        public SimpleAnalyticsComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
        }

        public void Initialize()
        {
            var migrationPlan = new MigrationPlan("SimpleAnalytics");

            migrationPlan.From(string.Empty)
                .To<AddCommentsTable>("simpleanalytics-db");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }
        public void Terminate()
        {
        }
    }


    //**************************************************************************************************************************
    public class SimpleAnalyticsComponent : IUserComposer
    {
        private readonly IScopeProvider scopeProvider;
        private void ApplicationStarted(Composition composition)
        {
            var logger = LoggerResolver.Current.Logger;
            using (var scope = scopeProvider.CreateScope())
            {
                var dbContext = scope.Database;
                
                // do stuff
                scope.Complete();
            }
            //var dbContext = ApplicationContext.Current.DatabaseContext;
            

            //Analytics
            if (!db.TableExist(AnalyticsVisit.TABLENAME))
            {
                db.CreateTable<AnalyticsVisit>(false);
            }

            var simpleAnalyticsDahboard = new AnalyticsDashboard();
            simpleAnalyticsDahboard.InstallDashboard();
        }
    }
}
