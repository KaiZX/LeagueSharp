using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Just_Auto_MasterYi
{
    class Program
    {
        // Init Player
        static Obj_AI_Hero Player = ObjectManager.Player;
        static string CHAR_NAME = "MasterYi";

        // Skill Variables
        static Spell Q, W, E, R;
        static SpellSlot SmiteSlot;
        // Skill Init function
        static void InitSpells()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 0f);
            E = new Spell(SpellSlot.E, 250f);
            R = new Spell(SpellSlot.R, 250f);
        }
        // Cast Functions
        static void CastQ(Obj_AI_Base Target)
        {
            bool CanUse = Q.IsReady();
            if (CanUse)
                Q.Cast(Target, UsePackets);
        }
        static void CastW()
        {
            bool CanUse = W.IsReady();
            if (CanUse)
                W.Cast(UsePackets);
        }
        static void CastE()
        {
            bool CanUse = E.IsReady();
            if (CanUse)
                E.Cast(UsePackets);
        }
        static void CastR()
        {
            bool CanUse = R.IsReady();
            if (CanUse)
                R.Cast(UsePackets);
        }
        // Smite Cast
        static void CastSmite(Obj_AI_Base Target)
        {
            if (!Target.IsValidTarget())
                return;

            SmiteSlot = Player.GetSpellSlot("s5_summonersmiteplayerganker");
            if (SmiteSlot == SpellSlot.Unknown)
                SmiteSlot = Player.GetSpellSlot("s5_summonersmiteduel");
            if (SmiteSlot == SpellSlot.Unknown)
                return;

            bool CanUse = Player.Spellbook.GetSpell(SmiteSlot).IsReady();
            if (CanUse)
                Player.Spellbook.CastSpell(SmiteSlot, Target);
        }
        
        // Item Variables
        static Items.Item Ghostblade;
        // Item Init function
        static void InitItems()
        {
            Ghostblade = new Items.Item((int)ItemId.Youmuus_Ghostblade, 300f);
        }
        // Item Cast Functions
        static void CastGhostblade(Obj_AI_Base Target)
        {
            bool CanUse = Ghostblade.IsReady() && Target.IsValidTarget(Ghostblade.Range);
            if (CanUse)
                Ghostblade.Cast();
        }
        // All Items Function
        static void CastAllItems(Obj_AI_Base Target)
        {
            if (Target.IsValidTarget())
            {
                CastGhostblade(Target);
            }
        }

        // Init Menu
        static Menu Config;
        // Function
        static void InitMenu()
        {
            Config = new Menu("Just Auto MasterYi", "Just_Auto_MasterYi", true);

            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("TargetSelector", "TargetSelector")));

            Config.AddItem(new MenuItem("Label1", "- Core Functions -"));
            Config.AddItem(new MenuItem("KeyToggle", "Toggle Key").SetValue(new KeyBind(84, KeyBindType.Toggle)));

            Config.AddItem(new MenuItem("Label2", "- AutoCast Functions -"));
            Config.AddItem(new MenuItem("Auto_E", "Cast E if near enemy").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));
            Config.AddItem(new MenuItem("Auto_R", "Cast R if near enemy").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));
            Config.AddItem(new MenuItem("Auto_Items", "Cast AllItems if near enemy").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));
            Config.AddItem(new MenuItem("Auto_Smite", "Cast Smite if near enemy").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));

            Config.AddItem(new MenuItem("Label3", "- KillSecure Functions -"));
            Config.AddItem(new MenuItem("KillSecure_Q", "Cast Q if will kill").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));

            Config.AddItem(new MenuItem("Label4", "- Drawing Functions -"));
            Config.AddItem(new MenuItem("Draw_KillSecure_Q", "Draw Q KillSecure Circle").SetValue(true));
            Config.AddItem(new MenuItem("Draw_OptimalTarget", "Draw Optimal Target").SetValue(true));

            Config.AddItem(new MenuItem("Label5", "- Misc Functions -"));
            Config.AddItem(new MenuItem("UsePackets", "Use packet cast?").SetValue(true));  

            Config.AddToMainMenu();
        }
        // Packet Variable
        static bool UsePackets { get { return Config.Item("UsePackets").GetValue<bool>(); } }

        static bool KeyToggled { get { return Config.Item("KeyToggle").GetValue<KeyBind>().Active; } }
        static int KillSecureQSetting { get { return Config.Item("KillSecure_Q").GetValue<StringList>().SelectedIndex; } }
        static int AutoESetting { get { return Config.Item("Auto_E").GetValue<StringList>().SelectedIndex; } }
        static int AutoRSetting { get { return Config.Item("Auto_R").GetValue<StringList>().SelectedIndex; } }
        static int AutoItemsSetting { get { return Config.Item("Auto_Items").GetValue<StringList>().SelectedIndex; } }
        static int AutoSmiteSetting { get { return Config.Item("Auto_Smite").GetValue<StringList>().SelectedIndex; } }

        static bool bKillSecureQSetting { get { return KillSecureQSetting == 2 || (KillSecureQSetting == 1 && KeyToggled); } }
        static bool bAutoESetting { get { return AutoESetting == 2 || (AutoESetting == 1 && KeyToggled); } }
        static bool bAutoRSetting { get { return AutoRSetting == 2 || (AutoRSetting == 1 && KeyToggled); } }
        static bool bAutoItemsSetting { get { return AutoItemsSetting == 2 || (AutoItemsSetting == 1 && KeyToggled); } }
        static bool bAutoSmiteSetting { get { return AutoSmiteSetting == 2 || (AutoSmiteSetting == 1 && KeyToggled); } }

        // Functional Stuff
        static void DoKillSecure()
        {
            bool CanQ = bKillSecureQSetting && Q.IsReady();

            if (CanQ)
            {
                Obj_AI_Hero TargetQ = HeroManager.Enemies.Where(X => X.IsValidTarget(Q.Range) && Q.IsKillable(X)).FirstOrDefault();
                if (TargetQ != null)
                {
                    CastQ(TargetQ);
                    return;
                }
            }  
        }
        static void DoAutoCast()
        {
            bool CanE = bAutoESetting;
            bool CanR = bAutoRSetting;
            bool CanS = bAutoSmiteSetting;
            bool CanI = bAutoItemsSetting;

            var Target = TargetSelector.GetTarget(300f, TargetSelector.DamageType.Physical);
            if (!Target.IsValidTarget())
                return;

            if (CanE)
                CastE();
            if (CanR)
                CastR();
            if (CanS)
                CastSmite(Target);
            if (CanI)
                CastAllItems(Target);
        }
        // Drawing Stuff
        static void DrawKillSecures()
        {
            if (Config.Item("Draw_KillSecure_Q").GetValue<bool>() && Q.IsReady())
            {
                foreach (Obj_AI_Hero Victim in HeroManager.Enemies.Where(X => X.IsValidTarget() && Q.IsKillable(X)))
                {
                    Render.Circle.DrawCircle(Victim.Position, Q.Range, Color.Red);
                }
            }
        }
        static void DrawOptimalTarget()
        {
            if (!Config.Item("Draw_OptimalTarget").GetValue<bool>())
                return;

            Obj_AI_Hero Target = TargetSelector.GetTarget(1250f, TargetSelector.DamageType.Physical);
            if (Target.IsValidTarget())
                Render.Circle.DrawCircle(Target.Position, 100f, Color.Lime);
        }

        // Main
        static void OnDraw(EventArgs args)
        {
            DrawKillSecures();
            DrawOptimalTarget();
        }
        static void OnTick(EventArgs args)
        {
            DoKillSecure();
            DoAutoCast();
        }
        static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != CHAR_NAME)
            {
                Game.PrintChat("Sorry, this Just Auto only for MasterYi!");
                return;
            }
            Game.PrintChat("Just Auto MasterYi! Enjoy your game!");

            InitMenu();
            InitItems();
            InitSpells();

            Game.OnGameUpdate += OnTick;
            Drawing.OnDraw += OnDraw;
        }
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
