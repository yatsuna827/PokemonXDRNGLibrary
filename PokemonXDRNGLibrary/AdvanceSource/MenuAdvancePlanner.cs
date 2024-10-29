using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;
using PokemonXDRNGLibrary.GroupBattle;
using PokemonXDRNGLibrary.QuickBattle;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PokemonXDRNGLibrary.AdvanceSource
{
    public enum Menus
    {
        MainMenu,
        VSMode,
        QuickBattle,
        GroupBattle,
        AnyDifficulty, //TODO rename to CPUBattle
        // TwoPlayerBattle,
        RumbleSave,
        Namescreen,
    }

    public struct MenuInput
    {
        public Menus Menu { get; set; }
        public uint Advances { get; set; }
        public uint NextSeed { get; set; }

        public MenuInput(Menus menu, uint advances, uint nextSeed)
        {
            this.Menu = menu;
            this.Advances = advances;
            this.NextSeed = nextSeed;
        }

        public override string ToString() // makes looking at debug values easier
        {
            return $"{Menu} | {Advances} | {NextSeed:X}";
        }
    }

    public class MenuAdvancePlanner
    {
        private readonly Dictionary<Menus, Dictionary<Menus, Func<uint, uint>>> graph;
        private readonly QuickBattleGenerator qbGenerator;
        private readonly GroupBattleGenerator gbGenerator;
        private HashSet<(Menus, uint)> attemptedPaths = new HashSet<(Menus, uint)>();
        private List<Queue<MenuInput>> paths = new List<Queue<MenuInput>>();

        public MenuAdvancePlanner(uint tsv)
        {
            graph = new Dictionary<Menus, Dictionary<Menus, Func<uint, uint>>>()
            {
                {
                    Menus.MainMenu,
                    new Dictionary<Menus, Func<uint, uint>>()
                    {
                        { Menus.VSMode, NoAdvances },
                        { Menus.RumbleSave, RumbleSave },
                        { Menus.Namescreen, NameScreen }
                    }
                },
                {
                    Menus.VSMode,
                    new Dictionary<Menus, Func<uint, uint>>()
                    {
                        { Menus.QuickBattle, QuickBattle },
                        { Menus.GroupBattle, GroupBattle },
                        { Menus.MainMenu, NoAdvances }
                    }
                },
                {
                    Menus.QuickBattle,
                    new Dictionary<Menus, Func<uint, uint>>()
                    {
                        { Menus.VSMode, NoAdvances },
                        { Menus.AnyDifficulty, AnyDifficulty }
                    }
                },
                {
                    Menus.GroupBattle,
                    new Dictionary<Menus, Func<uint, uint>>()
                    {
                        { Menus.VSMode, NoAdvances }
                    }
                },
                {
                    Menus.AnyDifficulty,
                    new Dictionary<Menus, Func<uint, uint>>()
                    {
                        { Menus.QuickBattle, NoAdvances }
                    }
                },
                {
                    Menus.RumbleSave,
                    new Dictionary<Menus, Func<uint, uint>>()
                    {
                        { Menus.MainMenu, NoAdvances }
                    }
                },
                {
                    Menus.Namescreen,
                    new Dictionary<Menus, Func<uint, uint>>()
                    {
                        { Menus.MainMenu, NoAdvances }
                    }
                }
            };

            qbGenerator = new QuickBattleGenerator(tsv);
            gbGenerator = new GroupBattleGenerator(tsv);
        }

        private uint QuickBattle(uint seed) => qbGenerator.EnterQuickBattle(seed);
        private uint GroupBattle(uint seed) => gbGenerator.EnterGroupBattle(seed);
        private uint AnyDifficulty(uint seed) => qbGenerator.Use(seed); //TODO add node for TwoPlayerBattle
        private uint NoAdvances(uint seed) => seed;
        private uint RumbleSave(uint seed) => seed.NextSeed(40);
        private uint NameScreen(uint seed) => seed.NextSeed(2);

        // best to keep targetAdvances in the range of ]0:150000] 
        public List<Queue<MenuInput>> FindPaths(uint startSeed, uint targetAdvances, bool palVersion, Menus start,  uint maxNameScreens, uint maxRumbleSaves, CancellationToken token)
        {
            var path = new Queue<MenuInput>();
            attemptedPaths?.Clear();
            paths?.Clear();
            uint rumbleSaves = 0;
            uint nameScreens = 0;

            var restrictions = new SearchRestrictions(maxRumbleSaves, maxNameScreens, rumbleSaves, nameScreens, palVersion);
            FindPaths(startSeed, targetAdvances, start, restrictions, path, token);

            return paths;
        }

        private void FindPaths(uint startSeed, uint targetAdvances, Menus start, SearchRestrictions restrictions, Queue<MenuInput> path, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            foreach (var route in graph[start])
            {
                // already tried this option from this seed -> each path only gets checked once
                if (!attemptedPaths.Add((route.Key, startSeed)))
                    continue;
                if (!restrictions.Check(route.Key))
                    continue;

                var currentPath = new Queue<MenuInput>(path);

                uint nextSeed = route.Value(startSeed);
                uint advances = nextSeed.GetIndex(startSeed);

                currentPath.Enqueue(new MenuInput(route.Key, advances, nextSeed));

                long sum = currentPath.Sum(_ => _.Advances);

                if (sum == targetAdvances)
                    paths.Add(currentPath);
                else if (sum > targetAdvances)
                    continue;
                else
                    FindPaths(nextSeed, targetAdvances, route.Key, restrictions, currentPath, token);
            }

        }

        private struct SearchRestrictions
        {
            public uint maxRumbleSaves;
            public uint maxNameScreens;
            public uint rumbleSaves;
            public uint nameScreens;
            public bool palVersion;

            public SearchRestrictions(uint maxRumbleSaves, uint maxNameScreens, uint rumbleSaves, uint nameScreens, bool palVersion)
            {
                this.maxRumbleSaves = maxRumbleSaves;
                this.maxNameScreens = maxNameScreens;
                this.rumbleSaves = rumbleSaves;
                this.nameScreens = nameScreens;
                this.palVersion = palVersion;
            }

            public bool Check(Menus nextMenu)
            {
                // this can be as high as it wants and by using x++ instead of ++x it avoids an off by 1 scenario for ==
                if (nextMenu == Menus.RumbleSave && rumbleSaves++ >= maxRumbleSaves)
                    return false;
                // this can be as high as it wants and by using x++ instead of ++x it avoids an off by 1 scenario for ==
                else if (nextMenu == Menus.Namescreen && (!palVersion || nameScreens++ >= maxNameScreens))
                    return false;
                else
                    return true;
            }
        }


        #region old implementation without struct

        [Obsolete]
        public List<Queue<MenuInput>> FindPaths_Old(uint startSeed, uint targetAdvances, bool palVersion, Menus start, uint maxNameScreens, uint maxRumbleSaves, CancellationToken token)
        {
            var path = new Queue<MenuInput>();
            attemptedPaths?.Clear();
            paths?.Clear();
            uint rumbleSaves = 0;
            uint nameScreens = 0;

            FindPaths_Old(startSeed, targetAdvances, palVersion, start, maxRumbleSaves, maxNameScreens, rumbleSaves, nameScreens, path, token);

            return paths;
        }

        [Obsolete]
        private void FindPaths_Old(uint startSeed, uint targetAdvances, bool palVersion, Menus start, uint maxRumbleSaves, uint maxNameScreens, uint rumbleSaves, uint nameScreens, Queue<MenuInput> path, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            foreach (var route in graph[start])
            {
                // already tried this option from this seed -> each path only gets checked once
                if (!attemptedPaths.Add((route.Key, startSeed)))
                    continue;
                // this can be as high as it wants and by using x++ instead of ++x it avoids and off by 1 scenario for ==
                if (route.Key == Menus.RumbleSave && rumbleSaves++ >= maxRumbleSaves)
                    continue;
                // this can be as high as it wants and by using x++ instead of ++x it avoids and off by 1 scenario for ==
                else if (route.Key == Menus.Namescreen && (!palVersion || nameScreens++ >= maxNameScreens))
                    continue;

                var currentPath = new Queue<MenuInput>(path);

                uint nextSeed = route.Value(startSeed);
                uint advances = nextSeed.GetIndex(startSeed);

                currentPath.Enqueue(new MenuInput(route.Key, advances, nextSeed));

                long sum = currentPath.Sum(_ => _.Advances);

                if (sum == targetAdvances)
                    paths.Add(currentPath);
                else if (sum > targetAdvances)
                    continue;
                else
                    FindPaths_Old(nextSeed, targetAdvances, palVersion, route.Key, maxRumbleSaves, maxNameScreens, rumbleSaves, nameScreens, currentPath, token);
            }

        }

        #endregion
    }
}
