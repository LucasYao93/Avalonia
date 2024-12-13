using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace Avalonia.Controls
{
    /// <summary>
    /// Represents a button that allows a user to select a single option from a group of options.
    /// </summary>
    public class RadioButton : ToggleButton, IRadioButton
    {
        /// <summary>
        /// Identifies the GroupName dependency property.
        /// </summary>
        public static readonly StyledProperty<string?> GroupNameProperty =
            AvaloniaProperty.Register<RadioButton, string?>(nameof(GroupName));

        private RadioButtonGroupManager? _groupManager;

        /// <summary>
        /// Gets or sets the name that specifies which RadioButton controls are mutually exclusive.
        /// </summary>
        public string? GroupName
        {
            get => GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        bool IRadioButton.IsChecked
        {
            get => IsChecked.GetValueOrDefault();
            set => SetCurrentValue(IsCheckedProperty, value);
        }

        MenuItemToggleType IRadioButton.ToggleType => MenuItemToggleType.Radio;

        protected override void Toggle()
        {
            if (!IsChecked.GetValueOrDefault())
            {
                SetCurrentValue(IsCheckedProperty, true);
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            _groupManager?.Remove(this, GroupName);
            
            DebugRadioButtonGroupManager($"{this.Content}-{this.GetHashCode()} OnAttachedToVisualTree removed", _groupManager);

            EnsureRadioGroupManager(e.Root);

            DebugRadioButtonGroupManager($"{this.Content}-{this.GetHashCode()} OnAttachedToVisualTree added", _groupManager);

            base.OnAttachedToVisualTree(e);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);            
            _groupManager?.Remove(this, GroupName);
            DebugRadioButtonGroupManager($"{this.Content}-{this.GetHashCode()} OnDetachedFromVisualTree removed", _groupManager);
            _groupManager = null;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadioButtonAutomationPeer(this);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsCheckedProperty)
            {
                IsCheckedChanged(change.GetNewValue<bool?>());
            }
            else if (change.Property == GroupNameProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<string?>();
                OnGroupNameChanged(oldValue, newValue);
            }
        }

        private void OnGroupNameChanged(string? oldGroupName, string? newGroupName)
        {
            if (!string.IsNullOrEmpty(oldGroupName))
            {
                _groupManager?.Remove(this, oldGroupName);
            }
            if (!string.IsNullOrEmpty(newGroupName))
            {
                EnsureRadioGroupManager();
            }

            DebugRadioButtonGroupManager($"{this.Content}-{this.GetHashCode()} OnGroupNameChanged add", _groupManager);

        }

        private new void IsCheckedChanged(bool? value)
        {
            if (value.GetValueOrDefault())
            {
                EnsureRadioGroupManager();
                
                DebugRadioButtonGroupManager($"{this.Content}-{this.GetHashCode()} OnCheckedChanged before", _groupManager);
                
                _groupManager?.OnCheckedChanged(this);

                DebugRadioButtonGroupManager($"{this.Content}-{this.GetHashCode()} OnCheckedChanged after", _groupManager);
            }
        }

        private void EnsureRadioGroupManager(IRenderRoot? root = null)
        {
            if (root == null)
            {
                root = this.GetVisualRoot();
            }

            if (root == null)
            {
                return;
            }

            _groupManager = RadioButtonGroupManager.GetOrCreateForRoot(root);
            _groupManager.Add(this);
        }

        private void DebugRadioButtonGroupManager(string type, RadioButtonGroupManager? groupManager)
        {
            if (groupManager is null)
            {
                return;
            }

            var groupManagerName = "s_registeredVisualRoots";

            //if (groupManager.GetHashCode() == RadioButtonGroupManager.s_default.GetHashCode())
            //{
            //    groupManagerName = "s_default";
            //}

            DebugMessage(new string('-', 100));

            DebugMessage($"{type} {groupManagerName} RadioButtonGroupManager: {groupManager.GetHashCode()}");
            DebugMessage($"{type} {groupManagerName} RadioButtonGroupManager _registeredGroups: {groupManager._registeredGroups.GetHashCode()}");

            if (string.IsNullOrEmpty(GroupName))
            {
                DebugMessage($"{type} {groupManagerName} RadioButtonGroupManager: group name empty");
                goto end;
            }
            
            groupManager._registeredGroups.TryGetValue(GroupName, out var group);

            if (group is null)
            {
                DebugMessage($"{type} {groupManagerName} RadioButtonGroupManager: group empty");
                goto end;
            }

            foreach(var ite in group)
            {
                if (ite.TryGetTarget(out var rb))
                {
                    var instance = rb as RadioButton;
                    DebugMessage($"{type} {groupManagerName} RadioButtonGroupManager: {instance?.Content}-{instance?.IsChecked}-{instance?.GetHashCode()}");
                }
            }
            end:
            DebugMessage(new string('-', 100));
        }

        private void DebugMessage(string message)
        {
            var processId = Process.GetCurrentProcess().Id;
            Debug.WriteLine(message, $"RadioButton PID:{processId}");
        }
    }
}
