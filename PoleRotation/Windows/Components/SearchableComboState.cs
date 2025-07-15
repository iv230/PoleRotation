using System.Collections.Generic;

namespace PoleRotation.Windows.Components;

public sealed class SearchableComboState<T>
{
    public List<T> Items = [];
    public int SelectedIndex = -1;
    public string SearchText = "";
}
