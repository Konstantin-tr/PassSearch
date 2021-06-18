using MudBlazor;
using System;

namespace PasswordQueryTool.WebApp.Client.Themes
{
    public class BlueTheme : MudTheme
    {
        public BlueTheme()
        {
            ModifyPalette();
        }

        private void ModifyPalette()
        {
            Palette.Primary = "#033592";
            Palette.PrimaryContrastText = "#FFFFFF";

            Palette.Secondary = "#E4F6FF";
            Palette.SecondaryContrastText = "#3C3C3C";

            Palette.Tertiary = "#1484FF";
            Palette.TertiaryContrastText = "#FFFFFF";

            Palette.Surface = "#1484FF";

            Palette.Background = "#E4F6FF";
            Palette.TextPrimary = "#FFFFFF";
            Palette.TextSecondary = "#dee8ff";

            Palette.Dark = "#2d323d";

            Palette.DrawerBackground = "#369afa";
            Palette.DrawerText = "#FFFFFF";
            Palette.DrawerIcon = "#B3D4FF";

            Palette.ActionDefault = "#B3D4FF";

            Palette.AppbarBackground = "#1484FF";
            Palette.AppbarText = "#FFFFFF";
        }
    }
}