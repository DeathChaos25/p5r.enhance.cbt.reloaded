using System.ComponentModel;
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

        [DisplayName("External Navigator text files")]
        [Description("Enables the usage of external navigator text binary from the game's files\nrather than being embedded in the game's executable.")]
        [DefaultValue(true)]
        [Category("Enhancements")]
        public bool _013_enableExternalNavi { get; set; } = true;

        [DisplayName("Disable Makoto as Navigator")]
        [Description("Prevents Makoto from appearing as a navigator.")]
        [DefaultValue(false)]
        [Category("Toggles")]
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

        [DisplayName("New Extra Outfits")]
        [Description("Adds new Outfits via Costume Framework.\nThis option does not contain the new costumes for Joker.")]
        [DefaultValue(true)]
        [Category("Extra Costumes (Costume Framework)")]
        public bool _019_extraCostumes { get; set; } = true;

        [DisplayName("New Extra Outfits (Joker)")]
        [Description("Adds new Outfits via Costume Framework.\nThis Option only contains the new costumes for Joker.")]
        [DefaultValue(true)]
        [Category("Extra Costumes (Costume Framework)")]
        public bool _019_extraCostumes_Joker { get; set; } = true;

        [DisplayName("Custom Combat Model Outline Colors")]
        [Description("Enables the ability to change the color of the solid red model copy made in combat.")]
        [DefaultValue(true)]
        [Category("Enhancements")]
        public bool _030_enableCustomColors { get; set; } = true;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xFF)]
        [Category("Combat Model Color: Joker")]
        public byte _031_JokerColor_R { get; set; } = 0xFF;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0)]
        [Category("Combat Model Color: Joker")]
        public byte _031_JokerColor_G { get; set; } = 0;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0)]
        [Category("Combat Model Color: Joker")]
        public byte _031_JokerColor_B { get; set; } = 0;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xF5)]
        [Category("Combat Model Color: Skull")]
        public byte _032_SkullColor_R { get; set; } = 0xF5;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xFE)]
        [Category("Combat Model Color: Skull")]
        public byte _032_SkullColor_G { get; set; } = 0xFE;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0x1F)]
        [Category("Combat Model Color: Skull")]
        public byte _032_SkullColor_B { get; set; } = 0x1F;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0x14)]
        [Category("Combat Model Color: Mona")]
        public byte _033_MonaColor_R { get; set; } = 0x14;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xD8)]
        [Category("Combat Model Color: Mona")]
        public byte _033_MonaColor_G { get; set; } = 0xD8;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xB1)]
        [Category("Combat Model Color: Mona")]
        public byte _033_MonaColor_B { get; set; } = 0xB1;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xF0)]
        [Category("Combat Model Color: Panther")]
        public byte _034_PantherColor_R { get; set; } = 0xF0;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0x4C)]
        [Category("Combat Model Color: Panther")]
        public byte _034_PantherColor_G { get; set; } = 0x4C;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xB8)]
        [Category("Combat Model Color: Panther")]
        public byte _034_PantherColor_B { get; set; } = 0xB8;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0x01)]
        [Category("Combat Model Color: Fox")]
        public byte _035_FoxColor_R { get; set; } = 0x01;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xCB)]
        [Category("Combat Model Color: Fox")]
        public byte _035_FoxColor_G { get; set; } = 0xCB;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xDB)]
        [Category("Combat Model Color: Fox")]
        public byte _035_FoxColor_B { get; set; } = 0xDB;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0x30)]
        [Category("Combat Model Color: Queen")]
        public byte _036_QueenColor_R { get; set; } = 0x30;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0x28)]
        [Category("Combat Model Color: Queen")]
        public byte _036_QueenColor_G { get; set; } = 0x28;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xC8)]
        [Category("Combat Model Color: Queen")]
        public byte _036_QueenColor_B { get; set; } = 0xC8;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xB0)]
        [Category("Combat Model Color: Noir")]
        public byte _037_NoirColor_R { get; set; } = 0xB0;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0)]
        [Category("Combat Model Color: Noir")]
        public byte _037_NoirColor_G { get; set; } = 0;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xA8)]
        [Category("Combat Model Color: Noir")]
        public byte _037_NoirColor_B { get; set; } = 0xA8;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xFF)]
        [Category("Combat Model Color: Crow (Prince)")]
        public byte _038_CrowPrinceColor_R { get; set; } = 0xFF;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xF8)]
        [Category("Combat Model Color: Crow (Prince)")]
        public byte _038_CrowPrinceColor_G { get; set; } = 0xF8;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xED)]
        [Category("Combat Model Color: Crow (Prince)")]
        public byte _038_CrowPrinceColor_B { get; set; } = 0xED;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xA4)]
        [Category("Combat Model Color: Crow (Dark)")]
        public byte _039_CrowDarkColor_R { get; set; } = 0xA4;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0x82)]
        [Category("Combat Model Color: Crow (Dark)")]
        public byte _039_CrowDarkColor_G { get; set; } = 0x82;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0x52)]
        [Category("Combat Model Color: Crow (Dark)")]
        public byte _039_CrowDarkColor_B { get; set; } = 0x52;

        [DisplayName("R")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0xFF)]
        [Category("Combat Model Color: Violet")]
        public byte _040_VioletColor_R { get; set; } = 0xFF;

        [DisplayName("G")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0)]
        [Category("Combat Model Color: Violet")]
        public byte _040_VioletColor_G { get; set; } = 0;

        [DisplayName("B")]
        [Description("Character color to replace the solid red color used in combat model copy.\n(Note: This will only work if the Custom Combat Model Outline Colors setting is enabled!)")]
        [DefaultValue(0x5D)]
        [Category("Combat Model Color: Violet")]
        public byte _040_VioletColor_B { get; set; } = 0x5D;

        [DisplayName("Joker Voice pack")]
        [Description("Include Joker's combat voicepack to be used with the new Joker Voices toggle.\n\nNOTE: Only English voice language files are included!")]
        [DefaultValue(true)]
        [Category("Mod Files")]
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
