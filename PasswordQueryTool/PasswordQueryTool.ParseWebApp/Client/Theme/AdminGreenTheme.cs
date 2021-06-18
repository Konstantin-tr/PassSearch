using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.ParseWebApp.Client.Theme
{
    public class AdminGreenTheme : MudTheme
    {
        public AdminGreenTheme()
        {
            ModifyPalette();
        }

        private void ModifyPalette()
        {
            Palette.Primary = "#992359";
            Palette.PrimaryContrastText = "#FFFFFF";

            Palette.Secondary = "#00B548";
            Palette.SecondaryContrastText = "#FFFFFF";

            Palette.Tertiary = "#40C876";
            Palette.TertiaryContrastText = "#FFFFFF";

            Palette.GrayLight = "#FFFFFF";

            Palette.Background = "#f4fbf6";
            Palette.TextPrimary = "#FFFFFF";
            Palette.TextSecondary = "#F4FCF7";

            Palette.Dark = "#2d323d";

            Palette.ActionDefault = "#F4FCF7";

            Palette.AppbarBackground = "#00B548";
            Palette.AppbarText = "#FFFFFF";
        }
    }
}
