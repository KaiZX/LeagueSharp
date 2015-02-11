using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace _Module__Potion_Manager
{
    class Program
    {
        static Menu _config;

        static Items.Item _healthpotion;
        static Items.Item _manapotion;
        static Items.Item _crystalineflask;
        static Items.Item _busquit;

        static void InitMenu()
        {
            _config = new Menu("[Module] Potion Manager", "PotionManager", true);

            _config.AddSubMenu(new Menu("Health Potion/Bisquit", "HP"));
            _config.SubMenu("HP").AddItem(new MenuItem("hp_use", "Use those?").SetValue(true));
            _config.SubMenu("HP").AddItem(new MenuItem("hp_start", "How low HP% you must have to use?").SetValue(new Slider(70)));

            _config.AddSubMenu(new Menu("Crystalline Flask", "FP"));
            _config.SubMenu("FP").AddItem(new MenuItem("fp_use", "Use those?").SetValue(true));
            _config.SubMenu("FP").AddItem(new MenuItem("fp_start", "How low HP% you must have to use?").SetValue(new Slider(80)));

            _config.AddSubMenu(new Menu("Mana Potion", "MP"));
            _config.SubMenu("MP").AddItem(new MenuItem("mp_use", "Use those?").SetValue(true));
            _config.SubMenu("MP").AddItem(new MenuItem("mp_start", "How low MP% you must have to use?").SetValue(new Slider(40)));

            _config.AddToMainMenu();
        }
        static void InitItems()
        {
            _healthpotion = new Items.Item((int)ItemId.Health_Potion);
            _manapotion = new Items.Item((int)ItemId.Mana_Potion);
            _crystalineflask = new Items.Item((int)ItemId.Crystalline_Flask);
            _busquit = new Items.Item(2010);
        }

        static void doHP()
        {
            if (_config.Item("hp_use").GetValue<bool>())
            {
                int _hp = _config.Item("hp_start").GetValue<Slider>().Value;
                if (ObjectManager.Player.HealthPercentage() <= _hp && !ObjectManager.Player.Buffs.Any(x => x.Name == "RegenerationPotion" || x.Name == "ItemMiniRegenPotion" || x.Name == "ItemCrystalFlask"))
                    if (_healthpotion.IsReady())
                        _healthpotion.Cast();
                    else if (_busquit.IsReady())
                        _busquit.Cast();
            }
        }

        static void doFP()
        {
            if (_config.Item("fp_use").GetValue<bool>())
            {
                int _hp = _config.Item("fp_start").GetValue<Slider>().Value;
                if (ObjectManager.Player.HealthPercentage() <= _hp && !ObjectManager.Player.Buffs.Any(x => x.Name == "RegenerationPotion" || x.Name == "ItemMiniRegenPotion" || x.Name == "ItemCrystalFlask"))
                    if (_crystalineflask.IsReady())
                        _crystalineflask.Cast();
            }
        }

        static void doMP()
        {
            if (_config.Item("mp_use").GetValue<bool>())
            {
                int _mp = _config.Item("mp_start").GetValue<Slider>().Value;
                if (ObjectManager.Player.ManaPercentage() <= _mp && !ObjectManager.Player.Buffs.Any(x => x.Name == "FlaskOfCrystalWater"))
                    if (_manapotion.IsReady())
                        _manapotion.Cast();
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
            InitItems();

            Game.OnGameUpdate += OnTick;
        }

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
