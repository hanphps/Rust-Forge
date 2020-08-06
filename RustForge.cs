using Rust;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;

using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using Oxide.Game.Rust.Libraries;

namespace Oxide.Plugins
{



    [Info("Rust-Forge", "han", "0.1.0")]
    [Description("A Server Manager for more community + event driven servers")]

    class RustForge : CovalencePlugin
    {

        #region Config
        private Configuration config;
        class Configuration
        {
            public string Version { get; set; }
            public Settings Settings { get; set; }
            public Dictionary<string, Multiplier> DefaultMultipliers { get; set; }
            public EventManager EventManager { get; set; }
            public AnnouncementManager AnnouncementManager { get; set; }


        }

        protected override void LoadDefaultConfig()
        {
            config = new Configuration
            {

                Settings = new Settings
                {
                    Delay = 10f,
                    Log = true
                },

                DefaultMultipliers = new Dictionary<string, Multiplier>
                {
                    {"Loot", new Multiplier
                        {
                            Value = 2f,
                            WhiteList = new List<string>{"loot-barrel-1",
                                         "loot-barrel-2",
                                         "loot_barrel_1",
                                         "loot_barrel_2",
                                         "crate_underwater_basic",
                                         "crate_underwater_advanced",
                                         "foodbox",
                                         "trash-pile-1",
                                         "minecart",
                                         "oil_barrel",
                                         "crate_basic",
                                         "crate_mine",
                                         "crate_tools",
                                         "crate_normal",
                                         "crate_normal_2",
                                         "crate_normal_2_food",
                                         "crate_normal_2_medical",
                                         "crate_elite",
                                         "codelockedhackablecrate",
                                         "bradley_crate",
                                         "heli_crate",
                                         "supply_drop"
                        },
                            BlackList = new List<string>{"cctv.camera",
                                         "target.computer",
                                         "Weapon",
                                         "Attire",
                                         "Tool"
                        }
                        }
                    },
                    {"Grow", new Multiplier{Value = 2f} },
                    {"Dispenser",new Multiplier{Value = 2f } },
                    {"Quarry",new Multiplier{ Value = 2f} },
                    {"Excavator",new Multiplier{ Value = 2f} },
                    {"Collectible",new Multiplier{ Value = 2f} },
                    {"Survey",new Multiplier{ Value = 2f} }
                },

                EventManager = new EventManager
                {
                    Settings = new Settings { Delay = Convert.ToSingle(20) },
                    Messaging = new Message
                    {
                        Icon = 0,
                        Title = "",
                        TitleColor = "orange",
                        TitleSize = 30,
                        MessageColor = "white",
                        MessageSize = 20,
                        Text = ""


                    },
                    EventsLibrary = new Dictionary<string, Event>
                    {
                        ["TestEvent"] = new Event
                        {
                            Name = "TEST EVENT",
                            StartTime = "07:00:00",
                            EndTime = "12:00:00",
                            EventChance = 1,
                            ChanceRolled = false,
                            StartMessage = new Message
                            {
                                Icon = 0,
                                Title = "SERVER EVENT",
                                TitleSize = 30,
                                TitleColor = "yellow",
                                MessageColor = "white",
                                MessageSize = 20,
                                Text = "\nTEST EVENT has begun."

                            },
                            EndMessage = new Message
                            {
                                Icon = 0,
                                Title = "SERVER EVENT\n",
                                TitleSize = 30,
                                TitleColor = "yellow",
                                MessageColor = "white",
                                MessageSize = 20,
                                Text = "\nTEST EVENT has ended."

                            },

                            CanEventStart = false,
                            IsEvent = false,

                            Multipliers = new Dictionary<string, Multiplier>
                            {
                                {"StructDamage",new Multiplier{Value=100f } }


                            }
                        }
                    },


                },

                AnnouncementManager = new AnnouncementManager
                {
                    Settings = new Settings { Delay = Convert.ToSingle(120) },
                    Messaging = new Message
                    {
                        Icon = 0,
                        Title = "",
                        TitleColor = "orange",
                        TitleSize = 35,
                        MessageColor = "white",
                        MessageSize = 25,
                        Text = ""
                    },
                    Announcements = new Dictionary<string, string>
                    {
                        {"Welcome!","This is a test."}
                    }
                }

            };
            SaveConfig();
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();

            }
            catch
            {
                //Create some error handling later
                if (config.Settings.Log) LogToFile("log", "Config does not exist. Creating new default config.", this);
                LoadDefaultConfig();
            }
            SaveConfig();
        }
        protected override void SaveConfig() => Config.WriteObject(config);

        #endregion

        #region CLASSES

        #region Event

        class Event
        {
            public string Name { get; set; }
            //Timers
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            //Messages Settings
            public Message StartMessage { get; set; }
            public Message EndMessage { get; set; }
            //Variables
            public bool CanEventStart { get; set; }
            public bool IsEvent { get; set; }
            //Probability
            public int EventChance { get; set; }
            public int CurrentRoll { get; set; }
            public bool ChanceRolled { get; set; }
            // Optional Multipliers
            public Dictionary<string, Multiplier> Multipliers { get; set; }

        }

        class Multiplier
        {

            public float Value { get; set; }
            public List<string> WhiteList { get; set; }
            public List<string> BlackList { get; set; }


        }
        #endregion

        #region EventManager

        class EventManager
        {
            public Settings Settings { get; set; }
            public Timer ManagerTimer { get; set; }
            public Message Messaging { get; set; }
            public Dictionary<string, Event> EventsLibrary { get; set; }

        }

        #endregion

        #region Message
        class Message
        {
            public ulong Icon { get; set; }
            public string Title { get; set; }
            public string TitleColor { get; set; }
            public int TitleSize { get; set; }
            public string MessageColor { get; set; }
            public int MessageSize { get; set; }
            public string Text { get; set; }
        }
        #endregion

        #region AnnouncementManager
        class AnnouncementManager
        {
            public Settings Settings { get; set; }
            public Message Messaging { get; set; }
            public Timer ManagerTimer { get; set; }

            public Dictionary<string, string> Announcements { get; set; }

        }
        #endregion

        #region Settings
        public class Settings
        {
            public float Delay { get; set; }

            public bool Log { get; set; }

        }
        #endregion
        #endregion

        #region Fields
        private Timer Looper;
        private System.Random random;
        #endregion

        #region Core

        private void NotifyServer(Message settings)
        {
            if (config.Settings.Log) LogToFile("log", server.Time.TimeOfDay + ":" + "ServerMessage" + settings.Title + settings.Text, this);
            server.Broadcast(
                $"<color={settings.MessageColor}><size={settings.MessageSize}>{settings.Text}</size></color>",
                $"<color={settings.TitleColor}><size={settings.TitleSize}>{settings.Title}</size></color>",
                settings.Icon
            );

        }

        //EVENT MANAGER STUFF
        private bool EventRoll(Event evt)
        {
            if (evt.CurrentRoll == null) { evt.CurrentRoll = random.Next(1, evt.EventChance); EventRoll(evt); }
            int roll = random.Next(1, evt.EventChance);
            return (roll == evt.CurrentRoll);
        }


        private void ApplyMultipliers(Item item, string Type)
        {
            try
            {
                if (config.DefaultMultipliers[Type].BlackList.Contains(item.info.shortname) || config.DefaultMultipliers[Type].BlackList.Contains(item.info.category.ToString()) || item.IsBlueprint()) { return; }
                item.amount *= (int)config.DefaultMultipliers[Type].Value;

            }
            catch
            {
                if (config.Settings.Log) LogToFile("log", Type + ": Multiplier does not exist", this);
                item.amount *= 1;
            }
        }
        private void EventMultipliers(Item item, string Type)
        {
            foreach (Event evt in config.EventManager.EventsLibrary.Values)
            {

                if (evt.IsEvent)
                {
                    try
                    {
                        if (config.DefaultMultipliers[Type].BlackList.Contains(item.info.shortname) || config.DefaultMultipliers[Type].BlackList.Contains(item.info.category.ToString()) || item.IsBlueprint()) continue;

                        item.amount *= Convert.ToInt32(evt.Multipliers[Type].Value);

                    }
                    catch
                    {
                        if (config.Settings.Log) LogToFile("log", Type + ": Multiplier does not exist", this);
                        item.amount *= 1;
                        continue;
                    }
                }

            }
        }

        private void EventDMultipliers(HitInfo info, string Type)
        {
            foreach (Event evt in config.EventManager.EventsLibrary.Values)
            {

                if (evt.IsEvent)
                {
                    try
                    {
                        info.damageTypes.ScaleAll(evt.Multipliers[Type].Value);
                    }
                    catch
                    {
                        if (config.Settings.Log) LogToFile("log", Type + ": Multiplier does not exist", this);
                        info.damageTypes.ScaleAll(1f);
                        continue;
                    }
                }

            }
        }

        private void EM_Init()
        {
            
            foreach (Event evt in config.EventManager.EventsLibrary.Values)
            {
                evt.CurrentRoll = random.Next(1, evt.EventChance);
                evt.CanEventStart = EventRoll(evt);
                evt.ChanceRolled = true;
            }
        }

        private void EM_Run()
        {

            if (config.EventManager.ManagerTimer != null && config.EventManager.ManagerTimer.Destroyed) { config.EventManager.ManagerTimer.Destroy(); }

            foreach (Event evt in config.EventManager.EventsLibrary.Values)
            {

                if (server.Time.TimeOfDay < TimeSpan.Parse(evt.StartTime) && server.Time.TimeOfDay > TimeSpan.Parse(evt.EndTime))
                {
                    if (evt.IsEvent) { NotifyServer(evt.EndMessage); evt.IsEvent = false; }
                    if (evt.ChanceRolled == false)
                    {
                        evt.CanEventStart = EventRoll(evt);
                        evt.ChanceRolled = true;
                    }
                }
                else if (server.Time.TimeOfDay >= TimeSpan.Parse(evt.StartTime) || server.Time.TimeOfDay < TimeSpan.Parse(evt.EndTime) )
                {
                    evt.ChanceRolled = false;

                    if (evt.CanEventStart == true && (server.Time.TimeOfDay > TimeSpan.Parse(evt.StartTime) || server.Time.TimeOfDay < TimeSpan.Parse(evt.EndTime)))
                    {
                        evt.CanEventStart = false;
                        evt.IsEvent = true;
                        NotifyServer(evt.StartMessage);

                    }

                }
                //Puts(evt.ChanceRolled.ToString() + evt.CanEventStart.ToString()+evt.IsEvent.ToString());

            }

            config.EventManager.ManagerTimer = timer.Once(config.EventManager.Settings.Delay, () => EM_Run());

        }

        //ANNOUNCEMENT MANAGER STUFF
        public void AM_Run()
        {

            if (config.AnnouncementManager.ManagerTimer != null && config.AnnouncementManager.ManagerTimer.Destroyed) { config.AnnouncementManager.ManagerTimer.Destroy(); }

            var val = random.Next(0, config.AnnouncementManager.Announcements.Count);
            config.AnnouncementManager.Messaging.Title = config.AnnouncementManager.Announcements.ElementAt(val).Key;
            config.AnnouncementManager.Messaging.Text = config.AnnouncementManager.Announcements.ElementAt(val).Value;
            NotifyServer(config.AnnouncementManager.Messaging);

            config.AnnouncementManager.ManagerTimer = timer.Once(config.AnnouncementManager.Settings.Delay, () => AM_Run());

        }

        #endregion

        #region Hooks
        private void Init()
        {
            LoadConfig();

        }

        private void OnServerInitialized()
        {
            random = new System.Random();
            EM_Init();
            EM_Run();
            AM_Run();

        }

        private void OnPlayerConnected(BasePlayer player)
        {
            NotifyServer(new Message
            {
                Icon = 0,
                Title = player.displayName,
                TitleSize = 15,
                TitleColor = "white",
                MessageColor = "green",
                MessageSize = 15,
                Text = "joined the server."
            });

        }

        private void OnPlayerDisconnected(BasePlayer player)
        {
            NotifyServer(new Message
            {
                Icon = 0,
                Title = player.displayName,
                TitleSize = 15,
                TitleColor = "white",
                MessageColor = "red",
                MessageSize = 15,
                Text = "left the server."
            });

        }

        //Multiplier Stuff
        private void OnDispenserGather(ResourceDispenser dispenser, BaseEntity entity, Item item)
        {
            if (!entity.ToPlayer()) return;
            ApplyMultipliers(item, "Gather");
            EventMultipliers(item, "Gather");
        }

        private void OnDispenserBonus(ResourceDispenser dispenser, BaseEntity entity, Item item)
        {
            OnDispenserGather(dispenser, entity, item);
        }

        private void OnGrowableGathered(GrowableEntity growable, Item item, BasePlayer player)
        {
            ApplyMultipliers(item, "Grow");
            EventMultipliers(item, "Grow");
        }

        private void OnQuarryGather(MiningQuarry quarry, Item item)
        {
            ApplyMultipliers(item, "Quarry");
            EventMultipliers(item, "Quarry");
        }

        private void OnExcavatorGather(ExcavatorArm excavator, Item item)
        {
            ApplyMultipliers(item, "Excavator");
            EventMultipliers(item, "Excavator");
        }

        private void OnCollectiblePickup(Item item, BasePlayer player)
        {
            ApplyMultipliers(item, "Collectible");
            EventMultipliers(item, "Collectible");
        }

        private void OnSurveyGather(SurveyCharge surveyCharge, Item item)
        {
            ApplyMultipliers(item, "Survey");
            EventMultipliers(item, "Survey");
        }

        private void OnLootSpawn(StorageContainer cont)
        {
            timer.Once(config.Settings.Delay, () =>
            {
                if (cont == null) return;
                if (!config.DefaultMultipliers["Loot"].WhiteList.Contains(cont.ShortPrefabName)) return;

                foreach (var item in cont.inventory.itemList.ToArray())
                {
                    ApplyMultipliers(item, "Loot");
                    EventMultipliers(item, "Loot");

                }


            });
        }

        //Even Damage Stuff
        void OnFireBallDamage(FireBall fire, BaseCombatEntity entity, HitInfo info)
        {
            EventDMultipliers(info, "FireDamage");
        }

        void OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            if (entity == null || !(entity is Door) || !(entity is BuildingBlock)) { return; }
            EventDMultipliers(info, "StructDamage");

        }

        #endregion

    }
}
