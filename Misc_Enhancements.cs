using p5r.enhance.cbt.reloaded.Configuration;
using p5r.enhance.cbt.reloaded.Template;
using p5rpc.flowscriptframework.interfaces;
using p5rpc.lib.interfaces;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Memory;
using Reloaded.Memory;
using Reloaded.Memory.Interfaces;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using static p5r.enhance.cbt.reloaded.GameFunctions_Structs;
using static p5r.enhance.cbt.reloaded.Utils;
using static p5rpc.lib.interfaces.Enums;

namespace p5r.enhance.cbt.reloaded
{
    internal unsafe class Misc_Enhancements
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
        private static Config _configuration;

        private static IP5RLib? _p5rLib;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        static private GameFunctions _gameFunctions;

        static private bool isDayAnnounced = false;

        public unsafe delegate int calendar_trans_play_knife_sfx_Delegate(CalendarTransStruct* a1);
        static private IHook<calendar_trans_play_knife_sfx_Delegate> _hook_calendar_trans_play_knife_sfx;

        public unsafe delegate int GetUnitModelID_Delegate(ushort a1, ushort a2, ushort a3);
        static private IHook<GetUnitModelID_Delegate> _hookGetUnitModelID;

        public unsafe delegate nint LoadResourceImpl_Delegate(ushort type, byte a2, ushort index, ushort major, byte minor, byte sub, short a7, nint a8, ushort a9, short a10);
        static private IHook<LoadResourceImpl_Delegate> _hookLoadResourceImpl;

        public unsafe delegate nint SetTitleBGM_Delegate(TitleScreenStruct* a1);
        static private IHook<SetTitleBGM_Delegate> _hookSetTitleBGM;

        public unsafe delegate int UnitTurnStart_Delegate(StructD* a1, nint a2, nint a3, nint a4);
        static private IHook<UnitTurnStart_Delegate> _hookUnitTurnStart;

        public unsafe delegate void CombineColorBytes_Delegate(nint a1, SoundBank_Struct* a2, nint a3);
        static private IHook<CombineColorBytes_Delegate> _hookCombineColorBytes;

        public unsafe delegate void CheckShaderExists_Delegate(nint a1);
        static private IHook<CheckShaderExists_Delegate> _hookCheckShaderExists;

        public unsafe delegate bool isMakotoNaviAvailable_Delegate();
        static private IHook<isMakotoNaviAvailable_Delegate> _hookIsMakotoNaviAvailable;

        public unsafe delegate void checkHasEnemyDDSBossName_Delegate(EnemyPersonaFunctionStruct3* a1);
        static private IHook<checkHasEnemyDDSBossName_Delegate> _hookcheckHasEnemyDDSBossName;

        public unsafe delegate bool AreBattleUnitsDeadDelegate(Participate* a1);
        static private IHook<AreBattleUnitsDeadDelegate> _hookAreBattleUnitsDead;

        public delegate nint PlayerEscapeDelegate(nint a1, nint a2);
        static private IHook<PlayerEscapeDelegate> _hookPlayerEscape;

        public delegate nint MarukiDetoxDelegate(Participate* a1, nint a2);
        static private IHook<MarukiDetoxDelegate> _hookMarukiDetox;

        public static byte rndTitle = 0;

        private static nint TitleBGMAddr = 0;
        private static nint PlayerOutlineColorAddr = 0;
        private static nint EnemyIDDDSNamePatchAddress = 0;

        private static readonly int[] BTL_COUNTERS = new int[512];

        private static nint UNIT_TBL_Section0_PTR;
        private static nint UNIT_TBL_Section1_PTR;
        private static nint UNIT_TBL_Section3_PTR;
        private static nint UNIT_TBL_Section4_PTR;
        private static nint ELSAI_TBL_Section0_PTR;
        private static nint ENCOUNT_TBL_Section0_PTR;

        private static nint SKILL_TBL_Section0_PTR;
        private static nint SKILL_TBL_Section1_PTR;

        private static nint ENEMY_ANALYSIS_DATA_PTR;

        private static nint CurrentAIBasePTR;

        private static int UNIT_TBL_Section0_EntrySize = 68;    // 0x44
        private static int UNIT_TBL_Section1_EntrySize = 40;    // 0x28
        private static int UNIT_TBL_Section3_EntrySize = 24;    // 0x18
        private static int UNIT_TBL_Section4_EntrySize = 6;     // 0x06
        private static int ELSAI_TBL_Section0_EntrySize = 44;   // 0x2C
        private static int ENCOUNT_TBL_Section0_EntrySize = 44; // 0x2C

        private static int SKILL_TBL_Section0_EntrySize = 8;  // 0x08
        private static int SKILL_TBL_Section1_EntrySize = 48; // 0x30

        internal Misc_Enhancements(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            Array.Fill(BTL_COUNTERS, 0);

            _gameFunctions = new GameFunctions(context);

            var memory = Memory.Instance;

            var flowFrameworkController = _modLoader.GetController<IFlowFramework>();
            if (flowFrameworkController == null || !flowFrameworkController.TryGetTarget(out var flowFramework))
            {
                throw new Exception("Failed to get IFlowFramework Controller");
            }

            var p5rLibController = _modLoader.GetController<IP5RLib>();
            if (p5rLibController == null || !p5rLibController.TryGetTarget(out var p5rLib))
            {
                throw new Exception("Failed to get IP5RLib Controller");
            }

            _p5rLib = p5rLib;

            // v1.0.0 = 0x14097D305
            SigScan("48 8B 05 ?? ?? ?? ?? 0F B6 50 ?? 0F BF 08 E8 ?? ?? ?? ?? 3C 09", "get_days_Sig", address =>
            {
                _gameFunctions.SetDaysAddr(address);
            });

            if (_configuration._016_fixDoubleStatsBug)
            {
                // v1.0.0 = 0x140dee1f2
                SigScan("E8 ?? ?? ?? ?? 41 0F B7 D4 0F BE D8", "double_stats_bug_1_Sig", address =>
                {
                    WriteReturn0(address);
                    WriteReturn0(address + 0xF);
                    WriteReturn0(address + 0x21);
                    WriteReturn0(address + 0x33);
                });

                // v1.0.0 = 0x140dee347
                SigScan("E8 ?? ?? ?? ?? BA 03 00 00 00 0F BE D8", "double_stats_bug_2_Sig", address =>
                {
                    WriteReturn0(address);
                    WriteReturn0(address + 0x10);
                    WriteReturn0(address + 0x23);
                    WriteReturn0(address + 0x36);
                });
            }

            if (_configuration._012_enableAnnounceWeekday)
            {
                Log("Joker announcing Week Day Enabled");
                // v1.0.0 = 0x140bd1c70
                SigScan("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 40 44 8B 05 ?? ?? ?? ??", "calendar_trans_play_knife_sfx_Sig", address =>
                {
                    _hook_calendar_trans_play_knife_sfx = _hooks.CreateHook<calendar_trans_play_knife_sfx_Delegate>(HookCalendarTransPlayKnifeSfx, address).Activate();
                });
            }

            // v1.0.0 = 0x14166b400
            SigScan("66 44 89 4C 24 ?? 44 88 44 24 ??", "get_unit_model_id_Sig", address =>
            {
                _hookGetUnitModelID = _hooks.CreateHook<GetUnitModelID_Delegate>(GetUnitModelId, address).Activate();
            });


            // v1.0.0 = 0x141648aa0
            SigScan("40 53 55 56 57 41 54 41 55 41 56 41 57 48 83 EC 48 0F B7 9C 24 ?? ?? ?? ??", "LoadResourceImpl_Sig", address =>
            {
                _hookLoadResourceImpl = _hooks.CreateHook<LoadResourceImpl_Delegate>(LoadResourceImpl, address).Activate();
            });


            // v1.0.0 = 0x14146bba7
            // v1.0.4 = 0x14138e75f
            SigScan("B9 85 03 00 00 66 89 53 ??", "set_titleBGM_Sig", address =>
            {
                TitleBGMAddr = address + 1;

                if (_configuration._011_randomTitle)
                {
                    rndTitle = (byte)_gameFunctions.RandomIntBetween(0, 2);

                    int rndBGM = rndTitle;
                    if (rndBGM == 1)
                    {
                        memory.SafeWrite((nuint)TitleBGMAddr, BitConverter.GetBytes((uint)101));   // Cue 101, P5 title screen
                    }
                    else if (rndBGM == 2)
                    {
                        memory.SafeWrite((nuint)TitleBGMAddr, BitConverter.GetBytes((uint)10999)); // Cue 10999, P5S title screen
                    }
                    else
                    {
                        memory.SafeWrite((nuint)TitleBGMAddr, BitConverter.GetBytes((uint)901));  // Cue 901, P5R title screen
                    }
                }
            });

            // v1.0.0 = 0x14146b940
            // v1.0.4 = 0x14138e3c0
            SigScan("48 8B C4 48 89 58 ?? 48 89 68 ?? 48 89 70 ?? 57 41 54 41 55 41 56 41 57 48 81 EC F0 04 00 00", "set_titleBGMFunc_Sig", address =>
            {
                if (_configuration._011_randomTitle)
                    _hookSetTitleBGM = _hooks.CreateHook<SetTitleBGM_Delegate>(SetTitleBGMFunc, address).Activate();
            });


            // v1.0.0 = 0x140ab0801
            // v1.0.4 = 0x140a8b6bf
            SigScan("C7 85 ?? ?? ?? ?? FF 00 00 FF 48 83 C2 18 4C 89 6C 24 ??", "red_combat_model_color_Sig", address =>
            {
                PlayerOutlineColorAddr = address;
            });

            // v1.0.0 = 0x140759dc0
            SigScan("4C 8B DC 4D 89 43 ?? 49 89 53 ?? 55 41 55", "unit_turn_start_Sig", address =>
            {
                if (_configuration._030_enableCustomColors)
                    _hookUnitTurnStart = _hooks.CreateHook<UnitTurnStart_Delegate>(UnitTurnStart, address).Activate();
            });

            // v1.0.0 = 0x14094be02
            SigScan("E8 ?? ?? ?? ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 8B C8 48 8D 54 24 ?? E8 ?? ?? ?? ?? 48 8B C8 E8 ?? ?? ?? ?? 48 8B C8 48 8D 54 24 ?? E8 ?? ?? ?? ?? 48 8B C8 E8 ?? ?? ?? ?? 48 8B C8 B2 01 E8 ?? ?? ?? ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B CE", "combine_color_struct_Sig", address =>
            {
                if (_configuration._030_enableCustomColors)
                {
                    var funcAddress = GetGlobalAddress(address + 1);
                    _hookCombineColorBytes = _hooks.CreateHook<CombineColorBytes_Delegate>(CombineColorBytes, (long)funcAddress).Activate();
                }
            });

            // v1.0.0 = 0x140205c3a
            SigScan("E8 ?? ?? ?? ?? 44 0F B6 65 ?? 45 84 E4", "check_shader_sig", address =>
            {
                if (_configuration._018_shaderWorkaround)
                {
                    var funcAddress = GetGlobalAddress(address + 1);
                    _hookCheckShaderExists = _hooks.CreateHook<CheckShaderExists_Delegate>(CheckShaderExists, (long)funcAddress).Activate();
                }
            });

            // v1.0.0 = 0x14163cb30
            // v1.0.4 = 0x1415359f0
            SigScan("48 83 EC 28 4C 8B 01 4C 8B D1", "check_load_PSZ_Model_Sig", address =>
            {
                byte[] Bytes = { 0x48, 0xE9 }; // // change jz to jmp

                memory.SafeWrite((nuint)address + 0x3A, Bytes);
            });

            // v1.0.4 = 0x140b527c0
            SigScan("48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8D AC 24 ?? ?? ?? ?? 48 81 EC 10 02 00 00", "checkHasEnemyDDSBossName", address =>
            {
                _hookcheckHasEnemyDDSBossName = _hooks.CreateHook<checkHasEnemyDDSBossName_Delegate>(checkHasEnemyDDSBossName, address).Activate();
            });

            // v1.0.4 = 0x140b527c0
            SigScan("C7 45 ?? 62 02 63 02 C7 45 ?? E8 02 E9 02 C7 45 ?? AE 02 AF 02 C7 45 ?? 34 02 35 02 C7 45 ?? 36 02 37 02 C7 45 00 39 02 45 02", "checkHasEnemyDDSBossName Enemy ID", address =>
            {
                EnemyIDDDSNamePatchAddress = address;
            });

            // v1.0.0 = 0x14163cb52
            SigScan("45 33 C0 48 8D 15 ?? ?? ?? ?? 66 0F 1F 44 ?? 00 0F B7 02 3B C1 74 ?? 41 FF C0 48 83 C2 02 41 81 F8 AB 00 00 00", "PSZ_check_2_Sig", address =>
            {
                byte[] Bytes = { 0xEB }; // // change jz to jmp

                memory.SafeWrite((nuint)address + 0x15, Bytes);
            });

            // v1.0.4 = 0x?????????
            SigScan("83 F8 1A 0F 86 ?? ?? ?? ??", "Joker Select Portait Range Fix", address =>
            {
                byte[] Bytes = { 0x31 }; // change 26 to 49

                memory.SafeWrite((nuint)address + 2, Bytes);
            });

            // v1.0.0 = 0x140ea3e15
            SigScan("E8 ?? ?? ?? ?? 84 C0 74 ?? C7 87 ?? ?? ?? ?? 01 00 00 00 B9 8A 00 00 00", "quick_evt_skip_enable_Sig", address =>
            {
                if (_configuration._020_QuickEventSkip)
                {
                    WriteReturn1(address);
                }
            });

            if (_configuration._013_NoGameOverOnJokerDie)
            {
                SigScan("4C 8B DC 48 83 EC 48 F6 41 ?? 04", "areBattleUnitsDead_Sig", address =>
                {
                    _hookAreBattleUnitsDead = _hooks.CreateHook<AreBattleUnitsDeadDelegate>(AreBattleUnitsDead, address).Activate();
                });

                SigScan("48 8B C4 48 89 50 ?? 55 53 56 48 8D A8 ?? ?? ?? ??", "PlayerEscape", address =>
                {
                    _hookPlayerEscape = _hooks.CreateHook<PlayerEscapeDelegate>(PlayerEscape, address).Activate();
                });

                SigScan("48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8B EC 48 81 EC 80 00 00 00 B9 05 01 00 00", "Maruki Detox Check", address =>
                {
                    _hookMarukiDetox = _hooks.CreateHook<MarukiDetoxDelegate>(MarukiDetox, address).Activate();
                });
            }


            // v1.0.0 = 0x140de801d
            SigScan("49 81 FA 40 0C 01 00", "PersonaVisualTBLALimit_Sig", address =>
            {
                memory.SafeWrite((nuint)address + 3, BitConverter.GetBytes(148 * 1000));
            });

            // v1.0.0 = 0x140de8327
            // v1.0.4 = 0x140d76b8f
            SigScan("48 C7 45 ?? D0 01 00 00", "PersonaVisualTBLBLimit_Sig", address =>
            {
                memory.SafeWrite((nuint)address + 4, BitConverter.GetBytes(1000));
            });

            // v1.0.0 = 0x140b749a0
            SigScan("48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8B EC 48 81 EC 80 00 00 00 45 33 FF", "isMakotoAvailableNavi_Sig", address =>
            {
                if (_configuration._014_disableMakotoNavi)
                {
                    _hookIsMakotoNaviAvailable = _hooks.CreateHook<isMakotoNaviAvailable_Delegate>(() => false, address).Activate();
                }
            });

            // v1.0.4 = 0x1415df4fc
            SigScan("48 8D 05 ?? ?? ?? ?? 8B 14 ?? 41 8B C1", "UNIT_TBL_Section0_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                UNIT_TBL_Section0_PTR = (nint)funcAddress;
                Log($"UNIT_TBL_Section0_PTR: 0x{UNIT_TBL_Section0_PTR:X}");
            });

            // v1.0.4 = 0x140d7a8e9
            SigScan("48 8D 35 ?? ?? ?? ?? 0F C8 48 83 C3 04 4C 63 C0 48 8B D3 89 44 24 ?? 48 8B CE E8 ?? ?? ?? ?? 4C 63 44 24 ?? 41 8D 50 ?? 8B C2 25 0F 00 00 80 7D ?? FF C8 83 C8 F0 FF C0 F7 D8 4A 8D 3C ??", "UNIT_TBL_Section1_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                UNIT_TBL_Section1_PTR = (nint)funcAddress;
                Log($"UNIT_TBL_Section1_PTR: 0x{UNIT_TBL_Section1_PTR:X}");
            });

            // v1.0.4 = 0x140d7a9c6
            SigScan("48 8D 0D ?? ?? ?? ?? 0F C8 48 83 C7 04 4C 63 C0 48 8B D7 89 44 24 ?? E8 ?? ?? ?? ?? 4C 63 44 24 ??", "UNIT_TBL_Section3_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                UNIT_TBL_Section3_PTR = (nint)funcAddress;
                Log($"UNIT_TBL_Section3_PTR: 0x{UNIT_TBL_Section3_PTR:X}");
            });

            // v1.0.4 = 0x140a45b5f
            SigScan("48 8D 0D ?? ?? ?? ?? 0F B7 0C ?? B8 A0 00 00 00", "UNIT_TBL_Section4_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                UNIT_TBL_Section4_PTR = (nint)funcAddress;
                Log($"UNIT_TBL_Section4_PTR: 0x{UNIT_TBL_Section4_PTR:X}");
            });

            // v1.0.4 = 0x14c322259
            SigScan("48 8D 35 ?? ?? ?? ?? 0F C8 48 83 C3 04 4C 63 C0 48 89 DA", "ELSAI_TBL_Section0_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                ELSAI_TBL_Section0_PTR = (nint)funcAddress;
                Log($"ELSAI_TBL_Section0_PTR: 0x{ELSAI_TBL_Section0_PTR:X}");
            });

            // v1.0.4 = 0x141172149
            SigScan("48 03 15 ?? ?? ?? ?? 0F B7 0A", "ENCOUNT_TBL_Section0_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                ENCOUNT_TBL_Section0_PTR = (nint)funcAddress;
                Log($"ENCOUNT_TBL_Section0_PTR: 0x{ENCOUNT_TBL_Section0_PTR:X}");
            });

            SigScan("48 8D 0D ?? ?? ?? ?? 80 7C ?? ?? 01", "SKILL_TBL_Section0_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                SKILL_TBL_Section0_PTR = (nint)funcAddress;
                Log($"SKILL_TBL_Section0_PTR: 0x{SKILL_TBL_Section0_PTR:X}");
            });

            SigScan("48 8D 05 ?? ?? ?? ?? 80 7C ?? ?? 01", "SKILL_TBL_Section1_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                SKILL_TBL_Section1_PTR = (nint)funcAddress;
                Log($"SKILL_TBL_Section1_PTR: 0x{SKILL_TBL_Section1_PTR:X}");
            });

            SigScan("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 0F B6 05 ?? ?? ?? ?? 48 8D 1D ?? ?? ?? ??", "ENEMY_ANALYSIS_DATA_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                ENEMY_ANALYSIS_DATA_PTR = (nint)funcAddress;
                Log($"ENEMY_ANALYSIS_DATA_PTR: 0x{ENEMY_ANALYSIS_DATA_PTR:X}");
            });

            SigScan("4C 8B 05 ?? ?? ?? ?? 41 8B 50 ?? 29 CA", "CurrentAIBasePTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                CurrentAIBasePTR = (nint)funcAddress;
                Log($"CurrentAIBasePTR: 0x{CurrentAIBasePTR:X}");
            });

            ////////////////////////////////
            ////////////////////////////////
            ////////////////////////////////
            // standalone flow functions

            var flowApi = flowFramework.GetFlowApi();

            if (_configuration._017_restoreDebugPrintingFunctions)
            {
                flowFramework.Register("PUT", 1, () =>
                {
                    LogNoPrefix(flowApi.GetIntArg(0).ToString());
                    return FlowStatus.SUCCESS;
                }, 0x0002);

                flowFramework.Register("PUTS", 1, () =>
                {
                    LogNoPrefix(flowApi.GetStringArg(0));
                    return FlowStatus.SUCCESS;
                }, 0x0003);

                flowFramework.Register("PUTF", 1, () =>
                {
                    LogNoPrefix(flowApi.GetFloatArg(0).ToString());
                    return FlowStatus.SUCCESS;
                }, 0x0004);

                flowFramework.Register("PUT_HEX", 1, () =>
                {
                    LogNoPrefix($"0x{flowApi.GetIntArg(0):X8}");
                    return FlowStatus.SUCCESS;
                });
            }

            flowFramework.Register("BTL_GET_COUNTER", 1, () =>
            {
                LogDebugFunc("BTL_GET_COUNTER called");
                
                var COUNTER = flowApi.GetIntArg(0);

                flowApi.SetReturnValue(BTL_COUNTERS[COUNTER]);

                return FlowStatus.SUCCESS;
            }, 0x0015);

            flowFramework.Register("BTL_SET_COUNTER", 2, () =>
            {
                LogDebugFunc("BTL_SET_COUNTER called");

                var COUNTER = flowApi.GetIntArg(0);
                var value = flowApi.GetIntArg(1);

                BTL_COUNTERS[COUNTER] = value;

                return FlowStatus.SUCCESS;
            }, 0x0016);

            flowFramework.Register("SET_HUMAN_LV", 2, () =>
            {
                LogDebugFunc("SET_HUMAN_LV called");

                var unitID = flowApi.GetIntArg(0);

                if (unitID != 1) return FlowStatus.SUCCESS;

                var targetLV = flowApi.GetIntArg(1);

                if (targetLV < 0 || targetLV > 100) return FlowStatus.SUCCESS;

                datUnit* Joker = _gameFunctions.getDatUnitFromPlayerID(1);

                Joker->Joker_Lv = (ushort)targetLV;
                Joker->Joker_EXP = JokerExpValues[targetLV];

                return FlowStatus.SUCCESS;
            }, 0x00D6);

            flowFramework.Register("FLAG_DATA_INPUT", 3, () =>
            {
                LogDebugFunc("FLAG_DATA_INPUT called");

                int majorID = flowApi.GetIntArg(0);
                int minorID = flowApi.GetIntArg(1);

                string flagData = $"debug/flagdata/flg_data_{majorID:D2}_{minorID:D2}.bin";
                var filename = Marshal.StringToHGlobalAnsi(flagData);

                Log($"loading debug flag data file {flagData}");

                fileHandleStruct* flagFile = _gameFunctions.file_open_simple((char*)filename)->ptrtoStruct;
                int fileStatus = _gameFunctions.fsSync(flagFile);

                while (fileStatus != 1)
                {
                    fileStatus = _gameFunctions.fsSync(flagFile);
                }

                Marshal.FreeHGlobal(filename);

                DebugFlagData* FlagData = (DebugFlagData*)flagFile->pointerToFile;

                for (int i = 0; i < 12800; i++)
                {
                    _gameFunctions.BIT_SET(i, FlagData->FlagStatus[i]);
                }

                return FlowStatus.SUCCESS;
            }, 0x01a1);

            flowFramework.Register("FLD_START_DEBUG_SCRIPT", 2, () =>
            {
                LogDebugFunc("FLD_START_DEBUG_SCRIPT called");

                int majorID = flowApi.GetIntArg(0);
                int minorID = flowApi.GetIntArg(1);

                string debugFile = $"debug/script/dbgscr_{majorID:D3}_{minorID:D3}.bf";

                Log($"loading debug script {debugFile}");

                var filename = Marshal.StringToHGlobalAnsi(debugFile);

                fileHandleStruct* targetBF = _gameFunctions.file_open_simple((char*)filename)->ptrtoStruct;
                int fileStatus = _gameFunctions.fsSync(targetBF);

                while (fileStatus != 1)
                {
                    fileStatus = _gameFunctions.fsSync(targetBF);
                }

                Marshal.FreeHGlobal(filename);

                _gameFunctions.scrRunScript(0, targetBF->pointerToFile, (nint)targetBF->bufferSize, 0);

                return FlowStatus.SUCCESS;
            }, 0x11f1);

            flowFramework.Register("CALL_NAVI_DIALOGUE", 3, () =>
            {
                LogDebugFunc("CALL_NAVI_DIALOGUE called");

                int charID = flowApi.GetIntArg(0);
                int expressionID = flowApi.GetIntArg(1);
                int messageIndex = flowApi.GetIntArg(2);

                int targetBustup = _gameFunctions.GetAssistBustupID(charID, expressionID);

                if (targetBustup > -1 && messageIndex > 0)
                {
                    _gameFunctions.CallNaviDialogueImpl(targetBustup, messageIndex);
                }

                return FlowStatus.SUCCESS;
            }, 0x20d0);

            // custom flow functions

            flowFramework.Register("GET_MAXBULLETS", 1, () =>
            {
                LogDebugFunc("GET_MAXBULLETS called");

                var partyMember = flowApi.GetIntArg(0);

                bool isValid = partyMember >= 1 && partyMember <= 10;
                if (!isValid)
                {
                    flowApi.SetReturnValue(0);
                    return FlowStatus.SUCCESS;
                }

                datUnit* PartyMember = _gameFunctions.getDatUnitFromPlayerID((ushort)partyMember);
                var currentBullets = PartyMember->numOfBullets;

                p5rLib.FlowCaller.BULLET_RECOVERY(partyMember);

                flowApi.SetReturnValue(PartyMember->numOfBullets);

                memory.Write<ushort>((nuint)(&PartyMember->numOfBullets), currentBullets);

                return FlowStatus.SUCCESS;
            });


            flowFramework.Register("GET_BULLETS", 1, () =>
            {
                LogDebugFunc("GET_BULLETS called");

                var partyMember = flowApi.GetIntArg(0);

                bool isValid = partyMember >= 1 && partyMember <= 10;
                if (!isValid)
                {
                    flowApi.SetReturnValue(0);
                    return FlowStatus.SUCCESS;
                }

                datUnit* PartyMember = _gameFunctions.getDatUnitFromPlayerID((ushort)partyMember);
                var currentBullets = PartyMember->numOfBullets;

                flowApi.SetReturnValue(currentBullets);

                return FlowStatus.SUCCESS;
            });


            flowFramework.Register("SET_BULLETS", 2, () =>
            {
                LogDebugFunc("SET_BULLETS called");

                var partyMember = flowApi.GetIntArg(0);
                var newBullets = (ushort)flowApi.GetIntArg(1);

                bool isValid = partyMember >= 1 && partyMember <= 10;
                if (!isValid)
                {
                    return FlowStatus.SUCCESS;
                }

                datUnit* PartyMember = _gameFunctions.getDatUnitFromPlayerID((ushort)partyMember);
                memory.Write<ushort>((nuint)(&PartyMember->numOfBullets), newBullets);

                return FlowStatus.SUCCESS;
            });


            flowFramework.Register("GET_TACTIC", 1, () =>
            {
                LogDebugFunc("GET_TACTIC called");
                var partyMember = flowApi.GetIntArg(0);

                bool isValid = partyMember >= 1 && partyMember <= 10;
                if (!isValid)
                {
                    flowApi.SetReturnValue(0);
                    return FlowStatus.SUCCESS;
                }

                datUnit* PartyMember = _gameFunctions.getDatUnitFromPlayerID((ushort)partyMember);
                var currentTactics = PartyMember->TacticsState;

                flowApi.SetReturnValue(currentTactics);

                return FlowStatus.SUCCESS;
            });


            flowFramework.Register("SET_TACTIC", 2, () =>
            {
                LogDebugFunc("SET_TACTIC called");
                var partyMember = flowApi.GetIntArg(0);
                var newTactics = (ushort)flowApi.GetIntArg(1);

                bool isValid = partyMember >= 1 && partyMember <= 10;
                if (!isValid)
                {
                    return FlowStatus.SUCCESS;
                }

                datUnit* PartyMember = _gameFunctions.getDatUnitFromPlayerID((ushort)partyMember);
                memory.Write<ushort>((nuint)(&PartyMember->TacticsState), newTactics);

                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_FLAGS", 3, () =>
            {
                LogDebugFunc("SET_UNIT_FLAGS called");
                int unitID = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                bool onOff = flowApi.GetIntArg(2) > 0;

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<int>((nuint)(&currUnit->Flags), ToggleBit(currUnit->Flags, toggleFlag, onOff));

                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_FLAGS", 2, () =>
            {
                LogDebugFunc("GET_UNIT_FLAGS called");
                int unitID = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);
                int isSet = IsBitSet(currUnit->Flags, toggleFlag);
                flowApi.SetReturnValue(isSet);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_ITEM_DROP_ITEM", 3, () =>
            {
                LogDebugFunc("SET_UNIT_ITEM_DROP_ITEM called");
                int unitID = flowApi.GetIntArg(0);
                int ItemDropSlot = flowApi.GetIntArg(1);
                int ItemID = flowApi.GetIntArg(2);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);
                memory.Write<short>((nuint)(&currUnit->ItemDrops[ItemDropSlot*2]), (short)ItemID);

                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_ITEM_DROP_PROBABILITY", 3, () =>
            {
                LogDebugFunc("SET_UNIT_ITEM_DROP_PROBABILITY called");
                int unitID = flowApi.GetIntArg(0);
                int ItemDropSlot = flowApi.GetIntArg(1);
                int itemProbability = flowApi.GetIntArg(2);
                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);
                memory.Write<short>((nuint)(&currUnit->ItemDrops[ItemDropSlot * 2 + 1]), (short)itemProbability);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_ITEM_DROP_ITEM", 2, () =>
            {
                LogDebugFunc("GET_UNIT_ITEM_DROP_ITEM called");
                int unitID = flowApi.GetIntArg(0);
                int ItemDropSlot = flowApi.GetIntArg(1);
                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                int itemID = memory.Read<short>((nuint)(&currUnit->ItemDrops[ItemDropSlot * 2]));
                flowApi.SetReturnValue(itemID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_ITEM_DROP_PROBABILITY", 2, () =>
            {
                LogDebugFunc("GET_UNIT_ITEM_DROP_PROBABILITY called");
                int unitID = flowApi.GetIntArg(0);
                int ItemDropSlot = flowApi.GetIntArg(1);
                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);
                int itemProbability = memory.Read<short>((nuint)(&currUnit->ItemDrops[ItemDropSlot * 2 + 1]));
                flowApi.SetReturnValue(itemProbability);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_EVT_ITEM", 3, () =>
            {
                LogDebugFunc("SET_UNIT_EVT_ITEM called");
                int unitID = flowApi.GetIntArg(0);
                int ItemSlot = flowApi.GetIntArg(1);
                int ItemID = flowApi.GetIntArg(2);
                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);
                memory.Write<short>((nuint)(&currUnit->EvtItemDrop[ItemSlot]), (short)ItemID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_EVT_ITEM", 2, () =>
            {
                LogDebugFunc("GET_UNIT_EVT_ITEM called");
                int unitID = flowApi.GetIntArg(0);
                int ItemSlot = flowApi.GetIntArg(1);
                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);
                int itemID = memory.Read<short>((nuint)(&currUnit->EvtItemDrop[ItemSlot]));
                flowApi.SetReturnValue(itemID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_STATUS_EFFECT", 3, () =>
            {
                LogDebugFunc("SET_STATUS_EFFECT called");
                var partyMember = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                bool onOff = flowApi.GetIntArg(2) > 0;

                bool isValid = partyMember >= 1 && partyMember <= 10;
                if (!isValid)
                {
                    return FlowStatus.SUCCESS;
                }

                datUnit* PartyMember = _gameFunctions.getDatUnitFromPlayerID((ushort)partyMember);
                memory.Write<int>((nuint)(&PartyMember->StatusAilments), ToggleBit((int)PartyMember->StatusAilments, toggleFlag, onOff));

                return FlowStatus.SUCCESS;
            });

            // starting reference point: https://github.com/DeathChaos25/ps3-ckit/blob/main/prx/modules/p5/EXFLW/EXFLW.c#L1432

            flowFramework.Register("SET_UNIT_ARCANA", 2, () =>
            {
                LogDebugFunc("SET_UNIT_ARCANA called");
                int unitID = flowApi.GetIntArg(0);
                byte arcana = (byte)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->Arcana), arcana);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_ARCANA", 1, () =>
            {
                LogDebugFunc("GET_UNIT_ARCANA called");
                int unitID = flowApi.GetIntArg(0);
                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);
                flowApi.SetReturnValue(currUnit->Arcana);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_LEVEL", 2, () =>
            {
                LogDebugFunc("SET_UNIT_LEVEL called");
                int unitID = flowApi.GetIntArg(0);
                short tarLv = (short)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<short>((nuint)(&currUnit->UnitLv), tarLv);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_LEVEL", 1, () =>
            {
                LogDebugFunc("GET_UNIT_LEVEL called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->UnitLv);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_HP", 2, () =>
            {
                LogDebugFunc("SET_UNIT_HP called");
                int unitID = flowApi.GetIntArg(0);
                int targetHP = flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<int>((nuint)(&currUnit->HP), targetHP);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_HP", 1, () =>
            {
                LogDebugFunc("GET_UNIT_HP called");
                int unitID = flowApi.GetIntArg(0);


                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->HP);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_SP", 2, () =>
            {
                LogDebugFunc("SET_UNIT_SP called");
                int unitID = flowApi.GetIntArg(0);
                int targetSP = flowApi.GetIntArg(1);


                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<int>((nuint)(&currUnit->SP), targetSP);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_SP", 1, () =>
            {
                LogDebugFunc("GET_UNIT_SP called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->SP);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_STATS", 3, () =>
            {
                LogDebugFunc("SET_UNIT_STATS called");
                int unitID = flowApi.GetIntArg(0);
                int stat_type = flowApi.GetIntArg(1);
                byte stat_value = (byte)flowApi.GetIntArg(2);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->Stats[stat_type]), stat_value);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_STATS", 2, () =>
            {
                LogDebugFunc("GET_UNIT_STATS called");
                int unitID = flowApi.GetIntArg(0);
                int stat_type = flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->Stats[stat_type]);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_SKILL", 3, () =>
            {
                LogDebugFunc("SET_UNIT_SKILL called");
                int unitID = flowApi.GetIntArg(0);
                int skill_slot = flowApi.GetIntArg(1);
                short skill_id = (short)flowApi.GetIntArg(2);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<short>((nuint)(&currUnit->BattleSkills[skill_slot]), skill_id);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_SKILL", 2, () =>
            {
                LogDebugFunc("GET_UNIT_SKILL called");
                int unitID = flowApi.GetIntArg(0);
                int skill_slot = flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->BattleSkills[skill_slot]);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_REWARD_EXP", 2, () =>
            {
                LogDebugFunc("SET_UNIT_REWARD_EXP called");
                int unitID = flowApi.GetIntArg(0);
                short exp = (short)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<short>((nuint)(&currUnit->ExpReward), exp);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_REWARD_EXP", 1, () =>
            {
                LogDebugFunc("GET_UNIT_REWARD_EXP called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->ExpReward);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_REWARD_MONEY", 2, () =>
            {
                LogDebugFunc("SET_UNIT_REWARD_MONEY called");
                int unitID = flowApi.GetIntArg(0);
                int money = flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<int>((nuint)(&currUnit->MoneyReward), money);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_REWARD_MONEY", 1, () =>
            {
                LogDebugFunc("GET_UNIT_REWARD_MONEY called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->MoneyReward);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_ATTACK_ELEMENT", 2, () =>
            {
                LogDebugFunc("SET_UNIT_ATTACK_ELEMENT called");
                int unitID = flowApi.GetIntArg(0);
                byte element = (byte)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->AtkAttribute), element);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_ATTACK_ELEMENT", 1, () =>
            {
                LogDebugFunc("GET_UNIT_ATTACK_ELEMENT called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->AtkAttribute);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_ATTACK_ACCURACY", 2, () =>
            {
                LogDebugFunc("SET_UNIT_ATTACK_ACCURACY called");
                int unitID = flowApi.GetIntArg(0);
                byte acc = (byte)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->AtkAccuracy), acc);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_ATTACK_ACCURACY", 1, () =>
            {
                LogDebugFunc("GET_UNIT_ATTACK_ACCURACY called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->AtkAccuracy);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_ATTACK_DAMAGE", 2, () =>
            {
                LogDebugFunc("SET_UNIT_ATTACK_DAMAGE called");
                int unitID = flowApi.GetIntArg(0);
                short dmg = (short)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<short>((nuint)(&currUnit->AtkDamage), dmg);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_ATTACK_DAMAGE", 1, () =>
            {
                LogDebugFunc("GET_UNIT_ATTACK_DAMAGE called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->AtkDamage);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_PERSONA_MASK", 2, () =>
            {
                LogDebugFunc("SET_UNIT_PERSONA_MASK called");
                int unitID = flowApi.GetIntArg(0);
                short personaID = (short)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(UNIT_TBL_Section4_PTR + (long)(unitID * UNIT_TBL_Section4_EntrySize));
                memory.Write<short>(targetPTR, personaID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_PERSONA_MASK", 1, () =>
            {
                LogDebugFunc("GET_UNIT_PERSONA_MASK called");
                int unitID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(UNIT_TBL_Section4_PTR + (long)(unitID * UNIT_TBL_Section4_EntrySize));
                short personaID = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(personaID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_MODEL", 2, () =>
            {
                LogDebugFunc("SET_UNIT_MODEL called");
                int unitID = flowApi.GetIntArg(0);
                byte modelID = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(UNIT_TBL_Section4_PTR + (long)(unitID * UNIT_TBL_Section4_EntrySize + 2));
                memory.Write<byte>(targetPTR, modelID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_MODEL", 1, () =>
            {
                LogDebugFunc("GET_UNIT_MODEL called");
                int unitID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(UNIT_TBL_Section4_PTR + (long)(unitID * UNIT_TBL_Section4_EntrySize + 2));
                byte modelID = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(modelID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_AI", 2, () =>
            {
                LogDebugFunc("SET_UNIT_AI called");
                int unitID = flowApi.GetIntArg(0);
                short ai_ID = (short)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(ELSAI_TBL_Section0_PTR + (long)(unitID * ELSAI_TBL_Section0_EntrySize + 2));
                memory.Write<short>(targetPTR, ai_ID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_AI", 1, () =>
            {
                LogDebugFunc("GET_UNIT_AI called");
                int unitID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(ELSAI_TBL_Section0_PTR + (long)(unitID * ELSAI_TBL_Section0_EntrySize + 2));
                short ai_ID = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(ai_ID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_ENCOUNT_ENEMY", 3, () =>
            {
                LogDebugFunc("SET_ENCOUNT_ENEMY called");
                int encount_ID = flowApi.GetIntArg(0);
                int enemySlot = flowApi.GetIntArg(1);
                int enemyID = flowApi.GetIntArg(2);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);
                memory.Write<ushort>((nuint)(&currEnc->BattleUnitID[enemySlot]), (ushort)enemyID);

                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_ENCOUNT_ENEMY", 2, () =>
            {
                LogDebugFunc("GET_ENCOUNT_ENEMY called");
                int encount_ID = flowApi.GetIntArg(0);
                int enemySlot = flowApi.GetIntArg(1);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                flowApi.SetReturnValue(memory.Read<ushort>((nuint)(&currEnc->BattleUnitID[enemySlot])));
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_ENCOUNT_FIELD", 3, () =>
            {
                LogDebugFunc("SET_ENCOUNT_FIELD called");
                int encount_ID = flowApi.GetIntArg(0);
                int majorID = flowApi.GetIntArg(1);
                int minorID = flowApi.GetIntArg(2);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);
                memory.Write<ushort>((nuint)(&currEnc->FieldID), (ushort)majorID);
                memory.Write<ushort>((nuint)(&currEnc->RoomID), (ushort)minorID);

                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_ENCOUNT_FIELD_MAJOR", 1, () =>
            {
                LogDebugFunc("GET_ENCOUNT_FIELD_MAJOR called");
                int encount_ID = flowApi.GetIntArg(0);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                flowApi.SetReturnValue(memory.Read<ushort>((nuint)(&currEnc->FieldID)));
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_ENCOUNT_FIELD_MINOR", 1, () =>
            {
                LogDebugFunc("GET_ENCOUNT_FIELD_MINOR called");
                int encount_ID = flowApi.GetIntArg(0);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                flowApi.SetReturnValue(memory.Read<ushort>((nuint)(&currEnc->RoomID)));
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_ENCOUNT_BGM", 2, () =>
            {
                LogDebugFunc("SET_ENCOUNT_BGM called");
                int encount_ID = flowApi.GetIntArg(0);
                int bgmID = flowApi.GetIntArg(1);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);
                memory.Write<ushort>((nuint)(&currEnc->BGMID), (ushort)bgmID);

                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_ENCOUNT_BGM", 1, () =>
            {
                LogDebugFunc("GET_ENCOUNT_BGM called");
                int encount_ID = flowApi.GetIntArg(0);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                flowApi.SetReturnValue(memory.Read<ushort>((nuint)(&currEnc->BGMID)));
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_AFFINITY", 3, () =>
            {
                LogDebugFunc("SET_UNIT_AFFINITY called");
                int unitID = flowApi.GetIntArg(0);
                int affinity_slot = flowApi.GetIntArg(1);
                short new_affinity = (short)flowApi.GetIntArg(2);
                nuint targetPTR = (nuint)(UNIT_TBL_Section1_PTR + (long)(unitID * UNIT_TBL_Section1_EntrySize) + (long)(affinity_slot * 2));
                memory.Write<short>(targetPTR, new_affinity);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_AFFINITY", 2, () =>
            {
                LogDebugFunc("GET_UNIT_AFFINITY called");
                int unitID = flowApi.GetIntArg(0);
                int affinity_slot = flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(UNIT_TBL_Section1_PTR + (long)(unitID * UNIT_TBL_Section1_EntrySize) + (long)(affinity_slot * 2));
                short affinity_value = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(affinity_value);
                return FlowStatus.SUCCESS;
            });

            //
            // reference: https://github.com/tge-was-taken/010-Editor-Templates/blob/master/templates/p5r_tbl.bt
            //

            flowFramework.Register("SET_SKILL_ELEMENT", 2, () =>
            {
                LogDebugFunc("SET_SKILL_ELEMENT called");
                int skill_ID = flowApi.GetIntArg(0);
                byte element = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize));
                memory.Write<byte>(targetPTR, element);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_ELEMENT", 1, () =>
            {
                LogDebugFunc("GET_SKILL_ELEMENT called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize));
                byte element = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(element);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_PASSIVE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_PASSIVE called");
                int skill_ID = flowApi.GetIntArg(0);
                byte isPassive = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize + 1));
                memory.Write<byte>(targetPTR, isPassive);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_PASSIVE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_PASSIVE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize + 1));
                byte isPassive = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(isPassive);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_INHERITABLE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_INHERITABLE called");
                int skill_ID = flowApi.GetIntArg(0);
                byte isInherit = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize + 2));
                memory.Write<byte>(targetPTR, isInherit);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_INHERITABLE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_INHERITABLE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize + 2));
                byte isInherit = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(isInherit);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_AREA_USABLE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_AREA_USABLE called");
                int skill_ID = flowApi.GetIntArg(0);
                byte area_val = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 5));
                memory.Write<byte>(targetPTR, area_val);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_AREA_USABLE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_AREA_USABLE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 5));
                byte area_val = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(area_val);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_DAMAGE_STAT", 2, () =>
            {
                LogDebugFunc("SET_SKILL_DAMAGE_STAT called");
                int skill_ID = flowApi.GetIntArg(0);
                byte stat_type = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 6));
                memory.Write<byte>(targetPTR, stat_type);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_DAMAGE_STAT", 1, () =>
            {
                LogDebugFunc("GET_SKILL_DAMAGE_STAT called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 6));
                byte stat_type = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(stat_type);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_COST_TYPE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_COST_TYPE called");
                int skill_ID = flowApi.GetIntArg(0);
                byte cost_type = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 7));
                memory.Write<byte>(targetPTR, cost_type);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_COST_TYPE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_COST_TYPE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 7));
                byte cost_type = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(cost_type);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_COST", 2, () =>
            {
                LogDebugFunc("SET_SKILL_COST called");
                int skill_ID = flowApi.GetIntArg(0);
                short cost = (short)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 8));
                memory.Write<short>(targetPTR, cost);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_COST", 1, () =>
            {
                LogDebugFunc("GET_SKILL_COST called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 8));
                short cost = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(cost);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_TARGETING_TYPE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_TARGETING_TYPE called");
                int skill_ID = flowApi.GetIntArg(0);
                byte tar_type = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 12));
                memory.Write<byte>(targetPTR, tar_type);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_TARGETING_TYPE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_TARGETING_TYPE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 12));
                byte tar_type = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(tar_type);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_TARGETS", 2, () =>
            {
                LogDebugFunc("SET_SKILL_TARGETS called");
                int skill_ID = flowApi.GetIntArg(0);
                byte tar = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 13));
                memory.Write<byte>(targetPTR, tar);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_TARGETS", 1, () =>
            {
                LogDebugFunc("GET_SKILL_TARGETS called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 13));
                byte tar = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(tar);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_ACCURACY", 2, () =>
            {
                LogDebugFunc("SET_SKILL_ACCURACY called");
                int skill_ID = flowApi.GetIntArg(0);
                byte acc = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 20));
                memory.Write<byte>(targetPTR, acc);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_ACCURACY", 1, () =>
            {
                LogDebugFunc("GET_SKILL_ACCURACY called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 20));
                byte acc = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(acc);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_MIN_HITS", 2, () =>
            {
                LogDebugFunc("SET_SKILL_MIN_HITS called");
                int skill_ID = flowApi.GetIntArg(0);
                byte min_hits_amt = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 21));
                memory.Write<byte>(targetPTR, min_hits_amt);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_MIN_HITS", 1, () =>
            {
                LogDebugFunc("GET_SKILL_MIN_HITS called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 21));
                byte min_hits_amt = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(min_hits_amt);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_MAX_HITS", 2, () =>
            {
                LogDebugFunc("SET_SKILL_MAX_HITS called");
                int skill_ID = flowApi.GetIntArg(0);
                byte max_hits_amt = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 22));
                memory.Write<byte>(targetPTR, max_hits_amt);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_MAX_HITS", 1, () =>
            {
                LogDebugFunc("GET_SKILL_MAX_HITS called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 22));
                byte max_hits_amt = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(max_hits_amt);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_DAMAGE_HEALING_TYPE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_DAMAGE_HEALING_TYPE called");
                int skill_ID = flowApi.GetIntArg(0);
                byte val = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 23));
                memory.Write<byte>(targetPTR, val);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_DAMAGE_HEALING_TYPE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_DAMAGE_HEALING_TYPE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 23));
                byte val = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(val);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_DAMAGE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_DAMAGE called");
                int skill_ID = flowApi.GetIntArg(0);
                short dmg = (short)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 24));
                memory.Write<short>(targetPTR, dmg);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_DAMAGE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_DAMAGE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 24));
                short dmg = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(dmg);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_DEPLETE_SP", 2, () =>
            {
                LogDebugFunc("SET_SKILL_DEPLETE_SP called");
                int skill_ID = flowApi.GetIntArg(0);
                byte SP = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 26));
                memory.Write<byte>(targetPTR, SP);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_DEPLETE_SP", 1, () =>
            {
                LogDebugFunc("GET_SKILL_DEPLETE_SP called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 26));
                byte SP = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(SP);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_CRIT_CHANCE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_CRIT_CHANCE called");
                int skill_ID = flowApi.GetIntArg(0);
                byte crit_chance = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 45));
                memory.Write<byte>(targetPTR, crit_chance);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_CRIT_CHANCE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_CRIT_CHANCE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 45));
                byte crit_chance = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(crit_chance);
                return FlowStatus.SUCCESS;
            });


            // https://i.imgur.com/Cm13zBQ.gif my reaction to this function
            flowFramework.Register("UNIT_CLEAR_ANALYSIS", 1, () =>
            {
                LogDebugFunc("UNIT_CLEAR_ANALYSIS called");
                int enemy_ID = flowApi.GetIntArg(0);

                for (int bitIndex = 0; bitIndex < 11; bitIndex++)
                {
                    WriteEnemyBit(ENEMY_ANALYSIS_DATA_PTR, enemy_ID, bitIndex, false);
                }

                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_ENCOUNT_FLAG", 3, () =>
            {
                LogDebugFunc("SET_ENCOUNT_FLAG called");
                int encount_ID = flowApi.GetIntArg(0);
                int toggleFlag = flowApi.GetIntArg(1);
                bool onOff = flowApi.GetIntArg(2) > 0;

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                memory.Write<int>((nuint)(&currEnc->flags), ToggleBit((int)currEnc->flags, toggleFlag, onOff));
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_ENCOUNT_FLAG", 2, () =>
            {
                LogDebugFunc("GET_ENCOUNT_FLAG called");
                int encount_ID = flowApi.GetIntArg(0);
                int toggleFlag = flowApi.GetIntArg(1);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                int flags = memory.Read<int>((nuint)(&currEnc->flags));

                flowApi.SetReturnValue(IsBitSet(flags, toggleFlag));
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_BATTLE_FLAG", 3, () =>
            {
                LogDebugFunc("SET_BATTLE_FLAG called");
                var partyMember = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                bool onOff = flowApi.GetIntArg(2) > 0;

                bool isValid = partyMember >= 1 && partyMember <= 10;
                if (!isValid)
                {
                    return FlowStatus.SUCCESS;
                }

                datUnit* PartyMember = _gameFunctions.getDatUnitFromPlayerID((ushort)partyMember);
                memory.Write<int>((nuint)(&PartyMember->Flags), ToggleBit((int)PartyMember->Flags, toggleFlag, onOff));

                return FlowStatus.SUCCESS;
            }, 0x2550);

            flowFramework.Register("GET_BATTLE_FLAG", 2, () =>
            {
                LogDebugFunc("GET_BATTLE_FLAG called");
                var partyMember = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                bool isValid = partyMember >= 1 && partyMember <= 10;
                if (!isValid)
                {
                    flowApi.SetReturnValue(0);
                    return FlowStatus.SUCCESS;
                }
                datUnit* PartyMember = _gameFunctions.getDatUnitFromPlayerID((ushort)partyMember);
                int flags = memory.Read<int>((nuint)(&PartyMember->Flags));
                flowApi.SetReturnValue(IsBitSet(flags, toggleFlag));
                return FlowStatus.SUCCESS;
            }, 0x2551);

            flowFramework.Register("SET_SKILL_EFFECT_TYPE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_EFFECT_TYPE called");
                int skill_ID = flowApi.GetIntArg(0);
                byte eff_type = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 30));
                memory.Write<byte>(targetPTR, eff_type);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_EFFECT_TYPE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_EFFECT_TYPE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 30));
                byte eff_type = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(eff_type);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_EFFECT_CHANCE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_EFFECT_CHANCE called");
                int skill_ID = flowApi.GetIntArg(0);
                byte eff_chance = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 31));
                memory.Write<byte>(targetPTR, eff_chance);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_EFFECT_CHANCE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_EFFECT_CHANCE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 31));
                byte eff_chance = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(eff_chance);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_EFFECT_EXTRA", 2, () =>
            {
                LogDebugFunc("SET_SKILL_EFFECT_EXTRA called");
                int skill_ID = flowApi.GetIntArg(0);
                byte eff_duration = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 44));
                memory.Write<byte>(targetPTR, eff_duration);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_EFFECT_EXTRA", 1, () =>
            {
                LogDebugFunc("GET_SKILL_EFFECT_EXTRA called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 44));
                byte eff_duration = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(eff_duration);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_EFFECT", 3, () =>
            {
                LogDebugFunc("SET_SKILL_EFFECT called");
                int skill_ID = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                bool onOff = flowApi.GetIntArg(2) > 0;

                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 32));
                long eff_bitfield = memory.Read<long>(targetPTR);
                memory.Write<long>(targetPTR, ToggleBit(eff_bitfield, toggleFlag, onOff));
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_EFFECT", 2, () =>
            {
                LogDebugFunc("GET_SKILL_EFFECT called");
                int skill_ID = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 32));
                long eff_bitfield = memory.Read<long>(targetPTR);
                flowApi.SetReturnValue(IsBitSet(eff_bitfield, toggleFlag));
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("AI_SET_FLAG", 2, () =>
            {
                LogDebugFunc("AI_SET_FLAG called");
                bool isValid = (nint)GetDatUnitFromCurrentAI() > 0;
                if (!isValid)
                {
                    return FlowStatus.SUCCESS;
                }

                var toggleFlag = flowApi.GetIntArg(0);
                bool onOff = flowApi.GetIntArg(1) > 0;

                datUnit* currEnemy = GetDatUnitFromCurrentAI();
                memory.Write<int>((nuint)(&currEnemy->Flags), ToggleBit((int)currEnemy->Flags, toggleFlag, onOff));

                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("AI_GET_FLAG", 1, () =>
            {
                LogDebugFunc("AI_GET_FLAG called");
                bool isValid = (nint)GetDatUnitFromCurrentAI() > 0;
                if (!isValid)
                {
                    flowApi.SetReturnValue(0);
                    return FlowStatus.SUCCESS;
                }
                var toggleFlag = flowApi.GetIntArg(0);
                datUnit* currEnemy = GetDatUnitFromCurrentAI();
                int flags = (int)currEnemy->Flags;
                flowApi.SetReturnValue(IsBitSet(flags, toggleFlag));
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("AI_SET_LV", 1, () =>
            {
                LogDebugFunc("AI_SET_LV called");
                bool isValid = (nint)GetDatUnitFromCurrentAI() > 0;
                if (!isValid)
                {
                    return FlowStatus.SUCCESS;
                }
                ushort targetLV = (ushort)flowApi.GetIntArg(0);
                datUnit* currEnemy = GetDatUnitFromCurrentAI();

                memory.Write<ushort>((nuint)(&currEnemy->Joker_Lv), targetLV);

                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("AI_GET_LV", 0, () =>
            {
                LogDebugFunc("AI_GET_LV called");
                bool isValid = (nint)GetDatUnitFromCurrentAI() > 0;
                if (!isValid)
                {
                    flowApi.SetReturnValue(0);
                    return FlowStatus.SUCCESS;
                }
                datUnit* currEnemy = GetDatUnitFromCurrentAI();
                ushort lv = memory.Read<ushort>((nuint)(&currEnemy->Joker_Lv));
                flowApi.SetReturnValue(lv);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_SKILL_JYOKYO_MESSAGE", 2, () =>
            {
                LogDebugFunc("SET_SKILL_JYOKYO_MESSAGE called");
                int skill_ID = flowApi.GetIntArg(0);
                short messID = (short)flowApi.GetIntArg(1);

                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 42)); // 0x2A

                memory.Write<short>(targetPTR, messID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_SKILL_JYOKYO_MESSAGE", 1, () =>
            {
                LogDebugFunc("GET_SKILL_JYOKYO_MESSAGE called");
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 42)); // 0x2A
                short messID = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(messID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_VOICE_ID", 2, () =>
            {
                LogDebugFunc("SET_UNIT_VOICE_ID called");
                int unitID = flowApi.GetIntArg(0);
                byte voiceID = (byte)flowApi.GetIntArg(1);

                var currUnit = GetUnitTBL_Segment3_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->VoiceID), voiceID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_VOICE_ID", 1, () =>
            {
                LogDebugFunc("GET_UNIT_VOICE_ID called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = GetUnitTBL_Segment3_Entry(unitID);

                flowApi.SetReturnValue(currUnit->VoiceID);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_TALK_PERSONALITY", 2, () =>
            {
                LogDebugFunc("SET_UNIT_TALK_PERSONALITY called");
                int unitID = flowApi.GetIntArg(0);
                byte talktype = (byte)flowApi.GetIntArg(1);

                var currUnit = GetUnitTBL_Segment3_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->TALK_PERSON), talktype);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_TALK_PERSONALITY", 1, () =>
            {
                LogDebugFunc("GET_UNIT_TALK_PERSONALITY called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = GetUnitTBL_Segment3_Entry(unitID);

                flowApi.SetReturnValue(currUnit->TALK_PERSON);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("SET_UNIT_VOICE_ABC", 2, () =>
            {
                LogDebugFunc("SET_UNIT_VOICE_ABC called");
                int unitID = flowApi.GetIntArg(0);
                byte abc = (byte)flowApi.GetIntArg(1);

                var currUnit = GetUnitTBL_Segment3_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->VoicePackABC), abc);
                return FlowStatus.SUCCESS;
            });

            flowFramework.Register("GET_UNIT_VOICE_ABC", 1, () =>
            {
                LogDebugFunc("GET_UNIT_VOICE_ABC called");
                int unitID = flowApi.GetIntArg(0);

                var currUnit = GetUnitTBL_Segment3_Entry(unitID);

                flowApi.SetReturnValue(currUnit->VoicePackABC);
                return FlowStatus.SUCCESS;
            });
        }

        public static unsafe int HookCalendarTransPlayKnifeSfx(CalendarTransStruct* a1)
        {
            if (a1 == null)
            {
                return _hook_calendar_trans_play_knife_sfx.OriginalFunction(a1);
            }

            int result = _hook_calendar_trans_play_knife_sfx.OriginalFunction(a1);

            if (a1->announceState == 2)
            {
                isDayAnnounced = false;
            }
            else if (a1->announceState == 3 && !isDayAnnounced)
            {
                if (a1->isCallingCard == 5)
                {
                    _gameFunctions.playSingleWordCue(51); // It's Showtime!
                }
                else
                {
                    int[] dayMapping = { 4, 5, 6, 0, 1, 2, 3 };
                    int Variation = _gameFunctions.RandomIntBetween(0, 1);
                    int DayOfWeek = _gameFunctions.GetTotalDays() % 7;
                    int mappedDayOfWeek = dayMapping[DayOfWeek];
                    int DayCue = 120 + Variation + (mappedDayOfWeek * 2);
                    _gameFunctions.playSingleWordCue(DayCue);
                }
                isDayAnnounced = true;
            }
            return result;
        }

        private const short MIN_CHAR_ID = 1;
        private const short MAX_CHAR_ID = 10;
        private const ushort PHANTOM_THIEF_DEFAULT = 51;
        private const ushort PHANTOM_THIEF_DEFAULT_DARK_SUIT = 52;

        public static unsafe int GetUnitModelId(ushort characterId, ushort modelId, ushort subID)
        {
            if (characterId < MIN_CHAR_ID || characterId > MAX_CHAR_ID || !_configuration._010_enableCutsceneOutfits || modelId < PHANTOM_THIEF_DEFAULT)
            {
                return _hookGetUnitModelID.OriginalFunction(characterId, modelId, subID);
            }

            for (int i = 0; i < 6; i++)
            {
                if (modelId == CharModelReplacementTable[characterId, i])
                {
                    ushort newId = (ushort)_hookGetUnitModelID.OriginalFunction(characterId, 50, 0);

                    if (newId != PHANTOM_THIEF_DEFAULT && newId != PHANTOM_THIEF_DEFAULT_DARK_SUIT && newId > 0)
                    {
                        return newId;
                    }
                    break;
                }
            }

            return _hookGetUnitModelID.OriginalFunction(characterId, modelId, subID);
        }

        public static unsafe nint LoadResourceImpl(
            ushort type, byte a2, ushort index, ushort major, byte minor, byte sub, short a7, nint a8, ushort a9, short a10)
        {
            // Log($"LoadResourceImpl: type {type}, a2 {a2}, index {index}, major {major}, minor {minor}, sub {sub}, a7 {a7}, a8 {a8}, a9 {a9}, a10 {a10}");
            if (type == 2 || type == 5)
            {
                if (major >= MIN_CHAR_ID && major <= MAX_CHAR_ID)
                {
                    if (minor == 48 && _configuration._011_randomTitle)
                    {
                        sub = rndTitle;
                    }
                    else if (minor >= 51 && _configuration._010_enableCutsceneOutfits)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (minor == CharModelReplacementTable[major, i])
                            {
                                ushort newId = (ushort)GetUnitModelId(major, 50, 0);
                                if (newId != 51 && newId != 52)
                                {
                                    minor = (byte)newId;
                                }
                            }
                        }
                    }
                }
            }

            return _hookLoadResourceImpl.OriginalFunction(type, a2, index, major, minor, sub, a7, a8, a9, a10);
        }

        public static nint SetTitleBGMFunc(TitleScreenStruct* a1)
        {
            nint result = _hookSetTitleBGM.OriginalFunction(a1);

            if (a1->PtrToSub1->StateID == 7)
            {
                var memory = Memory.Instance;

                int rndBGM = rndTitle;

                while (rndBGM == rndTitle)
                {
                    rndTitle = (byte)_gameFunctions.RandomIntBetween(0, 2);
                }

                if (rndBGM == 1)
                {
                    memory.SafeWrite((nuint)TitleBGMAddr, BitConverter.GetBytes((uint)101));              // Cue 101, P5 title screen
                }
                else if (rndBGM == 2)
                {
                    memory.SafeWrite((nuint)TitleBGMAddr, BitConverter.GetBytes((uint)10999));     // Cue 10999, P5S title screen
                }
                else
                {
                    memory.SafeWrite((nuint)TitleBGMAddr, BitConverter.GetBytes((uint)901)); // Cue 901, P5R title screen
                }
            }

            return result;
        }

        public static unsafe int UnitTurnStart(StructD* a1, nint a2, nint a3, nint a4)
        {
            if (a1 == null || a1->nextPTR == null || a1->nextPTR->datUnitPtr == null) return _hookUnitTurnStart.OriginalFunction(a1, a2, a3, a4);

            if (a1->nextPTR->datUnitPtr->unitType == 1)
            {
                uint targetColor = 0xFF0000FF;
                switch (a1->nextPTR->datUnitPtr->unitID)
                {
                    case 1:
                        targetColor = (uint)CombineBytes(_configuration._031_JokerColor_R, _configuration._031_JokerColor_G, _configuration._031_JokerColor_B);
                        break;
                    case 2:
                        targetColor = (uint)CombineBytes(_configuration._032_SkullColor_R, _configuration._032_SkullColor_G, _configuration._032_SkullColor_B);
                        break;
                    case 3:
                        targetColor = (uint)CombineBytes(_configuration._033_MonaColor_R, _configuration._033_MonaColor_G, _configuration._033_MonaColor_B);
                        break;
                    case 4:
                        targetColor = (uint)CombineBytes(_configuration._034_PantherColor_R, _configuration._034_PantherColor_G, _configuration._034_PantherColor_B);
                        break;
                    case 5:
                        targetColor = (uint)CombineBytes(_configuration._035_FoxColor_R, _configuration._035_FoxColor_G, _configuration._035_FoxColor_B);
                        break;
                    case 6:
                        targetColor = (uint)CombineBytes(_configuration._036_QueenColor_R, _configuration._036_QueenColor_G, _configuration._036_QueenColor_B);
                        break;
                    case 7:
                        targetColor = (uint)CombineBytes(_configuration._037_NoirColor_R, _configuration._037_NoirColor_G, _configuration._037_NoirColor_B);
                        break;
                    case 8:
                        targetColor = (uint)CombineBytes(_configuration._031_JokerColor_R, _configuration._031_JokerColor_G, _configuration._031_JokerColor_B);
                        break;
                    case 9:
                        if (_gameFunctions.BIT_CHK(0x40000110)) targetColor = (uint)CombineBytes(_configuration._039_CrowDarkColor_R, _configuration._039_CrowDarkColor_G, _configuration._039_CrowDarkColor_B);
                        else targetColor = (uint)CombineBytes(_configuration._038_CrowPrinceColor_R, _configuration._038_CrowPrinceColor_G, _configuration._038_CrowPrinceColor_B);
                        break;
                    case 10:
                        targetColor = (uint)CombineBytes(_configuration._040_VioletColor_R, _configuration._040_VioletColor_G, _configuration._040_VioletColor_B);
                        break;

                    default:
                        targetColor = 0xFF0000FF;
                        break;
                }

                var memory = Memory.Instance;
                memory.SafeWrite((nuint)PlayerOutlineColorAddr + 6, GetBytesBigEndian(targetColor));
            }

            return _hookUnitTurnStart.OriginalFunction(a1, a2, a3, a4);
        }

        public static unsafe void CheckShaderExists(nint a1)
        {
            if (a1 == 0)
            {
                Log("Warning: Using default shader as backup due to missing shader\n");
                return;
            }

            _hookCheckShaderExists.OriginalFunction(a1);
        }

        public static unsafe void checkHasEnemyDDSBossName(EnemyPersonaFunctionStruct3* a1)
        {
            int enemyID = 0;
            
            if (a1 != (EnemyPersonaFunctionStruct3*)0)
            {
                enemyID = (int)a1->datUnitPtr->unitID;
            }

            if (enemyID > 0)
            {
                int unitFlags = _gameFunctions.GetUnitTBL_Segment0_Entry(enemyID)->Flags;
                LogDebug($"DDS Has Boss Name function checked for enemy {a1->datUnitPtr->unitID} with unit tbl flags 0x{unitFlags:X8}\n");
                var memory = Memory.Instance;

                if (IsBitSet(unitFlags, 31) != 0)
                {
                    LogDebug($"DDS Boss Name patch applied for enemy {enemyID}\n");
                    var newEnemyID = _gameFunctions.GetUnitTBL_Segment0_Entry(enemyID)->EvtItemDrop[2];
                    if (newEnemyID > 0) enemyID = newEnemyID;

                    var FileString = $"battle/analyze/boss_name_{enemyID:d3}.dds";
                    var FileStringBytes = Marshal.StringToHGlobalAnsi(FileString);
                    nint result = _gameFunctions.somethingBossDDSFileOpen(FileStringBytes, 1, 0);
                    memory.Write<nint>((nuint)(&a1->Field38), result);
                    Marshal.FreeHGlobal(FileStringBytes);

                    return;
                }
                /*else
                {
                    LogDebug($"Enemy {enemyID} did not have custom boss dds flag, reverting\n");
                    memory.SafeWrite((nuint)EnemyIDDDSNamePatchAddress + 5, BitConverter.GetBytes((short)611));
                }*/
            }

            _hookcheckHasEnemyDDSBossName.OriginalFunction(a1);
        }

        public static unsafe void CombineColorBytes(nint a1, SoundBank_Struct* a2, nint a3)
        {
            a2->Field000 = (int)ByteswapUlong((uint)CombineBytes(_configuration._031_JokerColor_R, _configuration._031_JokerColor_G, _configuration._031_JokerColor_B));

            _hookCombineColorBytes.OriginalFunction(a1, a2, a3);
        }

        public unsafe bool AreBattleUnitsDead(Participate* a1)
        {
            bool jokerDead = isDatUnitDead(1);
            bool anyPartyAlive = !isDatUnitDead(_gameFunctions.GetPlayerIDFromPartySlot(1)) ||
                                 !isDatUnitDead(_gameFunctions.GetPlayerIDFromPartySlot(2)) ||
                                 !isDatUnitDead(_gameFunctions.GetPlayerIDFromPartySlot(3));

            bool result = _hookAreBattleUnitsDead.OriginalFunction(a1);

            if (isCurrentParticipateFromEnemy(a1))
            {
                if (GetNumberOfEnemyUnitsAlive(a1) == 0)
                {
                    if (jokerDead)
                    {
                        CheckJokerDeadAndRevive();
                    }
                    return true;
                }
            }
            else
            {
                if (jokerDead)
                {
                    if (anyPartyAlive)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public unsafe nint MarukiDetox(Participate* a1, nint a2)
        {
            nint result = _hookMarukiDetox.OriginalFunction(a1, a2);
            Log($"{GetNumberOfEnemyUnitsAlive(a1)} enemies alive in current encounter");
            return result;
        }

        public nint PlayerEscape(nint a1, nint a2)
        {
            CheckJokerDeadAndRevive();

            return _hookPlayerEscape.OriginalFunction(a1, a2);
        }


        static unsafe bool isDatUnitDead(ushort unitID)
        {
            if (unitID == 0) return true; // Invalid unit ID

            datUnit* unit = _gameFunctions.getDatUnitFromPlayerID(unitID);

            if (unit->currentHP <= 0 || IsBitSet(unit->StatusAilments, 19) > 0) return true;
            else return false;
        }

        public void CheckJokerDeadAndRevive()
        {
            var Joker = _gameFunctions.getDatUnitFromPlayerID(1);
            if (Joker->currentHP == 0)
            {
                Joker->currentHP = 1;
                Joker->StatusAilments = ToggleBit(Joker->StatusAilments, 19, false); // Clear the "Dead" bit
            }
        }

        static uint ByteswapUlong(uint value)
        {
            return ((value & 0x000000FF) << 24) |
                   ((value & 0x0000FF00) << 8) |
                   ((value & 0x00FF0000) >> 8) |
                   ((value & 0xFF000000) >> 24);
        }

        public static byte[] GetBytesBigEndian(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return bytes;
        }

        static int CombineBytes(byte R, byte G, byte B)
        {
            return (R << 24) | (G << 16) | (B << 8) | 255;
        }

        static int ToggleBit(int flags, int n, bool setBit)
        {
            if (n < 0 || n >= 32)
                throw new ArgumentOutOfRangeException(nameof(n), "Bit position must be between 0 and 31.");

            if (setBit)
            {
                return flags | (1 << n);
            }
            else
            {
                return flags & ~(1 << n);
            }
        }

        static long ToggleBit(long flags, int n, bool setBit) // 64 bits
        {
            if (n < 0 || n >= 64)
                throw new ArgumentOutOfRangeException(nameof(n), "Bit position must be between 0 and 63.");

            return setBit
                ? flags | (1L << n)    // Set bit
                : flags & ~(1L << n);   // Clear bit
        }

        static int IsBitSet(int flags, int n)
        {
            if (n < 0 || n >= 32)
                throw new ArgumentOutOfRangeException(nameof(n), "Bit position must be between 0 and 31.");

            return (flags & (1 << n)) != 0 ? 1 : 0;
        }

        static long IsBitSet(long flags, int n)
        {
            if (n < 0 || n >= 64)
                throw new ArgumentOutOfRangeException(nameof(n), "Bit position must be between 0 and 63.");

            return (flags & (1L << n)) != 0 ? 1 : 0;
        }


        public unsafe static datUnit* GetDatUnitFromCurrentAI()
        {
            ulong baseAddress = *(ulong*)CurrentAIBasePTR; 
            ulong addr = *(ulong*)(baseAddress + 0x260);

            if (addr == 0)
            {
                LogNoPrefix("CurrentAIBasePTR is null, returning null datUnit pointer\n");
                return (datUnit*)0;
            }

            CurrentAIStruct* currAI = (CurrentAIStruct*)addr;

            if (currAI->StructD_ptr == null || currAI->StructD_ptr->nextPTR == null || currAI->StructD_ptr->nextPTR->datUnitPtr == null)
            {
                return (datUnit*)0;
            }

            datUnit* currEnemy = currAI->StructD_ptr->nextPTR->datUnitPtr;
            // LogNoPrefix($"datUnit PTR for Current enemy is 0x{(nint)(&currEnemy):X8}\nEnemy ID / Level -> {currEnemy->unitID} / {currEnemy->Joker_Lv} \nHP/SP -> {currEnemy->currentHP}/{currEnemy->currentSP} \ndatUnit Flags -> 0x{currEnemy->Flags:X8}");
            return currEnemy;
        }

        public unsafe void ProcessEnemyUnits(Participate* a1)
        {
            if (a1 == null)
            {
                Log("Error: Participate pointer is null");
                return;
            }

            if (a1->field40.ptrToAI == null)
            {
                Log("Error: ptrToAI is null");
                return;
            }

            if (a1->field40.ptrToAI->PtrToPackage == null)
            {
                Log("Error: PtrToPackage is null");
                return;
            }

            PointerListgfw__SmartPointer_btl__Action* enemyUnits =
                &a1->field40.ptrToAI->PtrToPackage->enemyUnits;

            if (enemyUnits->first == null)
            {
                Log("No enemy units found in list");
                return;
            }

            PointerListEntry_gfw_SmartPointer_btlAction* current = enemyUnits->first;
            int unitCount = 0;

            while (current != null)
            {
                unitCount++;
                Log($"\nProcessing enemy unit #{unitCount}");

                if (&current->btlAction == null)
                {
                    LogNoPrefix("  Error: btlAction pointer is null");
                    current = current->next;
                    continue;
                }

                SmartPointer_btl__Action* action = &current->btlAction;

                if (action->participatePtr == null)
                {
                    LogNoPrefix("  Error: participatePtr is null");
                    current = current->next;
                    continue;
                }

                Participate* participate = action->participatePtr;

                if (&participate->field18 == null)
                {
                    LogNoPrefix("  Error: field18 pointer is null");
                    current = current->next;
                    continue;
                }

                SmartPointer_btl__Unit* unitPtr = &participate->field18;

                if (unitPtr->field18 == null)
                {
                    LogNoPrefix("  Error: Unit pointer is null");
                    current = current->next;
                    continue;
                }

                Unit* unit = unitPtr->field18;

                if (unit->datUnitPtr == null)
                {
                    LogNoPrefix("  Error: datUnitPtr is null");
                    current = current->next;
                    continue;
                }

                datUnit* datUnit = unit->datUnitPtr;

                LogNoPrefix($"  Unit Type: {datUnit->unitType}");
                LogNoPrefix($"  Unit ID: {datUnit->unitID}");

                current = current->next;
            }

            Log($"Finished processing {unitCount} enemy units");
        }

        public int GetNumberOfEnemyUnitsAlive(Participate* a1)
        {
            if (a1 == null || a1->field40.ptrToAI == null || a1->field40.ptrToAI->PtrToPackage == null)
            {
                Log("Error: Participate or AI package is null");
                return -1;
            }
            PointerListgfw__SmartPointer_btl__Action* enemyUnits = &a1->field40.ptrToAI->PtrToPackage->enemyUnits;
            if (enemyUnits->first == null)
            {
                Log("No enemy units found in list");
                return -1;
            }
            PointerListEntry_gfw_SmartPointer_btlAction* current = enemyUnits->first;
            int aliveCount = 0;
            while (current != null)
            {
                SmartPointer_btl__Action* action = &current->btlAction;
                if (action->participatePtr != null && action->participatePtr->field18.field18 != null)
                {
                    datUnit* datUnit = action->participatePtr->field18.field18->datUnitPtr;
                    if (datUnit != null && datUnit->currentHP > 0 && IsBitSet(datUnit->StatusAilments, 19) == 0)
                    {
                        aliveCount++;
                    }
                }
                current = current->next;
            }
            return aliveCount;
        }

        public unsafe datUnit* GetDatUnitOfTypeAndIDFromParticipate(Participate* a1, int unitType, int unitID)
        {
            if (a1 == null || a1->field40.ptrToAI == null || a1->field40.ptrToAI->PtrToPackage == null)
            {
                return null;
            }
            PointerListgfw__SmartPointer_btl__Action* allUnits = &a1->field40.ptrToAI->PtrToPackage->allUnits;
            if (allUnits->first == null)
            {
                return null;
            }
            PointerListEntry_gfw_SmartPointer_btlAction* current = allUnits->first;

            while (current != null)
            {
                SmartPointer_btl__Action* action = &current->btlAction;
                if (action->participatePtr != null && action->participatePtr->field18.field18 != null)
                {
                    datUnit* datUnit = action->participatePtr->field18.field18->datUnitPtr;
                    if (datUnit != null && datUnit->unitType == unitType && datUnit->unitID == unitID)
                    {
                        return datUnit;
                    }
                }
                current = current->next;
            }
            return null;
        }

        public unsafe bool isCurrentParticipateFromEnemy(Participate* a1)
        {
            if (a1 == null || a1->field18.field18 == null || a1->field18.field18->datUnitPtr == null)
            {
                return false;
            }
            
            return a1->field18.field18->datUnitPtr->unitType == 2;
        }


        //
        ///// I have no idea what the fuck I'm doing here, pray for all of our souls
        //
        public void WriteEnemyBit(nint baseAddress, int enemyId, int bitIndex, bool value)
        {
            var memory = Memory.Instance;

            int absoluteBitPos = (enemyId * 11) + bitIndex;
            int targetByteOffset = absoluteBitPos / 8;
            int bitInByte = absoluteBitPos % 8;

            byte currentByte = memory.Read<byte>((nuint)(baseAddress + targetByteOffset));

            if (value)
                currentByte |= (byte)(1 << bitInByte);  // on
            else
                currentByte &= (byte)~(1 << bitInByte); // off

            memory.Write<byte>((nuint)(baseAddress + targetByteOffset), currentByte);
        }

        public UnitTBL_Segment3* GetUnitTBL_Segment3_Entry(int enemyID)
        {
            nuint targetPTR = (nuint)(UNIT_TBL_Section3_PTR + (long)(enemyID * UNIT_TBL_Section3_EntrySize));
            return (UnitTBL_Segment3*)targetPTR;
        }
    }
}
