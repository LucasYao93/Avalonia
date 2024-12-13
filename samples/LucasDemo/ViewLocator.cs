using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MiniMvvm;

namespace LucasDemo
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data is null)
                return null;

            var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);
            DebugMessage(name, $"ViewModel {name}: Swicth to it");
            if (type != null)
            {
                DebugMessage(name, $"View {name}: Create it");
                var control = (Control)Activator.CreateInstance(type)!;
                
                DebugMessage(name, $"View {name}: Set data context");
                control.DataContext = data;

                DebugMessage(name, $"View {name}: Attact to visual tree.");
                return control;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }

        private void DebugMessage(string type, string message)
        {
            if (type != "LucasDemo.Views.RBView")
            {
                return;
            }

            var splitLine = new string('-', 100);
            Debug.WriteLine(splitLine);

            Debug.WriteLine(message);

            Debug.WriteLine(splitLine);

        }
    }
}
