using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;

namespace Module_Auto_Level_Skills
{
    class AutoLevelPlus
    {
        static int[] _order = new int[18];
        static int _lastLeveled;
        static bool isEnabled = false;

        static Random RandomNumber;

        public static void Initialize(int[] startOrd)
        {
            _order = startOrd;
            _lastLeveled = 0;
            Enabled(false);
            RandomNumber = new Random(Environment.TickCount);
        }

        public static void Enabled(bool enabled)
        {
            if (_order == null)
            {
                Game.PrintChat("Initalize AutoLevel before using it.");
                return;
            }

            if (enabled && !isEnabled)
            {
                isEnabled = true;
                Game.OnGameUpdate += Game_OnGameUpdate;
            }
            if (!enabled && isEnabled)
            {
                isEnabled = false;
                Game.OnGameUpdate -= Game_OnGameUpdate;
            }
        }

        static bool HasLevelOneSpell()
        {
            var name = ObjectManager.Player.ChampionName;
            var list = new List<string> { "Elise", "Jayce", "Karma", "Nidalee" };
            return list.Contains(name);
        }

        static int _getRealLevel()
        {
            if (!HasLevelOneSpell())
                return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level;
            else
                return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level - 1;
        }

        static void _levelUp()
        {
            int _lvl = _getRealLevel();
            for (int i = _lvl; i < ObjectManager.Player.Level; i++)
            {
                ObjectManager.Player.Spellbook.LevelSpell((SpellSlot)_order[i] - 1);
            }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.Level > _lastLeveled)
            {
                _lastLeveled = ObjectManager.Player.Level;
                _levelUp();
            }
        }
    }
}
