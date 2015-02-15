using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

namespace Module_Debug
{
    class Program
    {
        // Menu Variable
        static Menu Config;
        // Menu Init Function
        static void InitMenu()
        {
            Config = new Menu("[Module] Debug Module", "Module_Debug", true);

            Config.AddItem(new MenuItem("Label1", "- Core Functions -"));
            Config.AddItem(new MenuItem("Enabled", "Is Enabled?").SetValue(true));

            Config.AddItem(new MenuItem("Label2", "- Debug Functions -"));
            Config.AddItem(new MenuItem("WriteBuffs", "Write all buffs on player!").SetValue(true));

            Config.AddToMainMenu();
        }
        
        static void OnTick(EventArgs args)
        {
            if (Config.Item("WriteBuffs").GetValue<bool>())
            {
                foreach (BuffInstance Baff in ObjectManager.Player.Buffs)
                {
                    Console.WriteLine("Time({0}) Buff Info: Name = {1}, Count = {2}", Environment.TickCount, Baff.Name, Baff.Count);
                }
            }
        }
        static void OnLoad(EventArgs args)
        {
            InitMenu();

            Game.OnGameUpdate += OnTick;
        }
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
