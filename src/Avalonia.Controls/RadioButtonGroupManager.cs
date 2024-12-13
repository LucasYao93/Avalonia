using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia.Collections.Pooled;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace Avalonia.Controls;

internal interface IRadioButton : ILogical
{
    string? GroupName { get; }
    MenuItemToggleType ToggleType { get; }
    bool IsChecked { get; set; }
}

internal class RadioButtonGroupManager
{
    //public static readonly RadioButtonGroupManager s_default = new();
    public static readonly ConditionalWeakTable<IRenderRoot, RadioButtonGroupManager> s_registeredVisualRoots = new();

    public readonly Dictionary<string, List<WeakReference<IRadioButton>>> _registeredGroups = new();
    private bool _ignoreCheckedChanges;

    public static RadioButtonGroupManager GetOrCreateForRoot(IRenderRoot root)
    {
        //if (root == null)
        //{
        //    Debug.WriteLine($"GetOrCreateForRoot:s_default-{s_default.GetHashCode()}", "RadioButton");
        //    Debug.WriteLine($"GetOrCreateForRoot:s_default._registeredGroups-{s_default._registeredGroups.GetHashCode()}", "RadioButton");
        //    return s_default;
        //}
        var rbgm = s_registeredVisualRoots.GetValue(root, key => new RadioButtonGroupManager());

        Debug.WriteLine($"GetOrCreateForRoot:rbgm-{rbgm.GetHashCode()}", "RadioButton");
        Debug.WriteLine($"GetOrCreateForRoot:rbgm._registeredGroups-{rbgm._registeredGroups.GetHashCode()}", "RadioButton");
        return rbgm;
    }

    public void Add(IRadioButton radioButton)
    {
        var groupName = radioButton.GroupName;
        if (groupName is not null && radioButton.ToggleType == MenuItemToggleType.Radio)
        {
            if (!_registeredGroups.TryGetValue(groupName, out var group))
            {
                group = new List<WeakReference<IRadioButton>>();
                _registeredGroups.Add(groupName, group);
            }

            //PrintInlcudeRB("Add", groupName);
            var instance = radioButton as RadioButton;
            //Debug.WriteLine($"Add, Group [{group.GetHashCode()}]: {instance?.Content}-{instance?.IsChecked}-{instance?.GetHashCode()}", "RadioButton");
            group.Add(new WeakReference<IRadioButton>(radioButton));
        }
    }

    private void PrintInlcudeRB(string category, string groupName)
    {
        if (!_registeredGroups.TryGetValue(groupName, out var group))
        {
            return;
        }

        foreach (var item in group)
        {
            if (item.TryGetTarget(out var rb))
            {
                var instance = rb as RadioButton;
                //Debug.WriteLine($"{category} Inlcudes, Group [{group.GetHashCode()}]: {instance?.Content}-{instance?.IsChecked}-{instance?.GetHashCode()}", "RadioButton");
            }
        }
    }

    public void Remove(IRadioButton radioButton, string? oldGroupName)
    {
        if (!string.IsNullOrEmpty(oldGroupName) && _registeredGroups.TryGetValue(oldGroupName, out var group))
        {
            int i = 0;
            while (i < group.Count)
            {
                if (!group[i].TryGetTarget(out var button) || button == radioButton)
                {
                    var instance = button as RadioButton;
                    DebugMessage($"RadioButtonGroupManager Group-{group.GetHashCode()} remove RB: {instance?.Content}-{instance?.IsChecked}-{instance?.GetHashCode()}");
                    group.RemoveAt(i);
                    continue;
                }

                i++;
            }

            if (group.Count == 0)
            {
                _registeredGroups.Remove(oldGroupName);
            }
            else
            {
                //Debug.WriteLine($"Not remove all rb of the {oldGroupName}:{group.GetHashCode()}", typeof(RadioButton).Name);

                foreach(var item in group)
                {
                    if(item.TryGetTarget(out var rb))
                    {
                        var instance = rb as RadioButton;
                        //Debug.WriteLine($"Keep, Group [{group.GetHashCode()}]: {instance?.Content}-{instance?.IsChecked}-{instance?.GetHashCode()}", "RadioButton");
                    }
                }
            }
        }
    }

    public void OnCheckedChanged(IRadioButton radioButton)
    {
        if (_ignoreCheckedChanges || radioButton.ToggleType != MenuItemToggleType.Radio)
        {
            return;
        }

        _ignoreCheckedChanges = true;
        try
        {
            var groupName = radioButton.GroupName;
            if (!string.IsNullOrEmpty(groupName))
            {
                if (_registeredGroups.TryGetValue(groupName, out var group))
                {
                    var i = 0;
                    while (i < group.Count)
                    {
                        if (!group[i].TryGetTarget(out var current))
                        {
                            group.RemoveAt(i);
                            continue;
                        }

                        if (current != radioButton && current.IsChecked)
                            current.IsChecked = false;
                        i++;
                    }

                    if (group.Count == 0)
                    {
                        _registeredGroups.Remove(groupName);
                    }

                    var parent = radioButton.LogicalParent as IRadioButton;
                    while (parent is not null && parent.GroupName == groupName)
                    {
                        parent.IsChecked = true;
                        parent = parent.LogicalParent as IRadioButton;
                    }

                    //PrintInlcudeRB("CheckedChanged",groupName);
                }
            }
            else
            {
                if (radioButton.LogicalParent is { } parent)
                {
                    foreach (var sibling in parent.LogicalChildren)
                    {
                        if (sibling != radioButton
                            && sibling is IRadioButton { ToggleType: MenuItemToggleType.Radio } button
                            && string.IsNullOrEmpty(button.GroupName)
                            && button.IsChecked)
                        {
                            button.IsChecked = false;
                        }
                    }
                }
            }
        }
        finally
        {
            _ignoreCheckedChanges = false;
        }
    }

    private void DebugMessage(string message)
    {
        var processId = Process.GetCurrentProcess().Id;
        Debug.WriteLine(message, $"RadioButton PID:{processId}");
    }
}
