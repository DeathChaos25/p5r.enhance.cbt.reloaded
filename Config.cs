using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using p5r.enhance.cbt.reloaded.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;

namespace p5r.enhance.cbt.reloaded.Configuration
{
    public class Config : Configurable<Config>
    {
        [DisplayName("Combat BGM Enhancements")]
        [Description("Enables the extra non DLC Combat BGM.\nThis Includes:\nPlayer Advantage -> 2 new BGMs\nNeutral Advantage -> 1 new BGM\nEnemy Advantage -> 1 new BGM")]
        [DefaultValue(true)]
        [Category("Enhancements")]
        [Display(Order=0)]
        public bool _000_enableExtraBGM { get; set; } = true;

        [DisplayName("Cutscene Outfits")]
        [Description("Enables the player's equipped outfits to affect in engine cutscenes.")]
        [DefaultValue(true)]
        [Category("Enhancements")]
        public bool _010_enableCutsceneOutfits { get; set; } = true;

        [DisplayName("Randomized Title Screen")]
        [Description("Randomizes the title screen model and bgm between\none of three themed variants:\nOriginal Variant (unchanged)\nP5 Variant (Shujin uniform models)\nP5S variant (P5S colored models)")]
        [DefaultValue(true)]
        [Category("Enhancements")]
        public bool _011_randomTitle { get; set; } = true;

        [DisplayName("Weekday Announcements")]
        [Description("Enables the usage of Joker's unused voicelines to announce the calendar day\non the daily calendar transition after every night.")]
        [DefaultValue(true)]
        [Category("Enhancements")]
        public bool _012_enableAnnounceWeekday { get; set; } = true;

        [DisplayName("Disable Makoto as Navigator")]
        [Description("Prevents Makoto from appearing as a navigator.")]
        [DefaultValue(false)]
        [Category("Toggles")]
        [Display(Order = 1)]
        public bool _014_disableMakotoNavi { get; set; } = false;

        [DisplayName("Extra Joker Combat Voices")]
        [Description("Adds additional combat voicelines to Joker.")]
        [DefaultValue(true)]
        [Category("Enhancements")]
        public bool _015_enableJokerVoices { get; set; } = true;

        [DisplayName("Fix double equip stats bug")]
        [Description("Fixes a bug that exists in the game that causes\nequipment stats to be counted twice.")]
        [DefaultValue(true)]
        [Category("Toggles")]
        public bool _016_fixDoubleStatsBug { get; set; } = true;

        [DisplayName("Restore Printing Functions")]
        [Description("Makes the flow functions PUT/PUTF/PUTS work again.")]
        [DefaultValue(true)]
        [Category("Toggles")]
        public bool _017_restoreDebugPrintingFunctions { get; set; } = true;

        [DisplayName("Invalid Shader Crash Workaround")]
        [Description("Prevents invalid shader crashes due to model material combinations\nby attempting to use the default shader.")]
        [DefaultValue(true)]
        [Category("Toggles")]
        public bool _018_shaderWorkaround { get; set; } = true;

        [DisplayName("Faster Event Fast Forward")]
        [Description("Forces the screen crumple type of event skip to be usable in\nevery type of event and not just specific ones.")]
        [DefaultValue(true)]
        [Category("Toggles")]
        public bool _020_QuickEventSkip { get; set; } = false;

        [DisplayName("New Extra Outfits")]
        [Description("Adds new Outfits via Costume Framework.\nThis option does not contain the new costumes for Joker.")]
        [DefaultValue(true)]
        [Category("Extra Costumes (Costume Framework)")]
        [Display(Order = 3)]
        public bool _019_extraCostumes { get; set; } = true;

        [DisplayName("New Extra Outfits (Joker)")]
        [Description("Adds new Outfits via Costume Framework.\nThis Option only contains the new costumes for Joker.")]
        [DefaultValue(true)]
        [Category("Extra Costumes (Costume Framework)")]
        public bool _019_extraCostumes_Joker { get; set; } = true;

        [DisplayName("Enhanced DLC BGM")]
        [Description("Enhances existing DLC BGM.\nNOTE: Some of the new costumes rely on this option to have their new BGM\n(Such as the Moonlight or Persona 5 Strikers Costume).")]
        [DefaultValue(true)]
        [Category("Extra Costumes (Costume Framework)")]
        public bool _019_extraCostumes_BGM { get; set; } = true;

        [DisplayName("Custom Combat Model Outline Colors")]
        [Description("Enables the ability to change the color of the solid red model copy made in combat.")]
        [DefaultValue(true)]
        [Category("Enhancements")]
        public bool _030_enableCustomColors { get; set; } = true;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xFF)]
        [Category("Combat Model Color: Joker")]
        [Display(Order = 11)]
        public byte _031_JokerColor_R { get; set; } = (byte)0xFF;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0)]
        [Category("Combat Model Color: Joker")]
        [Display(Order = 12)]
        public byte _031_JokerColor_G { get; set; } = (byte)0;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0)]
        [Category("Combat Model Color: Joker")]
        [Display(Order = 13)]
        public byte _031_JokerColor_B { get; set; } = (byte)0;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xF5)]
        [Display(Order = 21)]
        [Category("Combat Model Color: Skull")]
        public byte _032_SkullColor_R { get; set; } = (byte)0xF5;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xFE)]
        [Display(Order = 22)]
        [Category("Combat Model Color: Skull")]
        public byte _032_SkullColor_G { get; set; } = (byte)0xFE;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0x1F)]
        [Display(Order = 22)]
        [Category("Combat Model Color: Skull")]
        public byte _032_SkullColor_B { get; set; } = (byte)0x1F;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0x14)]
        [Display(Order = 31)]
        [Category("Combat Model Color: Mona")]
        public byte _033_MonaColor_R { get; set; } = (byte)0x14;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xD8)]
        [Display(Order = 32)]
        [Category("Combat Model Color: Mona")]
        public byte _033_MonaColor_G { get; set; } = (byte)0xD8;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xB1)]
        [Display(Order = 33)]
        [Category("Combat Model Color: Mona")]
        public byte _033_MonaColor_B { get; set; } = (byte)0xB1;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xF0)]
        [Display(Order = 41)]
        [Category("Combat Model Color: Panther")]
        public byte _034_PantherColor_R { get; set; } = (byte)0xF0;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0x4C)]
        [Display(Order = 42)]
        [Category("Combat Model Color: Panther")]
        public byte _034_PantherColor_G { get; set; } = (byte)0x4C;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xB8)]
        [Display(Order = 43)]
        [Category("Combat Model Color: Panther")]
        public byte _034_PantherColor_B { get; set; } = (byte)0xB8;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0x1)]
        [Display(Order = 51)]
        [Category("Combat Model Color: Fox")]
        public byte _035_FoxColor_R { get; set; } = (byte)0x1;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xCB)]
        [Display(Order = 52)]
        [Category("Combat Model Color: Fox")]
        public byte _035_FoxColor_G { get; set; } = (byte)0xCB;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xDB)]
        [Display(Order = 53)]
        [Category("Combat Model Color: Fox")]
        public byte _035_FoxColor_B { get; set; } = (byte)0xDB;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0x30)]
        [Display(Order = 61)]
        [Category("Combat Model Color: Queen")]
        public byte _036_QueenColor_R { get; set; } = (byte)0x30;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0x28)]
        [Display(Order = 62)]
        [Category("Combat Model Color: Queen")]
        public byte _036_QueenColor_G { get; set; } = (byte)0x28;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xC8)]
        [Display(Order = 63)]
        [Category("Combat Model Color: Queen")]
        public byte _036_QueenColor_B { get; set; } = (byte)0xC8;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xB0)]
        [Display(Order = 71)]
        [Category("Combat Model Color: Noir")]
        public byte _037_NoirColor_R { get; set; } = (byte)0xB0;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0)]
        [Display(Order = 72)]
        [Category("Combat Model Color: Noir")]
        public byte _037_NoirColor_G { get; set; } = (byte)0;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xA8)]
        [Display(Order = 73)]
        [Category("Combat Model Color: Noir")]
        public byte _037_NoirColor_B { get; set; } = (byte)0xA8;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xFF)]
        [Display(Order = 91)]
        [Category("Combat Model Color: Crow (Prince)")]
        public byte _038_CrowPrinceColor_R { get; set; } = (byte)0xFF;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xF8)]
        [Display(Order = 92)]
        [Category("Combat Model Color: Crow (Prince)")]
        public byte _038_CrowPrinceColor_G { get; set; } = (byte)0xF8;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xED)]
        [Display(Order = 93)]
        [Category("Combat Model Color: Crow (Prince)")]
        public byte _038_CrowPrinceColor_B { get; set; } = (byte)0xED;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xA4)]
        [Display(Order = 94)]
        [Category("Combat Model Color: Crow (Dark)")]
        public byte _039_CrowDarkColor_R { get; set; } = (byte)0xA4;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0x82)]
        [Display(Order = 95)]
        [Category("Combat Model Color: Crow (Dark)")]
        public byte _039_CrowDarkColor_G { get; set; } = (byte)0x82;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0x52)]
        [Display(Order = 96)]
        [Category("Combat Model Color: Crow (Dark)")]
        public byte _039_CrowDarkColor_B { get; set; } = (byte)0x52;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0xFF)]
        [Display(Order = 101)]
        [Category("Combat Model Color: Violet")]
        public byte _040_VioletColor_R { get; set; } = (byte)0xFF;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0)]
        [Display(Order = 102)]
        [Category("Combat Model Color: Violet")]
        public byte _040_VioletColor_G { get; set; } = (byte)0;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue((byte)0x5D)]
        [Display(Order = 103)]
        [Category("Combat Model Color: Violet")]
        public byte _040_VioletColor_B { get; set; } = (byte)0x5D;

        [DisplayName("Joker Voice pack")]
        [Description("Include Joker's combat voicepack to be used with the new Joker Voices toggle.\n\nNOTE: Only English voice language files are included!")]
        [DefaultValue(true)]
        [Category("Mod Files")]
        [Display(Order = 2)]
        public bool _mod_JokerVoice { get; set; } = true;

        [DisplayName("Yusuke Eye Fix")]
        [Description("Include fixed Yusuke models for cutscene outfits usage.")]
        [DefaultValue(true)]
        [Category("Mod Files")]
        public bool _mod_yusukeEyeFix { get; set; } = true;

        [DisplayName("No Pre-rendered scenes")]
        [Description("Include necessary files to make some in-engine pre-render scenes be real time in engine scenes once more.")]
        [DefaultValue(true)]
        [Category("Mod Files")]
        public bool _mod_noPreRenders { get; set; } = true;

        [DisplayName("Modified Ryuji Ship Palace Scene")]
        [Description("For real?")]
        [DefaultValue(true)]
        [Category("Mod Files")]
        public bool _mod_RyujiScene { get; set; } = true;

        [DisplayName("PSZ Models")]
        [Description("Enable pre-bundled PSZ models that come with this mod\nto prevent softlocks due to missing models.\n(Note: These are (probably) unnecessary if using a full compendium replacement mod!)")]
        [DefaultValue(true)]
        [Category("Mod Files")]
        public bool _mod_pszModels { get; set; } = true;

        [DisplayName("Debug NPC")]
        [Description("Enable pre-bundled files that add a custom NPC to the game's dummy field.\nThis custom NPC allows you to access a reconstructed debug menu that\nwas made using leftover debug scripts in the game's files.")]
        [DefaultValue(true)]
        [Category("Mod Files")]
        public bool _mod_debugNPC { get; set; } = true;
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        // 
    }
}
