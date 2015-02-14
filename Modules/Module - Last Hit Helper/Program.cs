using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

namespace Module_Last_Hit_Helper
{
    class Program
    {
        // Player Variable
        static Obj_AI_Hero Player = ObjectManager.Player;

        // Menu Variable
        static Menu Config;
        // Menu Initialization Function
        static void InitMenu()
        {
            Config = new Menu("[Module] Last Hit Helper", "Module_Last_Hit_Helper", true);

            Config.AddItem(new MenuItem("Label1", "- Main Function - "));
            Config.AddItem(new MenuItem("AA_LastHit", "Draw LastHits from AA's").SetValue(true));
            Config.AddItem(new MenuItem("AA_PreLastHit", "Draw Pre-LastHits from AA's").SetValue(true));

            Config.AddItem(new MenuItem("Label2", "- Item Specific Functions - "));
            Config.AddItem(new MenuItem("Relic_LastHit", "Draw LastHits from Relic Shield").SetValue(true));
            Config.AddItem(new MenuItem("Relic_PreLastHit", "Draw Pre-LastHits from Relic Shield").SetValue(true));

            Config.AddItem(new MenuItem("Label3", "- Hero Specific Functions - "));
            Config.AddItem(new MenuItem("AnnieQ_LastHit", "Draw LastHits from Annie Q").SetValue(true));
            // Add Irelia Q someday
            // Miss Fortune Q too?
            // 

            //Config.AddItem(new MenuItem("Label1", "- Core Functions - "));

            Config.AddToMainMenu();
        }

        // Items Variables
        static Items.Item RelicShield, TargonsBrace, FaceOfTheMountain;
        // Items Initialization Function
        static void InitItems()
        {
            RelicShield = new Items.Item((int)ItemId.Relic_Shield);
            TargonsBrace = new Items.Item((int)ItemId.Targons_Brace);
            FaceOfTheMountain = new Items.Item((int)ItemId.Face_of_the_Mountain);
        }

        // Skills Variables
        static Spell Q, W, E, R;
        // Spell Init Variables
        static void InitSpells()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
        }

        // Draw Functions
        static void Draw_AA()
        {
            foreach (Obj_AI_Base Victim in MinionManager.GetMinions(2000f))
            {
                if (Player.GetAutoAttackDamage(Victim) > Victim.Health)
                    Render.Circle.DrawCircle(Victim.Position, 90f, Color.Yellow);
            }
        }
        static void Draw_NearDeath()
        {
            foreach (Obj_AI_Base Victim in MinionManager.GetMinions(2000f))
            {
                if (Player.GetAutoAttackDamage(Victim)*2.5 > Victim.Health)
                    Render.Circle.DrawCircle(Victim.Position, 85f, Color.YellowGreen);
            }
        }
        static void Draw_RelicShield()
        {
            if (!RelicShield.IsOwned())
                return;

            float DMG = Player.TotalAttackDamage() + 200;
            foreach (Obj_AI_Base Victim in MinionManager.GetMinions(2000f))
            {
                if (Victim.CountAlliesInRange(1100f) > 1)
                {
                    if (DMG > Victim.Health)
                        Render.Circle.DrawCircle(Victim.Position, 100f, Color.Cyan);
                    if (Config.Item("Relic_PreLastHit").GetValue<bool>() && DMG + 100 > Victim.Health)
                        Render.Circle.DrawCircle(Victim.Position, 95f, Color.DarkCyan);
                }
            }
        }
        static void Draw_TargonsBrace()
        {
            if (!TargonsBrace.IsOwned())
                return;

            float DMG = Player.TotalAttackDamage() + 240;
            foreach (Obj_AI_Base Victim in MinionManager.GetMinions(2000f))
            {
                if (Victim.CountAlliesInRange(1100f) > 1)
                {
                    if (DMG > Victim.Health)
                        Render.Circle.DrawCircle(Victim.Position, 100f, Color.Cyan);
                    if (Config.Item("Relic_PreLastHit").GetValue<bool>() && DMG + 100 > Victim.Health)
                        Render.Circle.DrawCircle(Victim.Position, 95f, Color.DarkCyan);
                }
            }
        }
        static void Draw_FaceOfTheMountain()
        {
            if (!FaceOfTheMountain.IsOwned())
                return;

            float DMG = Player.TotalAttackDamage() + 400;
            foreach (Obj_AI_Base Victim in MinionManager.GetMinions(2000f))
            {
                if (Victim.CountAlliesInRange(1100f) > 1)
                {
                    if (DMG > Victim.Health)
                        Render.Circle.DrawCircle(Victim.Position, 100f, Color.Cyan);
                    if (Config.Item("Relic_PreLastHit").GetValue<bool>() && DMG + 100 > Victim.Health)
                        Render.Circle.DrawCircle(Victim.Position, 95f, Color.DarkCyan);
                }
            }
        }
        static void Draw_RelicLine()
        {
            Draw_RelicShield();
            Draw_TargonsBrace();
            Draw_FaceOfTheMountain();
        }

        static void Draw_AnnieQ()
        {
            if (Player.ChampionName != "Annie")
                return;

            foreach (Obj_AI_Base Victim in MinionManager.GetMinions(2000f))
            {
                if (Q.IsKillable(Victim))
                    Render.Circle.DrawCircle(Victim.Position, 110f, Color.Orange);
            }
        }

        static void OnDraw(EventArgs args)
        {
            if (Config.Item("AA_LastHit").GetValue<bool>())
                Draw_AA();
            if (Config.Item("AA_PreLastHit").GetValue<bool>())
                Draw_NearDeath();
            if (Config.Item("Relic_LastHit").GetValue<bool>())
                Draw_RelicLine();
            if (Config.Item("AnnieQ_LastHit").GetValue<bool>())
                Draw_AnnieQ();
        }
        static void OnLoad(EventArgs args)
        {
            InitMenu();
            InitSpells();
            InitItems();

            Game.PrintChat("[Module] Last Hit Helper - ACTIVATED!");

            Drawing.OnDraw += OnDraw;
        }
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
