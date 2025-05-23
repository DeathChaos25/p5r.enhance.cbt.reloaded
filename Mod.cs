using p5r.enhance.cbt.reloaded.Configuration;
using p5r.enhance.cbt.reloaded.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using static p5r.enhance.cbt.reloaded.Utils;
using static p5r.enhance.cbt.reloaded.GameFunctions_Structs;
using CriFs.V2.Hook.Interfaces;
using BGME.Framework.Interfaces;
using P5R.CostumeFramework.Interfaces;

namespace p5r.enhance.cbt.reloaded
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        private GameFunctions _gameFunctions;
        private Misc_Enhancements _misc_enhance;

        public unsafe Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            Initialise(_logger, _configuration, _modLoader);

            _gameFunctions = new GameFunctions(context);
            _misc_enhance = new Misc_Enhancements(context);

            var criFsController = _modLoader.GetController<ICriFsRedirectorApi>();
            if (criFsController == null || !criFsController.TryGetTarget(out var criFsApi))
            {
                LogError($"CriFS has failed to load! Normal files will not load properly!");
                return;
            }

            // criFS
            if (_configuration._mod_JokerVoice)
            {
                criFsApi.AddProbingPath(Path.Combine("OptionalModFiles", "JOKERVOICE.CPK"));
            }
            if (_configuration._mod_yusukeEyeFix)
            {
                criFsApi.AddProbingPath(Path.Combine("OptionalModFiles", "YUSUKEEYEFIX.CPK"));
            }
            if (_configuration._mod_pszModels)
            {
                criFsApi.AddProbingPath(Path.Combine("OptionalModFiles", "PSZMODELS.CPK"));
            }
            if (_configuration._mod_RyujiScene)
            {
                criFsApi.AddProbingPath(Path.Combine("OptionalModFiles", "RYUJISCENE.CPK"));
            }
            if (_configuration._mod_debugNPC)
            {
                criFsApi.AddProbingPath(Path.Combine("OptionalModFiles", "DEBUGNPC.CPK"));
            }
            if (_configuration._mod_noPreRenders)
            {
                criFsApi.AddProbingPath(Path.Combine("OptionalModFiles", "NOPRERENDERS.CPK"));
            }

            var BGMEController = _modLoader.GetController<IBgmeApi>();
            if (BGMEController == null || !BGMEController.TryGetTarget(out var bgmeApi))
            {
                LogError($"BGME has failed to load!");
                return;
            }

            if (_configuration._000_enableExtraBGM)
            {
                bgmeApi.AddPath(Path.Join(this._modLoader.GetDirectoryForModId(this._modConfig.ModId), "ExtraBaseBGM", "AmbushAmbushedBGM.pme"));
            }

            if (_configuration._019_extraCostumes || _configuration._019_extraCostumes_Joker)
            {
                var CFController = _modLoader.GetController<ICostumeApi>();
                if (CFController == null || !CFController.TryGetTarget(out var cfAPI))
                {
                    _logger.WriteLine($"Costume Framework has failed to load!", System.Drawing.Color.Red);
                    return;
                }

                if (_configuration._019_extraCostumes)
                {
                    cfAPI.AddCostumesFolder(this._modConfig.ModId, Path.Join(_modLoader.GetDirectoryForModId(this._modConfig.ModId), "ExtraCostumes"));
                }

                if (_configuration._019_extraCostumes_Joker)
                {
                    cfAPI.AddCostumesFolder(this._modConfig.ModId, Path.Join(_modLoader.GetDirectoryForModId(this._modConfig.ModId), "ExtraCostumes_Joker"));
                }

                if (_configuration._019_extraCostumes_BGM)
                {
                    cfAPI.AddCostumesFolder(this._modConfig.ModId, Path.Join(_modLoader.GetDirectoryForModId(this._modConfig.ModId), "ExtraCostumes_BGM"));
                }
            }

            LoadCBTMod DllLoader = new LoadCBTMod();
            DllLoader.LoadCppDll(context);


            // For more information about this template, please see
            // https://reloaded-project.github.io/Reloaded-II/ModTemplate/

            // If you want to implement e.g. unload support in your mod,
            // and some other neat features, override the methods in ModBase.

            // TODO: Implement some mod logic
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}