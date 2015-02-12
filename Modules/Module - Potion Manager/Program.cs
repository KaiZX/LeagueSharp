using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace Module_Potion_Manager
{
    class Program
    {
        static Items.Item Item_HealthPotion = new Items.Item((int)ItemId.Health_Potion);
        static Items.Item Item_ManaPotion = new Items.Item((int)ItemId.Mana_Potion);
        static Items.Item Item_CrystalineFlask = new Items.Item((int)ItemId.Crystalline_Flask);
        static Items.Item Item_Busquit = new Items.Item(2010);

        // Menu Variable
        static Menu Config;
        // Menu Init Function
        static void InitMenu()
        {
            Config = new Menu("[Module] Potion Manager", "Module_PotionManager", true);

            Config.AddSubMenu(new Menu("Health Potion/Bisquit", "HP"));
            Config.SubMenu("HP").AddItem(new MenuItem("hp_use", "Use those?").SetValue(true));
            Config.SubMenu("HP").AddItem(new MenuItem("hp_start", "How low HP% you must have to use?").SetValue(new Slider(70)));

            Config.AddSubMenu(new Menu("Crystalline Flask", "FP"));
            Config.SubMenu("FP").AddItem(new MenuItem("fp_use", "Use those?").SetValue(true));
            Config.SubMenu("FP").AddItem(new MenuItem("fp_start", "How low HP% you must have to use?").SetValue(new Slider(80)));

            Config.AddSubMenu(new Menu("Mana Potion", "MP"));
            Config.SubMenu("MP").AddItem(new MenuItem("mp_use", "Use those?").SetValue(true));
            Config.SubMenu("MP").AddItem(new MenuItem("mp_start", "How low MP% you must have to use?").SetValue(new Slider(40)));

            Config.AddToMainMenu();
        }

        static void doHP()
        {
            if (Config.Item("hp_use").GetValue<bool>())
            {
                int _hp = Config.Item("hp_start").GetValue<Slider>().Value;
                if (ObjectManager.Player.HealthPercentage() <= _hp && !ObjectManager.Player.Buffs.Any(x => x.Name == "RegenerationPotion" || x.Name == "ItemMiniRegenPotion" || x.Name == "ItemCrystalFlask"))
                {
                    var ToUse = Item_Busquit.IsOwned() ? Item_Busquit : Item_HealthPotion;
                    if (ToUse.IsReady())
                        ToUse.Cast();
                }
            }
        }
        static void doFP()
        {
            if (Config.Item("fp_use").GetValue<bool>())
            {
                int _hp = Config.Item("fp_start").GetValue<Slider>().Value;
                if (ObjectManager.Player.HealthPercentage() <= _hp && !ObjectManager.Player.Buffs.Any(x => x.Name == "RegenerationPotion" || x.Name == "ItemMiniRegenPotion" || x.Name == "ItemCrystalFlask"))
                    if (Item_CrystalineFlask.IsReady())
                        Item_CrystalineFlask.Cast();
            }
        }
        static void doMP()
        {
            if (Config.Item("mp_use").GetValue<bool>())
            {
                int _mp = Config.Item("mp_start").GetValue<Slider>().Value;
                if (ObjectManager.Player.ManaPercentage() <= _mp && !ObjectManager.Player.Buffs.Any(x => x.Name == "FlaskOfCrystalWater"))
                    if (Item_ManaPotion.IsReady())
                        Item_ManaPotion.Cast();
            }
        }

        static void OnTick(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.InFountain())
                return;

            doFP();
            doHP();
            doMP();
        }
        static void OnLoad(EventArgs args)
        {
            InitMenu();

            Game.PrintChat("[Module] Auto Potion Manager - ACTIVATED!");

            Game.OnGameUpdate += OnTick;
        }
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
