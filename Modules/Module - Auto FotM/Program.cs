using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace Module_Auto_FotM
{
    class Program
    {
        static Items.Item Face_Of_The_Mountain = new Items.Item((int)ItemId.Face_of_the_Mountain, 700f);

        static Menu Config;
        static void InitMenu()
        {
            Config = new Menu("[Module] Auto FotM", "Module_AutoFotM", true);

            Config.AddItem(new MenuItem("Enabled", "Enabled?").SetValue(true));

            Config.AddItem(new MenuItem("Condition1", "- Condition 1 -").SetValue(true));
            Config.AddItem(new MenuItem("Condition1_HPLevel", "Ally %HP less than").SetValue(new Slider(30, 5, 50)));
            Config.AddItem(new MenuItem("Condition1_EnemyCheck", "If at least 1 enemy in range").SetValue(true));

            Config.AddItem(new MenuItem("Condition2", "- Condition 2 -").SetValue(true));
            Config.AddItem(new MenuItem("Condition2_EnemiesNear", "Count enemies near").SetValue(new Slider(3, 1, 5)));
            Config.AddItem(new MenuItem("Condition2_RangeNear", "Range of search").SetValue(new Slider(200, 200, 600)));
            Config.AddItem(new MenuItem("Condition2_HPLevel", "Ally %HP less than").SetValue(new Slider(75, 5, 90)));

            Config.AddToMainMenu();
        }

        static void CheckAndCast(Obj_AI_Hero Target)
        {
            bool Condition1 = Config.Item("Condition1").GetValue<bool>() &&
                              Config.Item("Condition1_HPLevel").GetValue<Slider>().Value >= Target.HealthPercentage() &&
                              (Target.Position.CountEnemiesInRange(800f) >= 1 && Config.Item("Condition1_EnemyCheck").GetValue<bool>() || !Config.Item("Condition1_EnemyCheck").GetValue<bool>());
            bool Condition2 = Config.Item("Condition2").GetValue<bool>() && 
                              Target.Position.CountEnemiesInRange(Config.Item("Condition2_RangeNear").GetValue<Slider>().Value) >= Config.Item("Condition2_EnemiesNear").GetValue<Slider>().Value &&
                              Config.Item("Condition2_HPLevel").GetValue<Slider>().Value >= Target.HealthPercentage();

            bool CanUse = Condition1 || Condition2;

            if (CanUse)
                Face_Of_The_Mountain.Cast(Target);
        }

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color='#00AA00'[Module] Auto Face Of The Mountain - ACTIVATED!</font>");
            InitMenu();
            Game.OnGameUpdate += Game_OnGameUpdate;
        }        
        static void Game_OnGameUpdate(EventArgs args)
        {
            if (!Face_Of_The_Mountain.IsReady() || ObjectManager.Player.IsDead)
                return;

            foreach (Obj_AI_Hero Ally in HeroManager.Allies)
            {
                if (Ally.IsValidTarget(Face_Of_The_Mountain.Range, false))
                    CheckAndCast(Ally);
            }
        }
        
    }
}
