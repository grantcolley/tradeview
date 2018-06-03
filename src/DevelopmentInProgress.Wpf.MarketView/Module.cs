using DevelopmentInProgress.Wpf.MarketView.View;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using DevelopmentInProgress.Wpf.Host.Module;
using DevelopmentInProgress.Wpf.Host.Navigation;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Unity;
using System;
using DevelopmentInProgress.Wpf.MarketView.Services;

namespace DevelopmentInProgress.Wpf.MarketView
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Market View";

        public Module(IUnityContainer container, ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(container, moduleNavigator, logger)
        {
        }

        public override void Initialize()
        {
            Container.RegisterType<object, UserAccountsView>(typeof(UserAccountsView).Name);
            Container.RegisterType<UserAccountsViewModel>(typeof(UserAccountsViewModel).Name);
            Container.RegisterType<object, TradingView>(typeof(TradingView).Name);
            Container.RegisterType<TradingViewModel>(typeof(TradingViewModel).Name);

            var moduleSettings = new ModuleSettings();
            moduleSettings.ModuleName = ModuleName;
            moduleSettings.ModuleImagePath = @"/DevelopmentInProgress.Wpf.MarketView;component/Images/diptrade.png";

            var moduleGroup = new ModuleGroup();
            moduleGroup.ModuleGroupName = $"Accounts : {Environment.UserName}";

            var accountsService = Container.Resolve<IAccountsService>();

            var userAccounts = accountsService.GetAccounts();

            var manageAccountsDocument = new ModuleGroupItem();
            manageAccountsDocument.ModuleGroupItemName = "Manage Accounts";
            manageAccountsDocument.TargetView = typeof(UserAccountsView).Name;
            manageAccountsDocument.TargetViewTitle = "Manage Accounts";
            manageAccountsDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.MarketView;component/Images/trade.png";
            moduleGroup.ModuleGroupItems.Add(manageAccountsDocument);

            foreach (var userAccount in userAccounts.Accounts)
            {
                var tradingDocument = new ModuleGroupItem();
                tradingDocument.ModuleGroupItemName = userAccount.AccountName;
                tradingDocument.TargetView = typeof(TradingView).Name;
                tradingDocument.TargetViewTitle = userAccount.AccountName;
                tradingDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.MarketView;component/Images/trade.png";
                moduleGroup.ModuleGroupItems.Add(tradingDocument);
            }

            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log("Initialize DevelopmentInProgress.Wpf.MarketView Complete", Category.Info, Priority.None);
        }
    }
}
