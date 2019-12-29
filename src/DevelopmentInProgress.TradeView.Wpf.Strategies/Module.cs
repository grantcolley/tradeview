using DevelopmentInProgress.TradeView.Wpf.Host.Module;
using DevelopmentInProgress.TradeView.Wpf.Host.Navigation;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Strategies.View;
using DevelopmentInProgress.TradeView.Wpf.Strategies.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Logging;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Strategies";
        private static IUnityContainer StaticContainer;

        private static string StrategyUser = $"Strategies : {Environment.UserName}";

        public Module(IUnityContainer container, ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(container, moduleNavigator, logger)
        {
            StaticContainer = container;
        }

        public async override void Initialize()
        {
            Container.RegisterType<object, StrategyRunnerView>(typeof(StrategyRunnerView).Name);
            Container.RegisterType<StrategyRunnerViewModel>(typeof(StrategyRunnerViewModel).Name);

            var moduleSettings = new ModuleSettings();
            moduleSettings.ModuleName = ModuleName;
            moduleSettings.ModuleImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Strategies;component/Images/strategyManager.png";

            var moduleGroup = new ModuleGroup();
            moduleGroup.ModuleGroupName = StrategyUser;

            var strategyService = Container.Resolve<IStrategyService>();

            try
            {
                var userStrategies = await strategyService.GetStrategies();

                foreach (var strategy in userStrategies)
                {
                    var strategyDocument = CreateStrategyModuleGroupItem(strategy.Name, strategy.Name);
                    moduleGroup.ModuleGroupItems.Add(strategyDocument);
                }

                moduleSettings.ModuleGroups.Add(moduleGroup);
                ModuleNavigator.AddModuleNavigation(moduleSettings);

                Logger.Log("Initialize DevelopmentInProgress.Wpf.Strategies Complete", Category.Info, Priority.None);
            }
            catch (Exception ex)
            {
                Logger.Log($"Initialize DevelopmentInProgress.Wpf.Strategies failed to load: {ex.ToString()}", Category.Info, Priority.None);
            }
        }

        private static ModuleGroupItem CreateStrategyModuleGroupItem(string name, string title)
        {
            var strategyDocument = new ModuleGroupItem();
            strategyDocument.ModuleGroupItemName = name;
            strategyDocument.TargetView = typeof(StrategyRunnerView).Name;
            strategyDocument.TargetViewTitle = title;
            strategyDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.Strategies;component/Images/strategy.png";
            return strategyDocument;
        }
    }
}
