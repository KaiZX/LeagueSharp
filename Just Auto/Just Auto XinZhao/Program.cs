using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

/*
 * No pressing buttons except (Q)(W)ER. For Ever.
 * 
 * Functions:
 *   Cast W if Q is casted. (Q-W combo)
 *   Cast Q if E is casted. (So it means, E-Q-W combo on E)
 *   Use items if E casted on enemy hero (Ghostblade, for example)
 *   Use E if will KillSecure.
 *   Use E-R if will KS and there is more than 1 person nearby.
 *   TODO Use Q-W-Items if attacking enemy.
 *   
 *   NEW!: Use Challenging Smite / Frost Smite on E.
 *   
 *   TODO Ultima Function: if Rightclicked enemy, cast all skills on this enemy and attack him!!!
 * 
*/

namespace Just_Auto_XinZhao
{
    class Program
    {
        // Player Variable
        static Obj_AI_Hero Player = ObjectManager.Player;
        // Player Character Name
        static string CharName = "XinZhao";

        // Skill Variables
        static Spell Q, W, E, R;
        static SpellSlot SmiteSlot;
        // Skill Init function
        static void InitSpells()
        {
            Q = new Spell(SpellSlot.Q, 350f);
            W = new Spell(SpellSlot.W, 350f);
            E = new Spell(SpellSlot.E, 600f);
            E.SetTargetted(0.5f, 1750f);
            R = new Spell(SpellSlot.R, 375f);
        }
        // Cast Functions
        static void CastQ()
        {
            bool CanUse = Q.IsReady();
            if (CanUse)
                Q.Cast(UsePackets);
        }
        static void CastW()
        {
            bool CanUse = W.IsReady();
            if (CanUse)
                W.Cast(UsePackets);
        }
        static void CastE(Obj_AI_Base Target)
        {
            bool CanUse = E.IsReady() && Target.IsValidTarget();
            if (CanUse)
                E.Cast(Target, UsePackets);
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
            SmiteSlot = Player.GetSpellSlot("s5_summonersmiteplayerganker");
            if (SmiteSlot == SpellSlot.Unknown)
                SmiteSlot = Player.GetSpellSlot("s5_summonersmiteduel"); 

            if (SmiteSlot == SpellSlot.Unknown)
                return;

            bool CanUse = Player.Spellbook.GetSpell(SmiteSlot).IsReady() && Target.IsValidTarget();
            if (CanUse)
                Player.Spellbook.CastSpell(SmiteSlot, Target);
        }

        // Item Variables
        static Items.Item Ghostblade;
        // Item Init function
        static void InitItems()
        {
            Ghostblade = new Items.Item((int)ItemId.Youmuus_Ghostblade, 350f);
        }
        // Item Cast Functions
        static void CastGhostblade()
        {
            bool CanUse = Ghostblade.IsReady();
            if (CanUse)
                Ghostblade.Cast();
        }
        // All Items Function
        static void CastAllItems(Obj_AI_Base Target)
        {
            if (Target.IsValidTarget())
            {
                CastGhostblade();
            }
        }

        // Menu Variable
        static Menu Config;
        // Menu Init function
        static void InitMenu()
        {
            Config = new Menu("Xin Zhao Helper", "XinHelper", true);

            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("TargetSelector", "TargetSelector")));
            Config.AddItem(new MenuItem("Label0", "- ToggleKey -"));
            Config.AddItem(new MenuItem("KeyToggle", "Toggle Key").SetValue(new KeyBind(84, KeyBindType.Toggle)));

            Config.AddItem(new MenuItem("Label1", "- Core functions -"));
            Config.AddItem(new MenuItem("Label2", "- Q functions -"));
            Config.AddItem(new MenuItem("Q_Setting", "Cast Q condition").SetValue(new StringList(new[] { "Never", "After W", "After E", "Always if near enemy hero" }, 0)));
            Config.AddItem(new MenuItem("Q_SettingAA", "Cast Q after AA").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));

            Config.AddItem(new MenuItem("Label3", "- W functions -"));
            Config.AddItem(new MenuItem("W_Setting", "Cast W condition").SetValue(new StringList(new[] { "Never", "After Q", "After E", "Always if near enemy hero" }, 0)));

            Config.AddItem(new MenuItem("Label4", "- E functions -"));
            Config.AddItem(new MenuItem("E_Setting", "(Not working) Cast E condition").SetValue(new StringList(new[] { "Never", "Mouse Selected Target + Toggle", "Mouse Selected Target Always" }, 0)));
            Config.AddItem(new MenuItem("Smite_After_E", "Cast SMITE if can?").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));
            Config.AddItem(new MenuItem("Items_After_E", "Cast ITEMS if can?").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));

            Config.AddItem(new MenuItem("Label5", "- (WIP) R functions -"));
            // Cast R: ... When?

            Config.AddItem(new MenuItem("Label6", "- KillSecure functions -"));
            Config.AddItem(new MenuItem("KillSecure_E", "Cast E if will kill").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));
            Config.AddItem(new MenuItem("KillSecureAdd_R", "(Extension) E+R combo or just R").SetValue(new StringList(new[] { "Always Off", "On Toggle", "Always On" }, 2)));

            Config.AddItem(new MenuItem("Label7", "- Drawing functions -"));
            Config.AddItem(new MenuItem("DrawKillSecureTargets", "Draw KillSecure Circles").SetValue(true));
            Config.AddItem(new MenuItem("DrawOptimal", "Draw optimal target").SetValue(true));

            Config.AddItem(new MenuItem("Label8", "- Other functions -"));
            Config.AddItem(new MenuItem("UsePackets", "Use packet cast?").SetValue(false));            

            Config.AddToMainMenu();
        }
        // Packet Variable
        static bool UsePackets { get { return Config.Item("UsePackets").GetValue<bool>(); } }

        // Roulette Variables
        static bool KeyToggled { get { return Config.Item("KeyToggle").GetValue<KeyBind>().Active; } }
        static int Q_SettingStatus { get { return Config.Item("Q_Setting").GetValue<StringList>().SelectedIndex; } }
        static int Q_SettingAAStatus { get { return Config.Item("Q_SettingAA").GetValue<StringList>().SelectedIndex; } }
        static int W_SettingStatus { get { return Config.Item("W_Setting").GetValue<StringList>().SelectedIndex; } }
        // static int E_SettingStatus { get { return Config.Item("E_Setting").GetValue<StringList>().SelectedIndex; } }
        static int ESMITE_SettingStatus { get { return Config.Item("Smite_After_E").GetValue<StringList>().SelectedIndex; } }
        static int EITEMS_SettingStatus { get { return Config.Item("Items_After_E").GetValue<StringList>().SelectedIndex; } }
        static int EKS_SettingStatus { get { return Config.Item("KillSecure_E").GetValue<StringList>().SelectedIndex; } }
        static int RKS_SettingStatus { get { return Config.Item("KillSecureAdd_R").GetValue<StringList>().SelectedIndex; } }

        // KS function
        static void KillSecure()
        {
            var CanE = E.IsReady() && ((EKS_SettingStatus == 2) || (EKS_SettingStatus == 1 && KeyToggled));
            var CanR = R.IsReady() && ((RKS_SettingStatus == 2) || (RKS_SettingStatus == 1 && KeyToggled));
            Obj_AI_Hero Victim = null;

            if (CanE)
            {
                Victim = HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && E.IsKillable(x)).FirstOrDefault();
                if (Victim != null)
                {
                    CastE(Victim);
                    return;
                }
                if (CanR)
                {
                    Victim = HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && (E.GetDamage(x) + R.GetDamage(x) > x.Health)).FirstOrDefault();
                    if (Victim != null)
                        CastE(Victim);
                    return;
                }
            }
            if (CanR)
            {
                Victim = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && R.IsKillable(x)).FirstOrDefault();
                if (Victim != null)
                    CastR();
                return;
            }
        }
        // Just "NearEnemySkill" function
        static void Skills()
        {
            var Target = HeroManager.Enemies.Where(x => Player.Distance3D(x) < 250f).FirstOrDefault();
            if (Target != null)
            {
                if (Q_SettingStatus == 3)
                    CastQ();
                if (W_SettingStatus == 3)
                    CastW();
            }
        }
        // Draw ER KS functions
        static void DrawEKS()
        {
            var CanE = E.IsReady() && ((EKS_SettingStatus == 2) || (EKS_SettingStatus == 1 && KeyToggled));
            var CanR = R.IsReady() && ((RKS_SettingStatus == 2) || (RKS_SettingStatus == 1 && KeyToggled));

            if (CanE)
            foreach (Obj_AI_Hero Victim in HeroManager.Enemies)
            {
                if (Victim.IsValidTarget() && E.IsKillable(Victim))
                    Render.Circle.DrawCircle(Victim.Position, E.Range, Color.Red);
                else if (Victim.IsValidTarget() && (E.GetDamage(Victim) + R.GetDamage(Victim) > Victim.Health) && CanR)
                    Render.Circle.DrawCircle(Victim.Position, E.Range, Color.Yellow);
            }
            if (CanR)
                foreach (Obj_AI_Hero Victim in HeroManager.Enemies)
            {
                if (Victim.IsValidTarget() && R.IsKillable(Victim))
                    Render.Circle.DrawCircle(Victim.Position, R.Range, Color.Yellow);
            }
        }
        static void DrawOptimalTarget()
        {
            if (!Config.Item("DrawOptimal").GetValue<bool>())
                return;

            var Target = TargetSelector.GetTarget(1250f, TargetSelector.DamageType.Physical);
            if (Target.IsValidTarget())
                Render.Circle.DrawCircle(Target.Position, 100f, Color.Lime);
        }

        // OnSpellCast
        static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            Obj_AI_Base Target = (Obj_AI_Base)args.Target;

            // If Q Casted
            if (args.SData.Name == Player.Spellbook.GetSpell(SpellSlot.Q).Name)
            {
                if (W_SettingStatus == 1)
                {
                    CastW();
                }
            }

            // If W Casted
            if (args.SData.Name == Player.Spellbook.GetSpell(SpellSlot.W).Name)
            {
                if (Q_SettingStatus == 1)
                {
                    CastQ();
                }
            }

            // If E Casted
            if (args.SData.Name == Player.Spellbook.GetSpell(SpellSlot.E).Name)
            {
                if ((ESMITE_SettingStatus == 2 || (ESMITE_SettingStatus == 1 && KeyToggled)) && Target.Type == GameObjectType.obj_AI_Hero)
                {
                    CastSmite(Target);
                }
                if ((EITEMS_SettingStatus == 2 || (EITEMS_SettingStatus == 1 && KeyToggled)) && Target.Type == GameObjectType.obj_AI_Hero)
                {
                    CastAllItems(Target);
                }
                if (Q_SettingStatus == 2)
                {
                    CastQ();
                }
                if (W_SettingStatus == 2)
                {
                    CastW();
                }
            }

            // If AA Casted
            if (args.SData.IsAutoAttack())
            {
                if (Q_SettingAAStatus == 2 || (Q_SettingAAStatus == 1 && KeyToggled))
                {
                    CastQ();
                }
            }
        }
        // OnTick
        static void OnTick(EventArgs args)
        {
            KillSecure();
            Skills();
        }
        static void OnDraw(EventArgs args)
        {
            DrawEKS();
            DrawOptimalTarget();
        }
        // OnLoad
        static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != CharName)
            {
                Game.PrintChat("Sorry. This script can help only Xin.");
                return;
            }

            Game.PrintChat("Xin Zhao Helper by Inferno. GL'n'HF!");

            InitSpells();
            InitItems();
            InitMenu();

            Game.OnGameUpdate += OnTick;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
        }       
        // Core Load
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
