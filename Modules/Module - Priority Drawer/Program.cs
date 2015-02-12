using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

namespace Module_Priority_Drawer
{
    class Program
    {
        static float GetEAR(Obj_AI_Base Target)
        {
            float Health = Target.Health;
            float Armor = Target.Armor + Target.FlatArmorMod;
            float EAR = Health * (1 + (Armor / 100));
            return EAR;
        }
        static float GetEMR(Obj_AI_Base Target)
        {
            float Health = Target.Health;
            float MagicArmor = Target.SpellBlock + Target.FlatSpellBlockMod;
            float EMR = Health * (1 + (MagicArmor / 100));
            return EMR;
        }
        static float GetETR(Obj_AI_Base Target)
        {
            float Health = Target.Health;
            float Armor = Target.Armor + Target.FlatArmorMod;
            float MagicArmor = Target.SpellBlock + Target.FlatSpellBlockMod;
            float ETR = Health * (1 + (Armor / 100)) * (1 + (MagicArmor / 100));
            return ETR;
        }

        static Color ColorFull = Color.FromArgb(255, 255, 255);
        static Color ColorEATR = Color.FromArgb(255, 255, 0);
        static Color ColorEMTR = Color.FromArgb(0, 255, 255);
        static Color ColorEAMR = Color.FromArgb(255, 0, 255);
        static Color ColorEAR = Color.FromArgb(255, 127, 0);
        static Color ColorEMR = Color.FromArgb(0, 127, 255);
        static Color ColorETR = Color.FromArgb(127, 127, 127);

        // Menu Variable
        static Menu Config;
        // Menu Initalization
        static void InitMenu()
        {
            Config = new Menu("[Module] Priority Drawer", "Module_PriorityDrawer", true);

            Config.AddItem(new MenuItem("Label1", "- Main Settings -"));
            Config.AddItem(new MenuItem("Enabled", "Is Enabled?").SetValue(true));
            Config.AddItem(new MenuItem("Range", "Range to search targets").SetValue(new Slider(1250, 600, 2000)));

            Config.AddToMainMenu();
        }

        static void OnDraw(EventArgs args)
        {
            if (!Config.Item("Enabled").GetValue<bool>())
                return;

            int Range = Config.Item("Range").GetValue<Slider>().Value;
            Obj_AI_Hero EAR_T = HeroManager.Enemies.Where(X => X.IsValidTarget(Range)).MinOrDefault(X => GetEAR(X));
            Obj_AI_Hero EMR_T = HeroManager.Enemies.Where(X => X.IsValidTarget(Range)).MinOrDefault(X => GetEMR(X));
            Obj_AI_Hero ETR_T = HeroManager.Enemies.Where(X => X.IsValidTarget(Range)).MinOrDefault(X => GetETR(X));

            if (ETR_T != null && EAR_T == EMR_T && EMR_T == ETR_T)
            {
                Render.Circle.DrawCircle(ETR_T.Position, 100f, ColorFull, 15);
                return;
            }
            if (EAR_T != null && EMR_T != null && EAR_T == ETR_T)
            {
                Render.Circle.DrawCircle(ETR_T.Position, 100f, ColorEATR, 10);
                Render.Circle.DrawCircle(EMR_T.Position, 100f, ColorEMR, 5);
                return;
            }
            if (EMR_T != null && EAR_T != null && EMR_T == ETR_T)
            {
                Render.Circle.DrawCircle(ETR_T.Position, 100f, ColorEMTR, 10);
                Render.Circle.DrawCircle(EAR_T.Position, 100f, ColorEAR, 5);
                return;
            }
            if (EAR_T != null && ETR_T != null && EAR_T == EMR_T)
            {
                Render.Circle.DrawCircle(EAR_T.Position, 100f, ColorEAMR, 10);
                Render.Circle.DrawCircle(ETR_T.Position, 100f, ColorETR, 5);
                return;
            }
            if (EAR_T != null)
                Render.Circle.DrawCircle(EAR_T.Position, 100f, ColorEAR, 5);
            if (EMR_T != null)
                Render.Circle.DrawCircle(EMR_T.Position, 100f, ColorEMR, 5);
            if (ETR_T != null)
                Render.Circle.DrawCircle(ETR_T.Position, 100f, ColorETR, 5);
        }
        static void OnLoad(EventArgs args)
        {
            InitMenu();

            Game.PrintChat("[Module] Priority Drawer - ACTIVATED!");
            Game.PrintChat("More reddish color: Target weakest to physical damage");
            Game.PrintChat("More bluish color: Target weakest to magical damage");
            Game.PrintChat("Gray/white color: Target weakest at ALL types of damage!");
            Drawing.OnDraw += OnDraw;
        }
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
