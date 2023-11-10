using System.Diagnostics;
using System.Reflection.Metadata;
using lib.Onrealm;
using lib.Onrealm.Manager;

namespace Slingshot.Onrealm;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked( object sender, EventArgs e )
    {

        _ = HandleRequest();
    }

    private async Task HandleRequest()
    {
        await MigrationDirector.Run( Token.Text, ClearData.IsChecked );
     

    }
}