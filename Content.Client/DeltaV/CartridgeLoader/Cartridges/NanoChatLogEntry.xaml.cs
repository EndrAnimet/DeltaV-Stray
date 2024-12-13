using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.DeltaV.CartridgeLoader.Cartridges;

[GenerateTypedNameReferences]
public sealed partial class NanoChatLogEntry : BoxContainer
{
    public NanoChatLogEntry(int number, string time, string message)
    {
        RobustXamlLoader.Load(this);
        NumberLabel.Text = number.ToString();
        TimeLabel.Text = time;
        MessageLabel.Text = message;
    }
}
