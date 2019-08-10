using DevelopmentInProgress.Wpf.Host.Module;
using DevelopmentInProgress.Wpf.Host.Navigation;
using DevelopmentInProgress.Wpf.Configuration.View;
using DevelopmentInProgress.Wpf.Configuration.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Logging;
using System;

namespace DevelopmentInProgress.Wpf.Configuration
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Configuration";
        private static string StrategyUser = $"Configuration : {Environment.UserName}";

        public Module(IUnityContainer container, ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(container, moduleNavigator, logger)
        {
        }

        public override void Initialize()
        {
            Container.RegisterType<object, StrategyManagerView>(typeof(StrategyManagerView).Name);
            Container.RegisterType<StrategyManagerViewModel>(typeof(StrategyManagerViewModel).Name);

            var moduleSettings = new ModuleSettings();
            moduleSettings.ModuleName = ModuleName;
            moduleSettings.ModuleImagePath = @"/DevelopmentInProgress.Wpf.Configuration;component/Images/configuration.png";

            var moduleGroup = new ModuleGroup();
            moduleGroup.ModuleGroupName = StrategyUser;

            var newDocument = new ModuleGroupItem();
            newDocument.ModuleGroupItemName = "Manage Strategies";
            newDocument.TargetView = typeof(StrategyManagerView).Name;
            newDocument.TargetViewTitle = "Manage Strategies";
            newDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.Configuration;component/Images/manageStrategies.png";
            moduleGroup.ModuleGroupItems.Add(newDocument);

            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log("Initialize DevelopmentInProgress.Wpf.Configuration Complete", Category.Info, Priority.None);
        }

        public static void AddStrategy(string strategyName)
        {
        }

        public static void RemoveStrategy(string strategyName)
        {
        }
    }
}
