// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Models
{
    public enum Shop
    {
        Unknown,
        Romsey,
        ChandlersFord,
        Highcliffe
    }

    public sealed record ShopDetails(Shop Shop, string DisplayName, string LogName, string Hours, string Phone, string Url)
    {
        private static readonly ShopDetails[] All =
        {
            new(Shop.Romsey, "Romsey", "Romsey", "Mon-Fri 9:15am-5:00pm - Sat 9:15am-2:00pm", "01794 517142", "https://www.thecomputerrepaircentre.co.uk/romsey"),
            new(Shop.ChandlersFord, "Chandler's Ford", "Chandlers Ford", "Mon-Fri 9:00am-5:00pm - Sat 9:00am-3:00pm", "02380 270271", "https://www.thecomputerrepaircentre.co.uk/chandlers-ford"),
            new(Shop.Highcliffe, "Highcliffe", "Highcliffe", "Mon-Fri 9:15am-5:00pm - Sat 9:15am-2:00pm", "01425 278579", "https://www.thecomputerrepaircentre.co.uk/highcliffe")
        };

        public static ShopDetails? For(Shop shop) => System.Array.Find(All, s => s.Shop == shop);

        public static ShopDetails? ForDisplayName(string? name) => System.Array.Find(All, s => s.DisplayName == name);
    }
}
