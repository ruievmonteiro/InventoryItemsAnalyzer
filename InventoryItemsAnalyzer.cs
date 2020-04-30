using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AdvancedTooltip;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using SharpDX;

namespace InventoryItemsAnalyzer
{
    public class InventoryItemsAnalyzer : BaseSettingsPlugin<InventoryItemsAnalyzerSettings>
    {
        private const string coroutineName = "InventoryItemsAnalyzer";

        private readonly string[] _incElemDmg =
            {"FireDamagePercentage", "ColdDamagePercentage", "LightningDamagePercentage"};

        private readonly string[] _nameAttrib = {"Intelligence", "Strength", "Dexterity"};
        private List<RectangleF> _allItemsPos;
        private List<RectangleF> _goodItemsPos;
        private List<RectangleF> _highItemsPos;
        private IngameState _ingameState;
        private List<RectangleF> _VeilItemsPos;
        private Vector2 _windowOffset;
        private Coroutine CoroutineWorker;
        private int CountInventory;
        private string[] GoodBaseTypes;
        private int idenf;

        private List<string> ShitUniquesList { get; } = new List<string>
        {
			"Abberath's Hooves",
			"Abberath's Horn",
			"Abyssus",
			"Advancing Fortress",
			"Aegis Aurora",
			"Agnerod East",
			"Agnerod North",
			"Agnerod South",
			"Agnerod West",
			"Ahkeli's Mountain",
			"Ahn's Contempt",
			"Ahn's Heritage",
			"Ahn's Might",
			"Al Dhih",
			"Alberon's Warpath",
			"Algor Mortis",
			"Allelopathy",
			"Allure",
			"Ambu's Charge",
			"Amplification Rod",
			"Anatomical Knowledge",
			"Ancient Waystones",
			"Andvarius",
			"Apep's Rage",
			"Apep's Slumber",
			"Araku Tiki",
			"Architect's Hand",
			"Ascent From Flesh",
			"Asenath's Mark",
			"Ashcaller",
			"Ashrend",
			"Asphyxia's Wrath",
			"Assailum",
			"Assassin's Haste",
			"Atziri's Foible",
			"Atziri's Mirror",
			"Atziri's Promise",
			"Atziri's Reign",
			"Atziri's Splendour",
			"Atziri's Step",
			"Augyre",
			"Aukuna's Will",
			"Aurseize",
			"Aurumvorax",
			"Auxium",
			"Axiom Perpetuum",
			"Balefire",
			"Bated Breath",
			"Beacon of Madness",
			"Belly of the Beast",
			"Belt of the Deceiver",
			"Beltimber Blade",
			"Berek's Grip",
			"Berek's Pass",
			"Bino's Kitchen Knife",
			"Bisco's Collar",
			"Bitterbind Point",
			"Bitterdream",
			"Black Sun Crest",
			"Blackgleam",
			"Blackheart",
			"Blasphemer's Grasp",
			"Blood of the Karui",
			"Blood Sacrifice",
			"Bloodboil",
			"Bloodbond",
			"Bloodgrip",
			"Bloodplay",
			"Bloodseeker",
			"Bones of Ullr",
			"Brain Rattler",
			"Bramblejack",
			"Brawn",
			"Breath of the Council",
			"Breathstealer",
			"Brightbeak",
			"Brinerot Flag",
			"Brinerot Mark",
			"Brinerot Whalers",
			"Briskwrap",
			"Brittle Barrier",
			"Broken Faith",
			"Bronn's Lithe",
			"Brute Force Solution",
			"Brutus' Lead Sprinkler",
			"Bubonic Trail",
			"Calamitous Visions",
			"Callinellus Malleus",
			"Cameria's Avarice",
			"Cameria's Maul",
			"Cane of Unravelling",
			"Carcass Jack",
			"Careful Planning",
			"Carnage Heart",
			"Cerberus Limb",
			"Chaber Cairn",
			"Chains of Command",
			"Chalice of Horrors",
			"Cheap Construction",
			"Chernobog's Pillar",
			"Cherrubim's Maleficence",
			"Chill of Corruption",
			"Chin Sol",
			"Chitus' Apex",
			"Chitus' Needle",
			"Chober Chaber",
			"Choir of the Storm",
			"Circle of Anguish",
			"Circle of Nostalgia",
			"Clayshaper",
			"Clear Mind",
			"Cloak of Defiance",
			"Cloak of Flame",
			"Cloak of Tawm'r Isley",
			"Coated Shrapnel",
			"Cold Iron Point",
			"Cold Steel",
			"Combat Focus",
			"Combustibles",
			"Command of the Pit",
			"Conqueror's Efficiency",
			"Conqueror's Longevity",
			"Conqueror's Potency",
			"Coralito's Signature",
			"Corrupted Energy",
			"Coruscating Elixir",
			"Cospri's Will",
			"Coward's Chains",
			"Cowl of the Ceraunophile",
			"Cowl of the Cryophile",
			"Cowl of the Thermophile",
			"Cragfall",
			"Craghead",
			"Craiceann's Chitin",
			"Craiceann's Pincers",
			"Craiceann's Tracks",
			"Crest of Perandus",
			"Crown of Eyes",
			"Crown of the Inward Eye",
			"Crown of the Pale King",
			"Crown of Thorns",
			"Crystal Vault",
			"Curtain Call",
			"Cybil's Paw",
			"Cyclopean Coil",
			"Dance of the Offered",
			"Daresso's Courage",
			"Daresso's Defiance",
			"Daresso's Passion",
			"Daresso's Salute",
			"Darkness Enthroned",
			"Darkray Vectors",
			"Dead Reckoning",
			"Death Rush",
			"Death's Hand",
			"Death's Harp",
			"Death's Oath",
			"Debeon's Dirge",
			"Deerstalker",
			"Deidbell",
			"Demon Stitcher",
			"Dendrobate",
			"Devoto's Devotion",
			"Dialla's Malefaction",
			"Disintegrator",
			"Divide and Conquer",
			"Divinarius",
			"Divination Distillate",
			"Doedre's Damning",
			"Doedre's Elixir",
			"Doedre's Scorn",
			"Doedre's Skin",
			"Doedre's Tenure",
			"Doomfletch",
			"Doomfletch's Prism",
			"Doomsower",
			"Doon Cuebiyari",
			"Doryani's Catalyst",
			"Doryani's Fist",
			"Doryani's Invitation",
			"Dreadarc",
			"Dreadbeak",
			"Dreadsurge",
			"Dream Fragments",
			"Dreamfeather",
			"Drillneck",
			"Duskblight",
			"Dusktoe",
			"Dyadian Dawn",
			"Dyadus",
			"Dying Breath",
			"Earendel's Embrace",
			"Eber's Unification",
			"Eclipse Solaris",
			"Edge of Madness",
			"Efficient Training",
			"Eldritch Knowledge",
			"Elegant Hubris",
			"Emberwake",
			"Empire's Grasp",
			"Energised Armour",
			"Ephemeral Edge",
			"Esh's Mirror",
			"Esh's Visage",
			"Essence Worm",
			"Essentia Sanguis",
			"Ewar's Mirage",
			"Extractor Mentis",
			"Eye of Chayula",
			"Eye of Innocence",
			"Eye of Malice",
			"Ezomyte Hold",
			"Ezomyte Peak",
			"Facebreaker",
			"Fairgraves' Tricorne",
			"Faminebind",
			"Farrul's Bite",
			"Farrul's Chase",
			"Farrul's Pounce",
			"Feastbind",
			"Femurs of the Saints",
			"Fencoil",
			"Fenumus' Shroud",
			"Fenumus' Toxins",
			"Fertile Mind",
			"Fevered Mind",
			"Fidelitas' Spike",
			"Fight for Survival",
			"Fireborn",
			"First Piece of Directions",
			"First Piece of Focus",
			"First Piece of Storms",
			"First Piece of Time",
			"First Snow",
			"Flamesight",
			"Flesh and Spirit",
			"Flesh-Eater",
			"Fluid Motion",
			"Forbidden Taste",
			"Fortress Covenant",
			"Fourth Piece of Focus",
			"Fox's Fortune",
			"Foxshade",
			"Fractal Thoughts",
			"Fragile Bloom",
			"Fragility",
			"From Dust",
			"Frostbreath",
			"Frozen Trail",
			"Galesight",
			"Gang's Momentum",
			"Geofri's Baptism",
			"Geofri's Crest",
			"Geofri's Devotion",
			"Geofri's Sanctuary",
			"Giantsbane",
			"Gifts from Above",
			"Glitterdisc",
			"Gloomfang",
			"Gluttony",
			"Goldrim",
			"Gorebreaker",
			"Goredrill",
			"Gorgon's Gaze",
			"Grand Spectrum",
			"Great Old One's Ward",
			"Greed's Embrace",
			"Growing Agony",
			"Haemophilia",
			"Hair Trigger",
			"Hale Negator",
			"Hand of Thought and Motion",
			"Hand of Wisdom and Action",
			"Hazardous Research",
			"Heartbound Loop",
			"Heartbreaker",
			"Heatshiver",
			"Hegemony's Era",
			"Heretic's Veil",
			"Hezmana's Bloodlust",
			"Hidden Potential",
			"Hiltless",
			"Hinekora's Sight",
			"Honourhome",
			"Hopeshredder",
			"Hotfooted",
			"Hrimnor's Dirge",
			"Hrimnor's Hymn",
			"Hrimnor's Resolve",
			"Hrimsorrow",
			"Hungry Abyss",
			"Hyaon's Fury",
			"Hyperboreus",
			"Hyrri's Bite",
			"Hyrri's Ire",
			"Hyrri's Truth",
			"Icefang Orbit",
			"Icetomb",
			"Ichimonji",
			"Immortal Flesh",
			"Impresence",
			"Incandescent Heart",
			"Indigon",
			"Inertia",
			"Inevitability",
			"Infernal Mantle",
			"Infractem",
			"Innsbury Edge",
			"Inpulsa's Broken Heart",
			"Invictus Solaris",
			"Inya's Epiphany",
			"Iron Commander",
			"Iron Heart",
			"Izaro's Dilemma",
			"Izaro's Turmoil",
			"Jack, the Axe",
			"Jaws of Agony",
			"Jorrhast's Blacksteel",
			"Kalisa's Grace",
			"Kaltenhalt",
			"Kaltensoul",
			"Kaom's Heart",
			"Kaom's Primacy",
			"Kaom's Roots",
			"Kaom's Sign",
			"Karui Charge",
			"Karui Ward",
			"Kiara's Determination",
			"Kikazaru",
			"Kingsguard",
			"Kintsugi",
			"Kitava's Feast",
			"Kitava's Teachings",
			"Kitava's Thirst",
			"Kondo's Pride",
			"Kongming's Stratagem",
			"Kongor's Undying Rage",
			"Lakishu's Blade",
			"Last Resort",
			"Lavianga's Spirit",
			"Lavianga's Wisdom",
			"Le Heup of All",
			"Leash of Oblation",
			"Leer Cast",
			"Leper's Alms",
			"Lifesprig",
			"Light of Lunaris",
			"Lightbane Raiment",
			"Lightning Coil",
			"Lightpoacher",
			"Limbsplit",
			"Lioneye's Glare",
			"Lioneye's Paws",
			"Lioneye's Remorse",
			"Lioneye's Vision",
			"Lion's Roar",
			"Lochtonial Caress",
			"Loreweave",
			"Lori's Lantern",
			"Lycosidae",
			"Malachai's Artifice",
			"Malachai's Awakening",
			"Malachai's Loop",
			"Malachai's Mark",
			"Malachai's Simula",
			"Malicious Intent",
			"Maligaro's Cruelty",
			"Maligaro's Lens",
			"Maligaro's Restraint",
			"Maloney's Nightfall",
			"Manastorm",
			"Mantra of Flames",
			"March of the Legion",
			"Mark of Submission",
			"Mark of the Doubting Knight",
			"Mark of the Elder",
			"Mark of the Red Covenant",
			"Mark of the Shaper",
			"Marohi Erqi",
			"Martial Artistry",
			"Martyr of Innocence",
			"Martyr's Crown",
			"Marylene's Fallacy",
			"Mask of the Tribunal",
			"Matua Tupuna",
			"Maw of Conquest",
			"Meginord's Girdle",
			"Meginord's Vise",
			"Memory Vault",
			"Midnight Bargain",
			"Might and Influence",
			"Might in All Forms",
			"Mightflay",
			"Mind of the Council",
			"Mindspiral",
			"Ming's Heart",
			"Mirebough",
			"Mistwall",
			"Mjölner",
			"Mokou's Embrace",
			"Mon'tregul's Grasp",
			"Moonbender's Wing",
			"Moonsorrow",
			"Mortem Morsu",
			"Mother's Embrace",
			"Mutated Growth",
			"Mutewind Pennant",
			"Mutewind Seal",
			"Mutewind Whispersteps",
			"Natural Affinity",
			"Nebuloch",
			"Ngamahu Tiki",
			"Ngamahu's Flame",
			"Ngamahu's Sign",
			"Nomic's Storm",
			"Null's Inclination",
			"Nuro's Harp",
			"Nycta's Lantern",
			"Obliteration",
			"Obscurantis",
			"Offering to the Serpent",
			"Omen on the Winds",
			"Ondar's Clasp",
			"Ornament of the East",
			"Oro's Sacrifice",
			"Oskarm",
			"Overwhelming Odds",
			"Pacifism",
			"Painseeker",
			"Panquetzaliztli",
			"Perandus Blazon",
			"Perandus Signet",
			"Perepiteia",
			"Perfidy",
			"Perquil's Toe",
			"Perseverance",
			"Pillar of the Caged God",
			"Piscator's Vigil",
			"Pledge of Hands",
			"Poacher's Aim",
			"Powerlessness",
			"Praxis",
			"Primordial Eminence",
			"Primordial Harmony",
			"Primordial Might",
			"Prismatic Eclipse",
			"Prismweave",
			"Profane Proxy",
			"Pugilist",
			"Pure Talent",
			"Putembo's Valley",
			"Pyre",
			"Quecholli",
			"Queen of the Forest",
			"Queen's Decree",
			"Quickening Covenant",
			"Quill Rain",
			"Rain of Splinters",
			"Rainbowstride",
			"Ralakesh's Impatience",
			"Rapid Expansion",
			"Rashkaldor's Patience",
			"Rathpith Globe",
			"Rat's Nest",
			"Razor of the Seventh Sun",
			"Reach of the Council",
			"Realm Ender",
			"Realmshaper",
			"Reaper's Pursuit",
			"Rearguard",
			"Reckless Defence",
			"Redbeak",
			"Redblade Band",
			"Redblade Banner",
			"Redblade Tramplers",
			"Relentless Fury",
			"Repentance",
			"Reverberation Rod",
			"Rigwald's Charge",
			"Rigwald's Command",
			"Rigwald's Crest",
			"Rigwald's Savagery",
			"Rime Gaze",
			"Ring of Blades",
			"Rise of the Phoenix",
			"Rive",
			"Rolling Flames",
			"Romira's Banquet",
			"Rotgut",
			"Roth's Reach",
			"Rotting Legion",
			"Rumi's Concoction",
			"Sacrificial Harvest",
			"Sacrificial Heart",
			"Sadima's Touch",
			"Saemus' Gift",
			"Saffell's Frame",
			"Sanguine Gambol",
			"Saqawal's Flock",
			"Saqawal's Talons",
			"Saqawal's Winds",
			"Scaeva",
			"Scold's Bridle",
			"Second Piece of Directions",
			"Second Piece of Focus",
			"Second Piece of Storms",
			"Second Piece of Time",
			"Self-Flagellation",
			"Sentari's Answer",
			"Severed in Sleep",
			"Shackles of the Wretched",
			"Shaper's Seed",
			"Shaper's Touch",
			"Shattered Chains",
			"Shavronne's Gambit",
			"Shavronne's Pace",
			"Shavronne's Revelation",
			"Shimmeron",
			"Shiversting",
			"Shroud of the Lightless",
			"Sibyl's Lament",
			"Sidhebreath",
			"Siegebreaker",
			"Sign of the Sin Eater",
			"Silverbough",
			"Silverbranch",
			"Sin Trek",
			"Singularity",
			"Sinvicta's Mettle",
			"Sire of Shards",
			"Skirmish",
			"Skullhead",
			"Slitherpinch",
			"Slivertongue",
			"Snakebite",
			"Snakepit",
			"Solaris Lorica",
			"Soul Catcher",
			"Soul Mantle",
			"Soul Strike",
			"Soul Tether",
			"Soul's Wick",
			"Soulthirst",
			"Soulwrest",
			"Southbound",
			"Speaker's Wreath",
			"Spine of the First Claimant",
			"Spire of Stone",
			"Spirit Guards",
			"Spirited Response",
			"Split Personality",
			"Spreading Rot",
			"Springleaf",
			"Starkonja's Head",
			"Static Electricity",
			"Steel Spirit",
			"Steppan Eard",
			"Stone of Lazhwar",
			"Storm Cloud",
			"Storm Prison",
			"Stormcharger",
			"Stormfire",
			"Storm's Gift",
			"Story of the Vaal",
			"Sunblast",
			"Sundance",
			"Sunspite",
			"Surgebinders",
			"Survival Instincts",
			"Survival Secrets",
			"Survival Skills",
			"Taproot",
			"Taryn's Shiver",
			"Tasalio's Sign",
			"Taste of Hate",
			"Tavukai",
			"Tear of Purity",
			"Tempered Flesh",
			"Tempered Mind",
			"Tempered Spirit",
			"Terminus Est",
			"The Anvil",
			"The Aylardex",
			"The Baron",
			"The Beast Fur Shawl",
			"The Black Cane",
			"The Blood Dance",
			"The Blood Reaper",
			"The Blood Thorn",
			"The Blue Dream",
			"The Brine Crown",
			"The Bringer of Rain",
			"The Broken Crown",
			"The Cauteriser",
			"The Coming Calamity",
			"The Consuming Dark",
			"The Covenant",
			"The Crimson Storm",
			"The Dancing Dervish",
			"The Dark Seer",
			"The Deep One's Hide",
			"The Embalmer",
			"The Enmity Divine",
			"The Eternal Apple",
			"The Flow Untethered",
			"The Formless Flame",
			"The Formless Inferno",
			"The Fracturing Spinner",
			"The Front Line",
			"The Goddess Bound",
			"The Golden Rule",
			"The Grey Spire",
			"The Gull",
			"The Harvest",
			"The Hungry Loop",
			"The Ignomon",
			"The Infinite Pursuit",
			"The Interrogation",
			"The Ivory Tower",
			"The Jinxed Juju",
			"The Long Winter",
			"The Magnate",
			"The Oak",
			"The Overflowing Chalice",
			"The Peregrine",
			"The Perfect Form",
			"The Poet's Pen",
			"The Princess",
			"The Queen's Hunger",
			"The Rat Cage",
			"The Red Trail",
			"The Restless Ward",
			"The Rippling Thoughts",
			"The Scourge",
			"The Screaming Eagle",
			"The Searing Touch",
			"The Siege",
			"The Snowblind Grace",
			"The Stormheart",
			"The Stormwall",
			"The Supreme Truth",
			"The Tempest's Binding",
			"The Tempestuous Steel",
			"The Three Dragons",
			"The Vertex",
			"The Vigil",
			"The Warden's Brand",
			"The Wasp Nest",
			"The Whispering Ice",
			"The Wise Oak",
			"The Writhing Jar",
			"Thief's Torment",
			"Third Piece of Directions",
			"Third Piece of Focus",
			"Third Piece of Storms",
			"Thirst for Horrors",
			"Thousand Ribbons",
			"Thousand Teeth Temu",
			"Thread of Hope",
			"Three-step Assault",
			"Thunderfist",
			"Thundersight",
			"Tidebreaker",
			"Timeclasp",
			"Timetwist",
			"Tinkerskin",
			"Titucius' Span",
			"To Dust",
			"Tombfist",
			"Torchoak Step",
			"Touch of Anguish",
			"Tremor Rod",
			"Triad Grip",
			"Trolltimber Spire",
			"Trypanon",
			"Tukohama's Fortress",
			"Tulborn",
			"Tulfall",
			"Twyzel",
			"Umbilicus Immortalis",
			"Ungil's Gauche",
			"Ungil's Harmony",
			"United in Dream",
			"Unstable Payload",
			"Unyielding Flame",
			"Uul-Netol's Embrace",
			"Uul-Netol's Kiss",
			"Uzaza's Meadow",
			"Vaal Caress",
			"Vaal Sentencing",
			"Valako's Sign",
			"Valyrium",
			"Varunastra",
			"Veil of the Night",
			"Venopuncture",
			"Veruso's Battering Rams",
			"Vessel of Vinktar",
			"Victario's Acuity",
			"Victario's Charity",
			"Victario's Flight",
			"Victario's Influence",
			"Viper's Scales",
			"Vis Mortis",
			"Vivinsect",
			"Vix Lunaris",
			"Vixen's Entrapment",
			"Voice of the Storm",
			"Voidbringer",
			"Voideye",
			"Voidforge",
			"Voidheart",
			"Voidhome",
			"Voidwalker",
			"Volkuur's Guidance",
			"Volley Fire",
			"Voll's Devotion",
			"Voll's Protector",
			"Voll's Vision",
			"Voltaxic Rift",
			"Vulconus",
			"Wake of Destruction",
			"Wall of Brambles",
			"Wanderlust",
			"Warlord's Reach",
			"Warped Timepiece",
			"Warrior's Legacy",
			"Weight of Sin",
			"Weight of the Empire",
			"Wheel of the Stormsail",
			"White Wind",
			"Wideswing",
			"Widowmaker",
			"Wildfire",
			"Wildslash",
			"Willowgift",
			"Winds of Change",
			"Windscream",
			"Wings of Entropy",
			"Winter Burial",
			"Winterheart",
			"Winter's Bounty",
			"Winterweave",
			"Witchfire Brew",
			"Wondertrap",
			"Worldcarver",
			"Wraithlord",
			"Wreath of Phrecia",
			"Wurm's Molt",
			"Wyrmsign",
			"Xirgil's Crank",
			"Xoph's Heart",
			"Xoph's Inception",
			"Xoph's Nurture",
			"Ylfeban's Trickery",
			"Yoke of Suffering",
			"Yriel's Fostering",
			"Zahndethus' Cassock",
			"Zeel's Amplifier",
			"Zerphi's Last Breath",
        };

        public override bool Initialise()
        {
            base.Initialise();

            ParseConfig_BaseType();

            Name = "INV Item Analyzer";

            var combine = Path.Combine(DirectoryFullName, "img", "GoodItem.png").Replace('\\', '/');
            Graphics.InitImage(combine, false);

            combine = Path.Combine(DirectoryFullName, "img", "Syndicate.png").Replace('\\', '/');
            Graphics.InitImage(combine, false);

            _ingameState = GameController.Game.IngameState;
            _windowOffset = GameController.Window.GetWindowRectangle().TopLeft;

            Input.RegisterKey(Settings.HotKey.Value);
            Input.RegisterKey(Keys.LControlKey);

            Settings.HotKey.OnValueChanged += () => { Input.RegisterKey(Settings.HotKey.Value); };

            return true;
        }

        public override void Render()
        {
            if (!_ingameState.IngameUi.InventoryPanel.IsVisible)
            {
                CountInventory = 0;
                idenf = 0;
                return;
            }

            var normalInventoryItems = _ingameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory]
                .VisibleInventoryItems;

            var temp = normalInventoryItems.Count(t => t.Item?.GetComponent<Mods>()?.Identified == true);

            //LogMessage(normalInventoryItems.Count.ToString() + " " + CountInventory.ToString() + " // " + temp .ToString() + " " + idenf.ToString(), 3f);

            if (normalInventoryItems.Count != CountInventory || temp != idenf)
            {
                ScanInventory(normalInventoryItems);
                CountInventory = normalInventoryItems.Count;
                idenf = temp;
            }

            if (!Settings.HideUnderMouse)
            {
                //DrawSyndicateItems(_VeilItemsPos);
                DrawGoodItems(_goodItemsPos);
                //DrawHighItemLevel(_highItemsPos);
                if (Settings.HotKey.PressedOnce())
                {
                    CoroutineWorker = new Coroutine(ClickShit(), this, coroutineName);
                    Core.ParallelRunner.Run(CoroutineWorker);
                }
            }
        }

        #region Scan Inventory

        private void ScanInventory(IList<NormalInventoryItem> normalInventoryItems)
        {
            _goodItemsPos = new List<RectangleF>();
            _allItemsPos = new List<RectangleF>();
            _highItemsPos = new List<RectangleF>();
            _VeilItemsPos = new List<RectangleF>();

            foreach (var normalInventoryItem in normalInventoryItems)
            {
	            try
	            {
                var highItemLevel = false;
                var item = normalInventoryItem.Item;
                if (item == null ||
                    item.IsValid == false)
                    continue;

                var modsComponent = item?.GetComponent<Mods>();

                if (string.IsNullOrEmpty(item.Path))
                    continue;

                var drawRect = normalInventoryItem.GetClientRect();
                //fix star position
                drawRect.X -= 5;
                drawRect.Y -= 5;

                #region Filter trash uniques

                if (modsComponent?.ItemRarity == ItemRarity.Unique &&
                	!item.HasComponent<Map>() &&
                    item?.GetComponent<Sockets>()?.LargestLinkSize != 6 &&
                    ShitUniquesList.Contains(modsComponent.UniqueName))
                {
                    LogMessage("Garbage detected " + modsComponent.UniqueName);
                    _allItemsPos.Add(drawRect);

                    continue;
                }

                #endregion

                if (modsComponent?.ItemRarity == ItemRarity.Normal ||
                    modsComponent?.ItemRarity == ItemRarity.Magic)
                {
                    if (item?.GetComponent<Sockets>()?.NumberOfSockets == 6)
                        _allItemsPos.Add(drawRect);

                    continue;
                }

                if (modsComponent?.ItemRarity != ItemRarity.Rare ||
                    modsComponent.Identified == false)
                    continue;

                var itemMods = modsComponent.ItemMods;

                //foreach (ItemMod im in itemMods) if (!GameController.Files.Mods.records.ContainsKey(im.RawName)) return;

                var mods =
                    itemMods.Select(
                        it => new ModValue(it, GameController.Files, modsComponent.ItemLevel,
                            GameController.Files.BaseItemTypes.Translate(item.Path))
                    ).ToList();

                #region Elder or Shaper

                {
                    var baseComponent = item?.GetComponent<Base>();
                    if (modsComponent.ItemLevel >= Settings.ItemLevel_ElderOrShaper &&
                        (baseComponent.isElder || baseComponent.isShaper || baseComponent.isCrusader ||
                         baseComponent.isHunter || baseComponent.isRedeemer || baseComponent.isSynthesized ||
                         baseComponent.isWarlord))
                        highItemLevel = true;
                }

                #endregion

                var bit = GameController.Files.BaseItemTypes.Translate(item.Path);

                #region Item Level

                {
                    if (modsComponent.ItemLevel >= Settings.ItemLevel_BaseType)
                        foreach (var BaseType in GoodBaseTypes)
                            if (bit.BaseName == BaseType)
                            {
                                highItemLevel = true;
                                break;
                            }
                }

                #endregion

                int count;

                switch (bit?.ClassName)
                {
                    case "Body Armour":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeBodyArmour(mods);
                        if (count >= Settings.BaAffixes && Settings.BodyArmour)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }

                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Quiver":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeQuiver(mods);
                        if (count >= Settings.QAffixes && Settings.Quiver)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Helmet":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeHelmet(mods);
                        if (count >= Settings.HAffixes && Settings.Helmet)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Boots":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeBoots(mods);
                        if (count >= Settings.BAffixes && Settings.Boots)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Gloves":

                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeGloves(mods);
                        if (count >= Settings.GAffixes && Settings.Gloves)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;


                    case "Shield":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeShield(mods);
                        if (count >= Settings.SAffixes && Settings.Shield)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Belt":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeBelt(mods);
                        if (count >= Settings.BeAffixes && Settings.Belt)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Ring":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeRing(mods);
                        if (count >= Settings.RAffixes && Settings.Ring)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Amulet":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeAmulet(mods);
                        if (count >= Settings.AAffixes && Settings.Amulet)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Dagger":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Rune Dagger":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Wand":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Sceptre":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Thrusting One Hand Sword":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Staff":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Warstaff":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;

                    case "Claw":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;
                    case "One Hand Sword":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;
                    case "Two Hand Sword":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;
                    case "One Hand Axe":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;
                    case "Two Hand Axe":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;
                    case "One Hand Mace":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;
                    case "Two Hand Mace":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;
                    case "Bow":
                        if (highItemLevel) _highItemsPos.Add(drawRect);

                        count = AnalyzeWeaponCaster(mods);
                        if (count >= Settings.WcAffixes && Settings.WeaponCaster)
                        {
                            if (count >= 100)
                                _VeilItemsPos.Add(drawRect);
                            else
                                _goodItemsPos.Add(drawRect);
                        }
                        else if (AnalyzeWeaponAttack(item) && Settings.WeaponAttack)
                        {
                            _goodItemsPos.Add(drawRect);
                        }
                        else
                        {
                            _allItemsPos.Add(drawRect);
                        }

                        break;
                }
	            }
	            catch (Exception e)
	            {
					continue;
	            }
            }
        }

        #endregion

        #region DrawHighItemLevel

        private void DrawHighItemLevel(List<RectangleF> HighItemLevel)
        {
            foreach (var position in HighItemLevel)
                if (Settings.StarOrBorder)
                {
                    var border = new RectangleF
                    {
                        X = position.X + 8, Y = position.Y + 8, Width = position.Width - 6, Height = position.Height - 6
                    };
                    Graphics.DrawFrame(border, Settings.ColorAll, 1);
                }
        }

        #endregion

        #region Draw GoodItems

        private void DrawGoodItems(List<RectangleF> goodItems)
        {
            foreach (var position in goodItems)
                if (Settings.StarOrBorder)
                {
                    var border = new RectangleF
                    {
                        X = position.X + 8, Y = position.Y + 8, Width = (position.Width - 6) / 1.5f,
                        Height = (position.Height - 6) / 1.5f
                    };

                    if (!Settings.Text)
                        Graphics.DrawImage("GoodItem.png", border);
                    else
                        Graphics.DrawText(@" Good Item ", position.TopLeft, Settings.Color, 30);
                }
        }

        #endregion

        #region Draw Syndicate items

        private void DrawSyndicateItems(List<RectangleF> SyndicateItems)
        {
            foreach (var position in SyndicateItems)
                if (Settings.StarOrBorder)
                {
                    var border = new RectangleF
                    {
                        X = position.X + 8, Y = position.Y + 8, Width = (position.Width - 6) / 1.5f,
                        Height = (position.Height - 6) / 1.5f
                    };

                    if (!Settings.Text)
                        Graphics.DrawImage("Syndicate.png", border);
                    else
                        Graphics.DrawText(@" Syndicate ", position.TopLeft, Settings.Color, 30);
                }
        }

        #endregion

        #region ClickShit

        private IEnumerator ClickShit()
        {
            Input.KeyDown(Keys.LControlKey);
            foreach (var position in _allItemsPos)
            {
                var vector2 = new Vector2(position.X + 25, position.Y + 25);

                Input.SetCursorPos(vector2 + _windowOffset);

                yield return new WaitTime(Settings.ExtraDelay.Value / 2);

                Input.Click(MouseButtons.Left);

                yield return new WaitTime(Settings.ExtraDelay.Value);
            }

            Input.KeyUp(Keys.LControlKey);
        }

        #endregion

        #region Body Armour

        private int AnalyzeBodyArmour(List<ModValue> mods)
        {
            var BaaffixCounter = 0;
            var elemRes = 0;

            foreach (var mod in mods)
            {
                if (mod.Record.Group == "IncreasedLife" && mod.StatValue[0] >= Settings.BaLife)
                    BaaffixCounter++;

                else if ((mod.Record.Group.Contains("DefencesPercent") ||
                          mod.Record.Group.Contains("BaseLocalDefences")) && mod.Tier <= Settings.BaEnergyShield &&
                         mod.Tier > 0)
                    BaaffixCounter++;

                else if (mod.Record.Group.Contains("Resist"))
                    if (mod.Record.Group == "AllResistances")
                        elemRes += mod.StatValue[0] * 3;
                    else if (mod.Record.Group.Contains("And"))
                        elemRes += mod.StatValue[0] * 2;
                    else
                        elemRes += mod.StatValue[0];

                else if (mod.Record.Group == "Strength" && mod.StatValue[0] >= Settings.BaStrength)
                    BaaffixCounter++;

                else if (mod.Record.Group == "Intelligence" && mod.StatValue[0] >= Settings.BaIntelligence)
                    BaaffixCounter++;

                else if (mod.Record.Group == "Dexterity" && mod.StatValue[0] >= Settings.BaDexterity)
                    BaaffixCounter++;

                else if (mod.Record.Group.Contains("BaseLocalDefencesAndLife") && mod.Tier <= Settings.BaLifeCombo &&
                         mod.Tier > 0)
                    BaaffixCounter++;

                else if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                    BaaffixCounter += 100;

                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false) LogMessage(mod.Record.Group, 10f);
                }
            }

            if (elemRes >= Settings.BaTotalRes)
                BaaffixCounter++;

            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + BaaffixCounter, 10f);
            }
            return BaaffixCounter;
        }

        #endregion

        #region Helmets

        private int AnalyzeHelmet(List<ModValue> mods)
        {
            var HaffixCounter = 0;
            var elemRes = 0;

            foreach (var mod in mods)
            {
                if (mod.Record.Group == "IncreasedLife" && mod.StatValue[0] >= Settings.HLife)
                    HaffixCounter++;

                else if ((mod.Record.Group.Contains("DefencesPercent") ||
                          mod.Record.Group.Contains("BaseLocalDefences")) && mod.Tier <= Settings.HEnergyShield &&
                         mod.Tier > 0)
                    HaffixCounter++;

                else if (mod.Record.Group.Contains("Resist"))
                    if (mod.Record.Group == "AllResistances")
                        elemRes += mod.StatValue[0] * 3;
                    else if (mod.Record.Group.Contains("And"))
                        elemRes += mod.StatValue[0] * 2;
                    else
                        elemRes += mod.StatValue[0];

                else if (mod.Record.Group == "IncreasedAccuracy" && mod.StatValue[0] >= Settings.HAccuracy)
                    HaffixCounter++;

                else if (mod.Record.Group == "Strength" && mod.StatValue[0] >= Settings.HStrength)
                    HaffixCounter++;

                else if (mod.Record.Group == "Intelligence" && mod.StatValue[0] >= Settings.HIntelligence)
                    HaffixCounter++;

                else if (mod.Record.Group == "Dexterity" && mod.StatValue[0] >= Settings.HDexterity)
                    HaffixCounter++;

                else if (mod.Record.Group == "IncreasedMana" && mod.StatValue[0] >= Settings.HMana)
                    HaffixCounter++;

                else if (mod.Record.Group.Contains("BaseLocalDefencesAndLife") && mod.Tier <= Settings.HLifeCombo &&
                         mod.Tier > 0)
                    HaffixCounter++;

                else if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                    HaffixCounter += 100;

                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false)
                        LogMessage(mod.Record.Group, 10f);
                }
            }

            if (elemRes >= Settings.HTotalRes)
                HaffixCounter++;
            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + HaffixCounter, 10f);
            }
            return HaffixCounter;
        }

        #endregion

        #region Gloves

        private int AnalyzeGloves(List<ModValue> mods)
        {
            var GaffixCounter = 0;
            var elemRes = 0;

            foreach (var mod in mods)
            {
                if (mod.Record.Group == "IncreasedLife" && mod.StatValue[0] >= Settings.GLife)
                    GaffixCounter++;

                else if ((mod.Record.Group.Contains("DefencesPercent") ||
                          mod.Record.Group.Contains("BaseLocalDefences")) && mod.Tier <= Settings.GEnergyShield &&
                         mod.Tier > 0)
                    GaffixCounter++;

                else if (mod.Record.Group.Contains("Resist"))
                    if (mod.Record.Group == "AllResistances")
                        elemRes += mod.StatValue[0] * 3;
                    else if (mod.Record.Group.Contains("And"))
                        elemRes += mod.StatValue[0] * 2;
                    else
                        elemRes += mod.StatValue[0];

                else if (mod.Record.Group == "IncreasedAccuracy" && mod.StatValue[0] >= Settings.GAccuracy)
                    GaffixCounter++;

                else if (mod.Record.Group == "IncreasedAttackSpeed" && mod.StatValue[0] >= Settings.GAttackSpeed)
                    GaffixCounter++;

                else if (mod.Record.Group == "PhysicalDamage" && Average(mod.StatValue) >= Settings.GPhysDamage)
                    GaffixCounter++;

                else if (mod.Record.Group == "Strength" && mod.StatValue[0] >= Settings.GStrength)
                    GaffixCounter++;

                else if (mod.Record.Group == "Intelligence" && mod.StatValue[0] >= Settings.GIntelligence)
                    GaffixCounter++;

                else if (mod.Record.Group == "Dexterity" && mod.StatValue[0] >= Settings.GDexterity)
                    GaffixCounter++;

                else if (mod.Record.Group == "IncreasedMana" && mod.StatValue[0] >= Settings.GMana)
                    GaffixCounter++;

                else if (mod.Record.Group.Contains("BaseLocalDefencesAndLife") && mod.Tier <= Settings.GLifeCombo &&
                         mod.Tier > 0)
                    GaffixCounter++;

                else if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                    GaffixCounter += 100;

                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false)
                        LogMessage(mod.Record.Group, 10f);
                }
            }

            if (elemRes >= Settings.GTotalRes)
                GaffixCounter++;
            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + GaffixCounter, 10f);
            }
            return GaffixCounter;
        }

        #endregion

        #region Boots

        private int AnalyzeBoots(List<ModValue> mods)
        {
            var BaffixCounter = 0;
            var elemRes = 0;

            foreach (var mod in mods)
            {
                if (mod.Record.Group == "MovementVelocity" && mod.StatValue[0] >= Settings.BMoveSpeed)
                    BaffixCounter++;

                else if (mod.Record.Group == "IncreasedLife" && mod.StatValue[0] >= Settings.BLife)
                    BaffixCounter++;

                else if ((mod.Record.Group.Contains("DefencesPercent") ||
                          mod.Record.Group.Contains("BaseLocalDefences")) && mod.Tier <= Settings.BEnergyShield &&
                         mod.Tier > 0)
                    BaffixCounter++;

                else if (mod.Record.Group.Contains("Resist"))
                    if (mod.Record.Group == "AllResistances")
                        elemRes += mod.StatValue[0] * 3;
                    else if (mod.Record.Group.Contains("And"))
                        elemRes += mod.StatValue[0] * 2;
                    else
                        elemRes += mod.StatValue[0];

                else if (mod.Record.Group == "Strength" && mod.StatValue[0] >= Settings.BStrength)
                    BaffixCounter++;

                else if (mod.Record.Group == "Intelligence" && mod.StatValue[0] >= Settings.BIntelligence)
                    BaffixCounter++;

                else if (mod.Record.Group == "Dexterity" && mod.StatValue[0] >= Settings.BDexterity)
                    BaffixCounter++;

                else if (mod.Record.Group == "IncreasedMana" && mod.StatValue[0] >= Settings.BMana)
                    BaffixCounter++;

                else if (mod.Record.Group.Contains("BaseLocalDefencesAndLife") && mod.Tier <= Settings.BLifeCombo &&
                         mod.Tier > 0)
                    BaffixCounter++;

                else if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                    BaffixCounter += 100;

                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false)
                        LogMessage(mod.Record.Group, 10f);
                }
            }

            if (elemRes >= Settings.BTotalRes)
                BaffixCounter++;
            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + BaffixCounter, 10f);
            }
            return BaffixCounter;
        }

        #endregion

        #region Belts

        private int AnalyzeBelt(List<ModValue> mods)
        {
            var BeaffixCounter = 0;
            var elemRes = 0;

            foreach (var mod in SumAffix(mods))
            {
                if (mod.Record.Group == "IncreasedLife" && mod.StatValue[0] >= Settings.BeLife)
                    BeaffixCounter++;

                else if (mod.Record.Group.Contains("EnergyShield") && mod.Tier <= Settings.BeEnergyShield &&
                         mod.Tier > 0)
                    BeaffixCounter++;

                else if (mod.Record.Group.Contains("Resist"))
                    if (mod.Record.Group == "AllResistances")
                        elemRes += mod.StatValue[0] * 3;
                    else if (mod.Record.Group.Contains("And"))
                        elemRes += mod.StatValue[0] * 2;
                    else
                        elemRes += mod.StatValue[0];

                else if (mod.Record.Group == "Strength" && mod.StatValue[0] >= Settings.BeStrength)
                    BeaffixCounter++;

                else if (mod.Record.Group == "IncreasedWeaponElementalDamagePercent" &&
                         mod.StatValue[0] >= Settings.BeWeaponElemDamage)
                    BeaffixCounter++;

                else if (mod.Record.Group == "BeltFlaskCharges" && mod.StatValue[0] >= Settings.BeFlaskReduced)
                    BeaffixCounter++;

                else if (mod.Record.Group == "BeltFlaskDuration" && mod.StatValue[0] >= Settings.BeFlaskDuration)
                    BeaffixCounter++;

                else if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                    BeaffixCounter += 100;


                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false)
                        LogMessage(mod.Record.Group, 10f);
                }
            }

            if (elemRes >= Settings.BeTotalRes)
                BeaffixCounter++;
            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + BeaffixCounter, 10f);
            }
            return BeaffixCounter;
        }

        #endregion

        #region Rings

        private int AnalyzeRing(List<ModValue> mods)
        {
            var RaffixCounter = 0;
            var elemRes = 0;

            foreach (var mod in SumAffix(mods))
            {
                if (mod.Record.Group == "IncreasedLife" && mod.StatValue[0] >= Settings.RLife)
                    RaffixCounter++;

                else if (mod.Record.Group.Contains("EnergyShield") && mod.Tier <= Settings.REnergyShield &&
                         mod.Tier > 0)
                    RaffixCounter++;

                else if (mod.Record.Group.Contains("Resist"))
                    if (mod.Record.Group == "AllResistances")
                        elemRes += mod.StatValue[0] * 3;
                    else if (mod.Record.Group.Contains("And"))
                        elemRes += mod.StatValue[0] * 2;
                    else
                        elemRes += mod.StatValue[0];

                else if (mod.Record.Group == "IncreasedAttackSpeed" && mod.StatValue[0] >= Settings.RAttackSpeed)
                    RaffixCounter++;

                else if (mod.Record.Group == "IncreasedCastSpeed" && mod.StatValue[0] >= Settings.RCastSpped)
                    RaffixCounter++;

                else if (mod.Record.Group == "IncreasedAccuracy" && mod.StatValue[0] >= Settings.RAccuracy)
                    RaffixCounter++;

                else if (mod.Record.Group == "PhysicalDamage" && Average(mod.StatValue) >= Settings.RPhysDamage)
                    RaffixCounter++;

                else if (mod.Record.Group == "IncreasedWeaponElementalDamagePercent" &&
                         mod.StatValue[0] >= Settings.RWeaponElemDamage)
                    RaffixCounter++;

                else if (mod.Record.Group == "Strength" && mod.StatValue[0] >= Settings.RStrength)
                    RaffixCounter++;

                else if (mod.Record.Group == "Intelligence" && mod.StatValue[0] >= Settings.RIntelligence)
                    RaffixCounter++;

                else if (mod.Record.Group == "Dexterity" && mod.StatValue[0] >= Settings.RDexterity)
                    RaffixCounter++;

                else if (mod.Record.Group == "IncreasedMana" && mod.StatValue[0] >= Settings.RMana)
                    RaffixCounter++;

                else if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                    RaffixCounter += 100;

                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false)
                        LogMessage(mod.Record.Group, 10f);
                }
            }

            if (elemRes >= Settings.RTotalRes)
                RaffixCounter++;

            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + RaffixCounter, 10f);
            }

            return RaffixCounter;
        }

        #endregion

        #region Amulet

        private int AnalyzeAmulet(List<ModValue> mods)
        {
            var AaffixCounter = 0;
            var elemRes = 0;

            foreach (var mod in SumAffix(mods))
            {
                if (mod.Record.Group == "IncreasedLife" && mod.StatValue[0] >= Settings.ALife)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group.Contains("EnergyShield"))
                {
                    var tier = mod.Tier > 0 ? mod.Tier : FixTierEs(mod.Record.Key);
                    if (tier <= Settings.AEnergyShield)
                        AaffixCounter++;
                }

                else if (mod.Record.Group.Contains("Resist"))
                {
                    if (mod.Record.Group == "AllResistances")
                        elemRes += mod.StatValue[0] * 3;
                    else if (mod.Record.Group.Contains("And"))
                        elemRes += mod.StatValue[0] * 2;
                    else
                        elemRes += mod.StatValue[0];
                }

                else if (mod.Record.Group == "IncreasedAccuracy" && mod.StatValue[0] >= Settings.AAccuracy)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group == "PhysicalDamage" && Average(mod.StatValue) >= Settings.APhysDamage)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group == "IncreasedWeaponElementalDamagePercent" &&
                         mod.StatValue[0] >= Settings.AWeaponElemDamage)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group == "CriticalStrikeMultiplier" && mod.StatValue[0] >= Settings.ACritMult)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group == "CriticalStrikeChanceIncrease" && mod.StatValue[0] >= Settings.ACritChance)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group == "SpellDamage" && mod.StatValue[0] >= Settings.ATotalElemSpellDmg)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group == "Strength" && mod.StatValue[0] >= Settings.AStrength)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group == "Intelligence" && mod.StatValue[0] >= Settings.AIntelligence)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group == "Dexterity" && mod.StatValue[0] >= Settings.ADexterity)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group == "IncreasedMana" && mod.StatValue[0] >= Settings.AMana)
                {
                    AaffixCounter++;
                }

                else if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                {
                    AaffixCounter += 100;
                }

                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false)
                        LogMessage(mod.Record.Group, 10f);
                }
            }

            if (elemRes >= Settings.ATotalRes)
                AaffixCounter++;

            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + AaffixCounter, 10f);
            }
            return AaffixCounter;
        }

        #endregion

        #region Quiver

        private int AnalyzeQuiver(List<ModValue> mods)
        {
            var QaffixCounter = 0;
            var elemRes = 0;

            foreach (var mod in SumAffix(mods))
            {
                if (mod.Record.Group == "IncreasedLife" && mod.StatValue[0] >= Settings.QLife)
                    QaffixCounter++;

                else if (mod.Record.Group.Contains("Resist"))
                    if (mod.Record.Group == "AllResistances")
                        elemRes += mod.StatValue[0] * 3;
                    else if (mod.Record.Group.Contains("And"))
                        elemRes += mod.StatValue[0] * 2;
                    else
                        elemRes += mod.StatValue[0];

                else if (mod.Record.Group == "IncreasedAccuracy" && mod.StatValue[0] >= Settings.QAccuracy)
                    QaffixCounter++;

                else if (mod.Record.Group == "PhysicalDamage" && Average(mod.StatValue) >= Settings.QPhysDamage)
                    QaffixCounter++;

                else if (mod.Record.Group == "IncreasedWeaponElementalDamagePercent" &&
                         mod.StatValue[0] >= Settings.QWeaponElemDamage)
                    QaffixCounter++;

                else if (mod.Record.Group == "CriticalStrikeMultiplier" && mod.StatValue[0] >= Settings.QCritMult)
                    QaffixCounter++;

                else if (mod.Record.Group == "CriticalStrikeChanceIncrease" && mod.StatValue[0] >= Settings.QCritChance)
                    QaffixCounter++;

                else if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                    QaffixCounter += 100;
                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false)
                        LogMessage(mod.Record.Group, 10f);
                }
            }

            if (elemRes >= Settings.ATotalRes)
                QaffixCounter++;

            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + QaffixCounter, 10f);
            }
            return QaffixCounter;
        }

        #endregion

        #region Shields

        private int AnalyzeShield(List<ModValue> mods)
        {
            var SaffixCounter = 0;
            var elemRes = 0;

            foreach (var mod in mods)
            {
                if (mod.Record.Group == "IncreasedLife" && mod.StatValue[0] >= Settings.SLife)
                    SaffixCounter++;

                else if ((mod.Record.Group.Contains("DefencesPercent") ||
                          mod.Record.Group.Contains("BaseLocalDefences")) && mod.Tier <= Settings.SEnergyShield &&
                         mod.Tier > 0)
                    SaffixCounter++;

                else if (mod.Record.Group.Contains("Resist"))
                    if (mod.Record.Group == "AllResistances")
                        elemRes += mod.StatValue[0] * 3;
                    else if (mod.Record.Group.Contains("And"))
                        elemRes += mod.StatValue[0] * 2;
                    else
                        elemRes += mod.StatValue[0];

                else if (mod.Record.Group == "Strength" && mod.StatValue[0] >= Settings.SStrength)
                    SaffixCounter++;

                else if (mod.Record.Group == "Intelligence" && mod.StatValue[0] >= Settings.SIntelligence)
                    SaffixCounter++;

                else if (mod.Record.Group == "Dexterity" && mod.StatValue[0] >= Settings.SDexterity)
                    SaffixCounter++;

                else if (mod.Record.Group == "SpellDamage" && mod.StatValue[0] >= Settings.SSpellDamage)
                    SaffixCounter++;

                else if (mod.Record.Group == "SpellCriticalStrikeChanceIncrease" &&
                         mod.StatValue[0] >= Settings.SSpellCritChance)
                    SaffixCounter++;

                else if (mod.Record.Group.Contains("BaseLocalDefencesAndLife") && mod.Tier <= Settings.SLifeCombo &&
                         mod.Tier > 0)
                    SaffixCounter++;

                else if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                    SaffixCounter += 100;
                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false)
                        LogMessage(mod.Record.Group, 10f);
                }
            }

            if (elemRes >= Settings.STotalRes)
                SaffixCounter++;
            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + SaffixCounter, 10f);
            }
            return SaffixCounter;
        }

        #endregion

        #region Weapon Caster

        private int AnalyzeWeaponCaster(List<ModValue> mods)
        {
            var WcaffixCounter = 0;
            var totalSpellDamage = 0;
            var addElemDamage = 0;


            foreach (var mod in SumAffix(mods))
            {
                if (mod.Record.Group == "SpellCriticalStrikeChanceIncrease" &&
                    mod.StatValue[0] >= Settings.WcSpellCritChance)
                    WcaffixCounter++;

                else if (mod.Record.Group.Contains("SpellDamage"))
                    totalSpellDamage += mod.StatValue[0];

                else if (_incElemDmg.Contains(mod.Record.Group))
                    totalSpellDamage += mod.StatValue[0];

                else if (mod.Record.Group.Contains("SpellAddedElementalDamage"))
                    addElemDamage += Average(mod.StatValue);

                else if (mod.Record.Group == "SpellCriticalStrikeMultiplier" && mod.StatValue[0] >= Settings.WcCritMult)
                    WcaffixCounter++;

                else if (mod.Record.Group == "DamageOverTimeMultiplier")
                    WcaffixCounter += 3;

                if (mod.Record.Group.Contains("VeiledSuffix") || mod.Record.Group.Contains("VeiledPrefix"))
                    WcaffixCounter += 100;
                //DEBUG TEST BLOCK
                {
                    if (Settings.DebugMode != false)
                        LogMessage(mod.Record.Group, 10f);
                }
            }

            if (totalSpellDamage >= Settings.WcTotalElemSpellDmg)
                WcaffixCounter++;

            if (addElemDamage >= Settings.WcToElemDamageSpell)
                WcaffixCounter++;
            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                    LogMessage("# of Affixes:" + WcaffixCounter, 10f);
            }
            return WcaffixCounter;
        }

        #endregion

        #region Weapon Attack

        private bool AnalyzeWeaponAttack(Entity item)
        {
            var WaaffixCounter = 0;

            var component = item.GetComponent<Weapon>();
            var mods = item.GetComponent<Mods>().ItemMods;

            var attackSpeed = 1f / (component.AttackTime / 1000f);
            attackSpeed *= 1f + mods.GetStatValue("LocalIncreasedAttackSpeed") / 100f;

            var phyDmg = (component.DamageMin + component.DamageMax) / 2f +
                         mods.GetAverageStatValue("LocalAddedPhysicalDamage");
            phyDmg *= 1f + (mods.GetStatValue("LocalIncreasedPhysicalDamagePercent") + 20) / 100f;
            if (phyDmg * attackSpeed >= Settings.WaPhysDmg)
                WaaffixCounter++;

            var elemDmg =
                mods.GetAverageStatValue("LocalAddedColdDamage") + mods.GetAverageStatValue("LocalAddedFireDamage")
                                                                 + mods.GetAverageStatValue(
                                                                     "LocalAddedLightningDamage");
            if (elemDmg * attackSpeed >= Settings.WaElemDmg)
                WaaffixCounter++;

            //DEBUG TEST BLOCK
            {
                if (Settings.DebugMode != false)
                {
                    LogMessage(component.DumpObject(), 10f);
                    LogMessage("# of Affixes:" + WaaffixCounter, 10f);
                }
            }
            return WaaffixCounter >= Settings.WaAffixes;
        }

        #endregion

        #region Load config

        private void ParseConfig_BaseType()
        {
            var path = $"{DirectoryFullName}\\BaseType.txt";

            CheckConfig(path);

            using (var reader = new StreamReader(path))
            {
                var text = reader.ReadToEnd();

                GoodBaseTypes = text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

                reader.Close();
            }
        }

        private void CheckConfig(string path)
        {
            if (File.Exists(path)) return;

            // Tier 1-3 NeverSink
            var text = "Opal Ring" + "\r\n" + "Steel Ring" + "\r\n" + "Vermillion Ring" + "\r\n" + "Blue Pearl Amulet" +
                       "\r\n" + "Bone Helmet" + "\r\n" +
                       "Cerulean Ring" + "\r\n" + "Convoking Wand" + "\r\n" + "Crystal Belt" + "\r\n" +
                       "Fingerless Silk Gloves" + "\r\n" + "Gripped Gloves" + "\r\n" +
                       "Marble Amulet" + "\r\n" + "Sacrificial Garb" + "\r\n" + "Spiked Gloves" + "\r\n" +
                       "Stygian Vise" + "\r\n" + "Two-Toned Boots" + "\r\n" +
                       "Vanguard Belt" + "\r\n" + "Diamond Ring" + "\r\n" + "Onyx Amulet" + "\r\n" + "Two-Stone Ring" +
                       "\r\n" + "Colossal Tower Shield" + "\r\n" +
                       "Eternal Burgonet" + "\r\n" + "Hubris Circlet" + "\r\n" + "Lion Pelt" + "\r\n" +
                       "Sorcerer Boots" + "\r\n" + "Sorcerer Gloves" + "\r\n" +
                       "Titanium Spirit Shield" + "\r\n" + "Vaal Regalia" + "\r\n";


            using (var streamWriter = new StreamWriter(path, true))
            {
                streamWriter.Write(text);
                streamWriter.Close();
            }
        }

        #endregion

        #region Sum Affix

        private static int Average(IReadOnlyList<int> x)
        {
            return (x[0] + x[1]) / 2;
        }

        private static List<ModValue> SumAffix(List<ModValue> mods)
        {
            foreach (var mod in mods)
            foreach (var mod2 in mods.Where(x => x != mod && mod.Record.Group == x.Record.Group))
            {
                mod2.StatValue[0] += mod.StatValue[0];
                mod2.StatValue[1] += mod.StatValue[1];
                mods.Remove(mod);
                return mods;
            }

            return mods;
        }

        private static int FixTierEs(string key)
        {
            return 9 - int.Parse(key.Last().ToString());
        }

        #endregion
    }

    #region Get item Stats

    public static class ModsExtension
    {
        public static float GetStatValue(this List<ItemMod> mods, string name)
        {
            var m = mods.FirstOrDefault(mod => mod.Name == name);
            return m?.Value1 ?? 0;
        }

        public static float GetAverageStatValue(this List<ItemMod> mods, string name)
        {
            var m = mods.FirstOrDefault(mod => mod.Name == name);
            return (m?.Value1 + m?.Value2) / 2 ?? 0;
        }
    }

    #endregion
}