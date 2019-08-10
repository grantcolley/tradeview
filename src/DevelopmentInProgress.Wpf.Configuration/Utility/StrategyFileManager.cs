using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Configuration.View;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using Interface = DevelopmentInProgress.MarketView.Interface.Strategy;

namespace DevelopmentInProgress.Wpf.Configuration.Utility
{
    public class StrategyFileManager : IStrategyFileManager
    {  
        public string GetStrategyTypeAsJson(StrategyFile strategyFile)
        {
            Type type = typeof(Interface.StrategyParameters);
            var assembly = Assembly.LoadFile(strategyFile.File);
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type));
            if (!types.Any())
            {
                throw new Exception($"No types implementing {type.Name} available in {assembly.GetName().Name}");
            }

            var parameterDialogView = new ParameterDialogView(types);
            parameterDialogView.ShowDialog();

            if (parameterDialogView.SelectedType != null)
            {
                var selectedType = parameterDialogView.SelectedType;

                var strategyParameters = Activator.CreateInstance(selectedType);
                return JsonConvert.SerializeObject(strategyParameters, Formatting.Indented);
            }

            return string.Empty;
        }
    }
}