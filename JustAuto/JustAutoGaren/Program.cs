using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

namespace Just_Auto_Garen
{
    class Program
    {
        // Player Init
        static Obj_AI_Hero Player = ObjectManager.Player;
        static string CHAR_NAME = "Garen";

        // Skill Var
        static Spell Q, W, E, R;
        // Skill Init Function
        static void InitSpell()
        {
            Q = new Spell(SpellSlot.Q, 150f);
            W = new Spell(SpellSlot.W, 150f);
            E = new Spell(SpellSlot.E, 150f);
            R = new Spell(SpellSlot.R, 400f);
            R.SetTargetted(0.13f, 900f);
        }

        // Menu Var
        static Menu Config;
        // Menu Init Function
        static void InitMenu()
        {
            Config = new Menu("Just Auto Garen", "JA_Garen", true);
            StringList Roulette = new StringList(new[] {"Always off", "On Toggle", "Always on"}, 2);

            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("TargetSelector", "TargetSelector")));

            Config.AddItem(new MenuItem("Label1", "- Toggle Key -"));
            Config.AddItem(new MenuItem("ToggleKey", "Toggle Key").SetValue(new KeyBind(84, KeyBindType.Toggle))); // T

            Config.AddItem(new MenuItem("Label2", "- Main Finctions -"));
            Config.AddItem(new MenuItem("AutoW", "Auto W Usage").SetValue(Roulette));
            Config.AddItem(new MenuItem("AutoE", "Auto E Usage").SetValue(Roulette));
            Config.AddItem(new MenuItem("AutoR", "Auto R KillSequring").SetValue(Roulette));
            
            Config.AddItem(new MenuItem("Label3", "- Drawing Functions -"));
            Config.AddItem(new MenuItem("DrawOpt", "Draw optimal target to attack").SetValue(true));
            Config.AddItem(new MenuItem("AutoR_Draw", "Draw R range for OneHitKO targets").SetValue(true));

            Config.AddItem(new MenuItem("Label4", "- Other Functions -"));
            Config.AddItem(new MenuItem("Packet", "Try packet cast").SetValue(true));

            Config.AddToMainMenu();
        }
        // PacketCast
        static bool PacketCast { get { return Config.Item("Packet").GetValue<bool>(); } }
        // IsToggleActive
        static bool IsToggleActive { get { return Config.Item("Togglekey").GetValue<KeyBind>().Active; } }
        static int AutoWState { get { return Config.Item("AutoW").GetValue<StringList>().SelectedIndex; } }
        static int AutoEState { get { return Config.Item("AutoE").GetValue<StringList>().SelectedIndex; } }
        static int AutoRState { get { return Config.Item("AutoR").GetValue<StringList>().SelectedIndex; } }

        // Auto Functions
        static void CastW()
        {
            if (W.IsReady())
                W.Cast(PacketCast);
        }
        static void CastE()
        {
            if (!E.IsReady() || Player.HasBuff("GarenE") || Player.HasBuff("GarenQ") || Player.Position.CountEnemiesInRange(E.Range) == 0)
                return;
            E.Cast(PacketCast);
        }
        static void CastR()
        {
            if (!R.IsReady())
                return;

            foreach (Obj_AI_Hero Victim in HeroManager.Enemies)
            {
                if (Victim.IsValidTarget(R.Range) && R.IsKillable(Victim))
                {
                    R.Cast(Victim, PacketCast);
                    return;
                }
            }
        }
        // Draw Functions
        static void DrawR()
        {
            foreach (Obj_AI_Hero Victim in HeroManager.Enemies)
            {
                if (Victim.IsValidTarget() && R.IsKillable(Victim))
                    Render.Circle.DrawCircle(Victim.Position, R.Range, Color.Red);
            }
        }
        static void DrawOptimalTarget()
        {
            Obj_AI_Hero Target = TargetSelector.GetTarget(1250f, TargetSelector.DamageType.Physical);
            if (Target.IsValidTarget())
                Render.Circle.DrawCircle(Target.Position, 100f, Color.Lime);
        }

        // Main Functions
        static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.IsDead)
                return;
            if (!sender.IsEnemy)
                return;
            if (sender.Type != GameObjectType.obj_AI_Hero)
                return;
            if (args.SData.IsAutoAttack())
                return;

            
            if (args.Target == Player || args.End.GetAlliesInRange(120f).Any(X => X.IsMe) )
            {
                if ((AutoWState == 1 && IsToggleActive) || AutoWState == 2)
                    CastW();
            }
        }
        static void OnTick(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if ((AutoRState == 1 && IsToggleActive) || AutoRState == 2)
                CastR();
            if ((AutoEState == 1 && IsToggleActive) || AutoEState == 2)
                CastE();
        }
        static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Config.Item("AutoR_Draw").GetValue<bool>() && R.IsReady())
                DrawR();
            if (Config.Item("DrawOpt").GetValue<bool>())
                DrawOptimalTarget();
        }
        static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != CHAR_NAME)
            {
                Game.PrintChat("Sorry, this 'Just Auto' for {0} only.", CHAR_NAME);
                return;
            }
            Game.PrintChat("Just Auto {0}. Enjoy!", CHAR_NAME);

            InitSpell();
            InitMenu();
            Game.OnGameUpdate += OnTick;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Hero.OnProcessSpellCast += OnCast;
        }
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
