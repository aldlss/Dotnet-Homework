using System.Collections.ObjectModel;

namespace homework5.Models;

public class TreeNode
{
    public string Value { get; private set; }
    public ObservableCollection<TreeNode> Children { get; } = new();

    public TreeNode(string value)
    {
        Value = value;
    }

    public void AddChild(TreeNode node)
    {
        Children.Add(node);
    }
}
