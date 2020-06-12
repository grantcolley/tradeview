using DevelopmentInProgress.TradeView.Core.Strategy;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Configuration.View;
using Newtonsoft.Json;
using System;
using System.Linq;
using Microsoft.Win32;
using System.Reflection;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.Utility
{
    public class StrategyFileManager : IStrategyFileManager
    {
        public string GetStrategyTypeAsJson(StrategyFile strategyFile)
        {
            string file = string.Empty;

            var dialog = new OpenFileDialog
            {
                Title = "Select",
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true
            };

            var result = dialog.ShowDialog();
            if (result.HasValue
                && result.Value.Equals(true)
                && dialog.FileNames != null
                && dialog.FileNames.Length == 1)
            {
                file = dialog.FileNames[0];
            }

            if (!string.IsNullOrWhiteSpace(file))
            {
                Type type = typeof(StrategyParameters);
                var assembly = Assembly.LoadFile(file);
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
            }

            return string.Empty;
        }
    }
}