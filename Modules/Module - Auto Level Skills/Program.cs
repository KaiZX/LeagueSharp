using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

namespace Module_Auto_Level_Skills
{
    class Cell
    {
        public string _champName;
        public string _caption;
        public string _description;
        public int[] _order;

        public Cell(string ChampName, string Caption, string Description, int[] SkillOrder)
        {
            _champName = ChampName;
            _caption = Caption;
            _description = Description;
            _order = SkillOrder;
        }
    }
    class Program
    {
        // Menu Variable
        static Menu _config;
        // Menu Init Function
        static void InitMenu()
        {
            _config = new Menu("[Module] Auto Level Skills", "Module_AutoLevelSkills", true);
            _config.AddItem(new MenuItem("Enabled", "Enabled?").SetValue(true));
            _config.AddToMainMenu();
        }

        // Init DataBase
        static List<Cell> DataBase = new List<Cell>();
        // Add to DB
        static void AddToDB(string ChampName, string ID, string Desc, int[] order)
        {
            DataBase.Add(new Cell(ChampName, ID, Desc, order));
        }
        // Load DataBase
        static void LoadDB()
        {
            AddToDB("Akali", "standart", "R->Q->E->W", new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 });

            AddToDB("MasterYi", "lane", "R->E->Q->W", new int[] { 1, 3, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 });
            AddToDB("MasterYi", "jungle", "R->Q->E->W", new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 });

            AddToDB("Taric", "heal", "R->W->Q->E", new int[] { 2, 3, 2, 1, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 });
            AddToDB("Taric", "stun", "R->W->E->Q", new int[] { 2, 3, 1, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 });
        }

        // Our Hero All Possibilities
        static List<Cell> LocalDataBase = new List<Cell>();
        // Add All Our Hero Possibilities
        static void LoadHero()
        {
            foreach (Cell Item in DataBase)
            {
                if (Item._champName == ObjectManager.Player.ChampionName)
                    LocalDataBase.Add(Item);
            }
        }
        // Get From ID
        static int[] GetFromID(string id)
        {
            foreach (Cell Item in DataBase)
            {
                if (Item._caption == id)
                    return Item._order;
            }

            return null;
        }
        // Transform Custom to Line
        static int[] Transform(string[] from)
        {
            if (from.Count() != 19)
                return null;

            int[] retorder = new int[18];
            int countq = 0;
            int countw = 0;
            int counte = 0;
            int countr = 0;
            for (int i=0; i<18; i++)
            {
                if (from[i+1] == "1")
                {
                    retorder[i] = 1;
                    countq++;
                }
                else if (from[i+1] == "2")
                {
                    retorder[i] = 2;
                    countw++;
                }
                else if (from[i+1] == "3")
                {
                    retorder[i] = 3;
                    counte++;
                }
                else if (from[i+1] == "4")
                {
                    retorder[i] = 4;
                    countr++;
                }
                else
                {
                    return null;
                }

                if ((countq > 1 && i+1 < 3) || (countq > 2 && i+1 < 5) || (countq > 3 && i+1 < 7) || (countq > 4 && i+1 < 9))
                    return null;
                if ((countw > 1 && i+1 < 3) || (countw > 2 && i+1 < 5) || (countw > 3 && i+1 < 7) || (countw > 4 && i+1 < 9))
                    return null;
                if ((counte > 1 && i+1 < 3) || (counte > 2 && i+1 < 5) || (counte > 3 && i+1 < 7) || (counte > 4 && i+1 < 9))
                    return null;
                if ((countr > 0 && i+1 < 6) || (countr > 1 && i+1 < 11) || (countr > 2 && i+1 < 16))
                    return null;
            }

            return retorder;
        }
        // Other Variables
        static bool IsLoaded = false;

        static void OnChatInput(GameInputEventArgs args)
        {
            if (args.Input == "")
                return;

 	        if (args.Input[0] == '#')
            {
                args.Process = false;

                if (IsLoaded)
                {
                    Game.PrintChat("<font color='#FF0000'>You're already loaded sequence, you can't change it!</font>");
                    return;
                }
           
                string[] query = args.Input.Substring(1).ToLower().Split(' ');
                if (query[0] == "auto")
                {
                    int[] order = GetFromID(query[1]);
                    if (order == null)
                    {
                        Game.PrintChat("<font color='#FF0000'>Sorry, we can't find {0} in auto database!</font>", query[1]);
                    }
                    else
                    {
                        AutoLevelPlus.Initialize(order);
                        AutoLevelPlus.Enabled(_config.Item("Enabled").GetValue<bool>());
                        IsLoaded = true;
                        Game.PrintChat("<font color='#00FF00'>Autolevel initalized successfully! Enjoy!</font>");
                    }
                }
                else if (query[0] == "custom")
                {
                    int[] order = Transform(query);
                    if (order == null)
                    {
                        Game.PrintChat("<font color='#FF0000'>Sorry, you have mistakes in your order line, can't load!</font>");
                    }
                    else
                    {
                        AutoLevelPlus.Initialize(order);
                        AutoLevelPlus.Enabled(_config.Item("Enabled").GetValue<bool>());
                        IsLoaded = true;
                        Game.PrintChat("<font color='#00FF00'>Autolevel initalized successfully! Enjoy!</font>");
                    }  
                }
                else
                {
                    Game.PrintChat("<font color='#FF0000'>Wrong command! Try again!</font>");
                }
            }
        }
        static void OnChange(object sender, OnValueChangeEventArgs args)
        {
            AutoLevelPlus.Enabled(_config.Item("Enabled").GetValue<bool>());
        }
        static void OnLoad(EventArgs args)
        {

            InitMenu();
            LoadDB();
            LoadHero();

            Game.PrintChat("<font color='#FF0000'>[<font color='#FFFFFF'>Auto Level Skills Module Initalized</font>]</font>");
            if (LocalDataBase.Count == 0)
            {
                Game.PrintChat("<font color='#FF0000'>We can't find AutoLevels for {0} in our base.</font>", ObjectManager.Player.ChampionName);
                Game.PrintChat("<font color='#00AA00'>Use #custom 1 2 3 4 5 6 7 8 9 10...18 to init autolevel manually!</font>");
            }
            else
            {
                Game.PrintChat("<font color='#00FF00'>We found premade AutoLevels for your {0}!</font>", ObjectManager.Player.ChampionName);
                Game.PrintChat("<font color='#00AA00'>Use #custom 1 2 3 4 5 6 7 8 9 10...18 to init autolevel manually!</font>");
                Game.PrintChat("<font color='#00AA00'>Or use #auto [id there] init premade autolevel!</font>");
                Game.PrintChat("<font color='#00AA00'>All ID's you can use - below.</font>");

                foreach (Cell Item in LocalDataBase)
                {
                    Game.PrintChat("<font color='#FFFF00'>{0}</font>: <font color='#00FFFF'>{1}</font>", Item._caption, Item._description);
                }
            }

            _config.Item("Enabled").ValueChanged += OnChange;
            Game.OnGameInput += OnChatInput;
        }
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
