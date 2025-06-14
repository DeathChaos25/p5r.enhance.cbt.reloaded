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

        public static byte rndTitle = 0;

        private static nint TitleBGMAddr = 0;
        private static nint PlayerOutlineColorAddr = 0;

        private static readonly int[] BTL_COUNTERS = new int[512];

        private static nint UNIT_TBL_Section0_PTR;
        private static nint UNIT_TBL_Section1_PTR;
        private static nint UNIT_TBL_Section4_PTR;
        private static nint ELSAI_TBL_Section0_PTR;
        private static nint ENCOUNT_TBL_Section0_PTR;

        private static nint SKILL_TBL_Section0_PTR;
        private static nint SKILL_TBL_Section1_PTR;

        private static nint ENEMY_ANALYSIS_DATA_PTR;

        private static nint CurrentAIBasePTR;

        private static int UNIT_TBL_Section0_EntrySize = 68;    // 0x44
        private static int UNIT_TBL_Section1_EntrySize = 40;    // 0x28
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

            // v1.0.0 = 0x14163cb52
            SigScan("45 33 C0 48 8D 15 ?? ?? ?? ?? 66 0F 1F 44 ?? 00 0F B7 02 3B C1 74 ?? 41 FF C0 48 83 C2 02 41 81 F8 AB 00 00 00", "PSZ_check_2_Sig", address =>
            {
                byte[] Bytes = { 0xEB }; // // change jz to jmp

                memory.SafeWrite((nuint)address + 0x15, Bytes);
            });

            // v1.0.0 = 0x140ea3e15
            SigScan("E8 ?? ?? ?? ?? 84 C0 74 ?? C7 87 ?? ?? ?? ?? 01 00 00 00 B9 8A 00 00 00", "quick_evt_skip_enable_Sig", address =>
            {
                if (_configuration._020_QuickEventSkip)
                {
                    WriteReturn1(address);
                }
            });


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
                Log($"UNIT_TBL_Section0_PTR: {UNIT_TBL_Section0_PTR:X}");
            });

            // v1.0.4 = 0x140d7a8e9
            SigScan("48 8D 35 ?? ?? ?? ?? 0F C8 48 83 C3 04 4C 63 C0 48 8B D3 89 44 24 ?? 48 8B CE E8 ?? ?? ?? ?? 4C 63 44 24 ?? 41 8D 50 ?? 8B C2 25 0F 00 00 80 7D ?? FF C8 83 C8 F0 FF C0 F7 D8 4A 8D 3C ??", "UNIT_TBL_Section1_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                UNIT_TBL_Section1_PTR = (nint)funcAddress;
                Log($"UNIT_TBL_Section1_PTR: {UNIT_TBL_Section1_PTR:X}");
            });

            // v1.0.4 = 0x140a45b5f
            SigScan("48 8D 0D ?? ?? ?? ?? 0F B7 0C ?? B8 A0 00 00 00", "UNIT_TBL_Section4_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                UNIT_TBL_Section4_PTR = (nint)funcAddress;
                Log($"UNIT_TBL_Section4_PTR: {UNIT_TBL_Section4_PTR:X}");
            });

            // v1.0.4 = 0x14c322259
            SigScan("48 8D 35 ?? ?? ?? ?? 0F C8 48 83 C3 04 4C 63 C0 48 89 DA", "ELSAI_TBL_Section0_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                ELSAI_TBL_Section0_PTR = (nint)funcAddress;
                Log($"ELSAI_TBL_Section0_PTR: {ELSAI_TBL_Section0_PTR:X}");
            });

            // v1.0.4 = 0x141172149
            SigScan("48 03 15 ?? ?? ?? ?? 0F B7 0A", "ENCOUNT_TBL_Section0_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                ENCOUNT_TBL_Section0_PTR = (nint)funcAddress;
                Log($"ENCOUNT_TBL_Section0_PTR: {ENCOUNT_TBL_Section0_PTR:X}");
            });

            SigScan("48 8D 0D ?? ?? ?? ?? 80 7C ?? ?? 01", "SKILL_TBL_Section0_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                SKILL_TBL_Section0_PTR = (nint)funcAddress;
                Log($"SKILL_TBL_Section0_PTR: {SKILL_TBL_Section0_PTR:X}");
            });

            SigScan("48 8D 05 ?? ?? ?? ?? 80 7C ?? ?? 01", "SKILL_TBL_Section1_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                SKILL_TBL_Section1_PTR = (nint)funcAddress;
                Log($"SKILL_TBL_Section1_PTR: {SKILL_TBL_Section1_PTR:X}");
            });

            SigScan("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 0F B6 05 ?? ?? ?? ?? 48 8D 1D ?? ?? ?? ??", "ENEMY_ANALYSIS_DATA_PTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                ENEMY_ANALYSIS_DATA_PTR = (nint)funcAddress;
                Log($"ENEMY_ANALYSIS_DATA_PTR: {ENEMY_ANALYSIS_DATA_PTR:X}");
            });

            SigScan("4C 8B 05 ?? ?? ?? ?? 41 8B 50 ?? 29 CA", "CurrentAIBasePTR", address => // 
            {
                var funcAddress = GetGlobalAddress(address + 3);
                CurrentAIBasePTR = (nint)funcAddress;
                Log($"CurrentAIBasePTR: {CurrentAIBasePTR:X}");
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
                }, 0x9999);
            }

            flowFramework.Register("BTL_GET_COUNTER", 1, () =>
            {
                var COUNTER = flowApi.GetIntArg(0);

                flowApi.SetReturnValue(BTL_COUNTERS[COUNTER]);

                return FlowStatus.SUCCESS;
            }, 0x0015);

            flowFramework.Register("BTL_SET_COUNTER", 2, () =>
            {
                var COUNTER = flowApi.GetIntArg(0);
                var value = flowApi.GetIntArg(1);

                BTL_COUNTERS[COUNTER] = value;

                return FlowStatus.SUCCESS;
            }, 0x0016);

            flowFramework.Register("SET_HUMAN_LV", 2, () =>
            {
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
            }, 0x2500);


            flowFramework.Register("GET_BULLETS", 1, () =>
            {
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
            }, 0x2501);


            flowFramework.Register("SET_BULLETS", 2, () =>
            {
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
            }, 0x2502);


            flowFramework.Register("GET_TACTIC", 1, () =>
            {
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
            }, 0x2503);


            flowFramework.Register("SET_TACTIC", 2, () =>
            {
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
            }, 0x2504);

            flowFramework.Register("GET_PC_LEVEL", 0, () =>
            {
                datUnit* PartyMember = _gameFunctions.getDatUnitFromPlayerID((ushort)1);
                var currLv = PartyMember->Joker_Lv;

                flowApi.SetReturnValue(currLv);

                return FlowStatus.SUCCESS;
            }, 0x019e);

            flowFramework.Register("SET_UNIT_FLAGS", 3, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                bool onOff = flowApi.GetIntArg(2) > 0;

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<int>((nuint)currUnit->Flags, ToggleBit(currUnit->Flags, toggleFlag, onOff));

                return FlowStatus.SUCCESS;
            }, 0x2506);

            flowFramework.Register("SET_STATUS_EFFECT", 3, () =>
            {
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
            }, 0x2507);

            // starting reference point: https://github.com/DeathChaos25/ps3-ckit/blob/main/prx/modules/p5/EXFLW/EXFLW.c#L1432

            flowFramework.Register("SET_UNIT_ARCANA", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                byte arcana = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(UNIT_TBL_Section0_PTR + (long)(unitID * UNIT_TBL_Section0_EntrySize + 4)); // UnitTBL_Segment1->Arcana
                memory.Write<byte>(targetPTR, arcana);
                return FlowStatus.SUCCESS;
            }, 0x2508);

            flowFramework.Register("GET_UNIT_ARCANA", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);
                flowApi.SetReturnValue(currUnit->Arcana);
                return FlowStatus.SUCCESS;
            }, 0x2509);

            flowFramework.Register("SET_UNIT_LEVEL", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                short tarLv = (short)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<short>((nuint)(&currUnit->UnitLv), tarLv);
                return FlowStatus.SUCCESS;
            }, 0x250A);

            flowFramework.Register("GET_UNIT_LEVEL", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->UnitLv);
                return FlowStatus.SUCCESS;
            }, 0x250B);

            flowFramework.Register("SET_UNIT_HP", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                int targetHP = flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<int>((nuint)(&currUnit->HP), targetHP);
                return FlowStatus.SUCCESS;
            }, 0x250C);

            flowFramework.Register("GET_UNIT_HP", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);


                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->HP);
                return FlowStatus.SUCCESS;
            }, 0x250D);

            flowFramework.Register("SET_UNIT_SP", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                int targetSP = flowApi.GetIntArg(1);


                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<int>((nuint)(&currUnit->SP), targetSP);
                return FlowStatus.SUCCESS;
            }, 0x250E);

            flowFramework.Register("GET_UNIT_SP", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->SP);
                return FlowStatus.SUCCESS;
            }, 0x250F);

            flowFramework.Register("SET_UNIT_STATS", 3, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                int stat_type = flowApi.GetIntArg(1);
                byte stat_value = (byte)flowApi.GetIntArg(2);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->Stats[stat_type]), stat_value);
                return FlowStatus.SUCCESS;
            }, 0x2510);

            flowFramework.Register("GET_UNIT_STATS", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                int stat_type = flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->Stats[stat_type]);
                return FlowStatus.SUCCESS;
            }, 0x2511);

            flowFramework.Register("SET_UNIT_SKILL", 3, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                int skill_slot = flowApi.GetIntArg(1);
                short skill_id = (short)flowApi.GetIntArg(2);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<short>((nuint)(&currUnit->BattleSkills[skill_slot]), skill_id);
                return FlowStatus.SUCCESS;
            }, 0x2512);

            flowFramework.Register("GET_UNIT_SKILL", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                int skill_slot = flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->BattleSkills[skill_slot]);
                return FlowStatus.SUCCESS;
            }, 0x2513);

            flowFramework.Register("SET_UNIT_REWARD_EXP", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                short exp = (short)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<short>((nuint)(&currUnit->ExpReward), exp);
                return FlowStatus.SUCCESS;
            }, 0x2514);

            flowFramework.Register("GET_UNIT_REWARD_EXP", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->ExpReward);
                return FlowStatus.SUCCESS;
            }, 0x2515);

            flowFramework.Register("SET_UNIT_REWARD_MONEY", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                int money = flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<int>((nuint)(&currUnit->MoneyReward), money);
                return FlowStatus.SUCCESS;
            }, 0x2516);

            flowFramework.Register("GET_UNIT_REWARD_MONEY", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->MoneyReward);
                return FlowStatus.SUCCESS;
            }, 0x2517);

            flowFramework.Register("SET_UNIT_ATTACK_ELEMENT", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                byte element = (byte)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->AtkAttribute), element);
                return FlowStatus.SUCCESS;
            }, 0x2518);

            flowFramework.Register("GET_UNIT_ATTACK_ELEMENT", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->AtkAttribute);
                return FlowStatus.SUCCESS;
            }, 0x2519);

            flowFramework.Register("SET_UNIT_ATTACK_ACCURACY", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                byte acc = (byte)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<byte>((nuint)(&currUnit->AtkAccuracy), acc);
                return FlowStatus.SUCCESS;
            }, 0x251A);

            flowFramework.Register("GET_UNIT_ATTACK_ACCURACY", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->AtkAccuracy);
                return FlowStatus.SUCCESS;
            }, 0x251B);

            flowFramework.Register("SET_UNIT_ATTACK_DAMAGE", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                short dmg = (short)flowApi.GetIntArg(1);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                memory.Write<short>((nuint)(&currUnit->AtkDamage), dmg);
                return FlowStatus.SUCCESS;
            }, 0x251C);

            flowFramework.Register("GET_UNIT_ATTACK_DAMAGE", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);

                var currUnit = _gameFunctions.GetUnitTBL_Segment0_Entry(unitID);

                flowApi.SetReturnValue(currUnit->AtkDamage);
                return FlowStatus.SUCCESS;
            }, 0x251D);

            flowFramework.Register("SET_UNIT_PERSONA_MASK", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                short personaID = (short)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(UNIT_TBL_Section4_PTR + (long)(unitID * UNIT_TBL_Section4_EntrySize));
                memory.Write<short>(targetPTR, personaID);
                return FlowStatus.SUCCESS;
            }, 0x251E);

            flowFramework.Register("GET_UNIT_PERSONA_MASK", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(UNIT_TBL_Section4_PTR + (long)(unitID * UNIT_TBL_Section4_EntrySize));
                short personaID = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(personaID);
                return FlowStatus.SUCCESS;
            }, 0x251F);

            flowFramework.Register("SET_UNIT_MODEL", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                byte modelID = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(UNIT_TBL_Section4_PTR + (long)(unitID * UNIT_TBL_Section4_EntrySize + 2));
                memory.Write<byte>(targetPTR, modelID);
                return FlowStatus.SUCCESS;
            }, 0x2520);

            flowFramework.Register("GET_UNIT_MODEL", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(UNIT_TBL_Section4_PTR + (long)(unitID * UNIT_TBL_Section4_EntrySize + 2));
                byte modelID = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(modelID);
                return FlowStatus.SUCCESS;
            }, 0x2521);

            flowFramework.Register("SET_UNIT_AI", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                short ai_ID = (short)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(ELSAI_TBL_Section0_PTR + (long)(unitID * ELSAI_TBL_Section0_EntrySize + 2));
                memory.Write<short>(targetPTR, ai_ID);
                return FlowStatus.SUCCESS;
            }, 0x2522);

            flowFramework.Register("GET_UNIT_AI", 1, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(ELSAI_TBL_Section0_PTR + (long)(unitID * ELSAI_TBL_Section0_EntrySize + 2));
                short ai_ID = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(ai_ID);
                return FlowStatus.SUCCESS;
            }, 0x2523);

            flowFramework.Register("SET_ENCOUNT_ENEMY", 3, () =>
            {
                int encount_ID = flowApi.GetIntArg(0);
                int enemySlot = flowApi.GetIntArg(1);
                int enemyID = flowApi.GetIntArg(2);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);
                memory.Write<ushort>((nuint)(&currEnc->BattleUnitID[enemySlot]), (ushort)enemyID);

                return FlowStatus.SUCCESS;
            }, 0x2524);

            flowFramework.Register("GET_ENCOUNT_ENEMY", 2, () =>
            {
                int encount_ID = flowApi.GetIntArg(0);
                int enemySlot = flowApi.GetIntArg(1);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                flowApi.SetReturnValue(memory.Read<ushort>((nuint)(&currEnc->BattleUnitID[enemySlot])));
                return FlowStatus.SUCCESS;
            }, 0x2525);

            flowFramework.Register("SET_ENCOUNT_FIELD", 3, () =>
            {
                int encount_ID = flowApi.GetIntArg(0);
                int majorID = flowApi.GetIntArg(1);
                int minorID = flowApi.GetIntArg(2);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);
                memory.Write<ushort>((nuint)(&currEnc->FieldID), (ushort)majorID);
                memory.Write<ushort>((nuint)(&currEnc->RoomID), (ushort)minorID);

                return FlowStatus.SUCCESS;
            }, 0x2526);

            flowFramework.Register("GET_ENCOUNT_FIELD_MAJOR", 1, () =>
            {
                int encount_ID = flowApi.GetIntArg(0);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                flowApi.SetReturnValue(memory.Read<ushort>((nuint)(&currEnc->FieldID)));
                return FlowStatus.SUCCESS;
            }, 0x2527);

            flowFramework.Register("GET_ENCOUNT_FIELD_MINOR", 1, () =>
            {
                int encount_ID = flowApi.GetIntArg(0);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                flowApi.SetReturnValue(memory.Read<ushort>((nuint)(&currEnc->RoomID)));
                return FlowStatus.SUCCESS;
            }, 0x2528);

            flowFramework.Register("SET_ENCOUNT_BGM", 2, () =>
            {
                int encount_ID = flowApi.GetIntArg(0);
                int bgmID = flowApi.GetIntArg(1);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);
                memory.Write<ushort>((nuint)(&currEnc->BGMID), (ushort)bgmID);

                return FlowStatus.SUCCESS;
            }, 0x2529);

            flowFramework.Register("GET_ENCOUNT_BGM", 1, () =>
            {
                int encount_ID = flowApi.GetIntArg(0);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                flowApi.SetReturnValue(memory.Read<ushort>((nuint)(&currEnc->BGMID)));
                return FlowStatus.SUCCESS;
            }, 0x252A);

            flowFramework.Register("SET_UNIT_AFFINITY", 3, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                int affinity_slot = flowApi.GetIntArg(1);
                short new_affinity = (short)flowApi.GetIntArg(2);
                nuint targetPTR = (nuint)(UNIT_TBL_Section1_PTR + (long)(unitID * UNIT_TBL_Section1_EntrySize) + (long)(affinity_slot * 2));
                memory.Write<short>(targetPTR, new_affinity);
                return FlowStatus.SUCCESS;
            }, 0x252B);

            flowFramework.Register("GET_UNIT_AFFINITY", 2, () =>
            {
                int unitID = flowApi.GetIntArg(0);
                int affinity_slot = flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(UNIT_TBL_Section1_PTR + (long)(unitID * UNIT_TBL_Section1_EntrySize) + (long)(affinity_slot * 2));
                short affinity_value = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(affinity_value);
                return FlowStatus.SUCCESS;
            }, 0x252C);

            //
            // reference: https://github.com/tge-was-taken/010-Editor-Templates/blob/master/templates/p5r_tbl.bt
            //

            flowFramework.Register("SET_SKILL_ELEMENT", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte element = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize));
                memory.Write<byte>(targetPTR, element);
                return FlowStatus.SUCCESS;
            }, 0x252D);

            flowFramework.Register("GET_SKILL_ELEMENT", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize));
                byte element = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(element);
                return FlowStatus.SUCCESS;
            }, 0x252E);

            flowFramework.Register("SET_SKILL_PASSIVE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte isPassive = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize + 1));
                memory.Write<byte>(targetPTR, isPassive);
                return FlowStatus.SUCCESS;
            }, 0x252F);

            flowFramework.Register("GET_SKILL_PASSIVE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize + 1));
                byte isPassive = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(isPassive);
                return FlowStatus.SUCCESS;
            }, 0x2530);

            flowFramework.Register("SET_SKILL_INHERITABLE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte isInherit = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize + 2));
                memory.Write<byte>(targetPTR, isInherit);
                return FlowStatus.SUCCESS;
            }, 0x2531);

            flowFramework.Register("GET_SKILL_INHERITABLE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section0_PTR + (long)(skill_ID * SKILL_TBL_Section0_EntrySize + 2));
                byte isInherit = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(isInherit);
                return FlowStatus.SUCCESS;
            }, 0x2532);

            flowFramework.Register("SET_SKILL_AREA_USABLE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte area_val = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 5));
                memory.Write<byte>(targetPTR, area_val);
                return FlowStatus.SUCCESS;
            }, 0x2533);

            flowFramework.Register("GET_SKILL_AREA_USABLE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 5));
                byte area_val = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(area_val);
                return FlowStatus.SUCCESS;
            }, 0x2534);

            flowFramework.Register("SET_SKILL_DAMAGE_STAT", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte stat_type = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 6));
                memory.Write<byte>(targetPTR, stat_type);
                return FlowStatus.SUCCESS;
            }, 0x2535);

            flowFramework.Register("GET_SKILL_DAMAGE_STAT", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 6));
                byte stat_type = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(stat_type);
                return FlowStatus.SUCCESS;
            }, 0x2536);

            flowFramework.Register("SET_SKILL_COST_TYPE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte cost_type = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 7));
                memory.Write<byte>(targetPTR, cost_type);
                return FlowStatus.SUCCESS;
            }, 0x2537);

            flowFramework.Register("GET_SKILL_COST_TYPE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 7));
                byte cost_type = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(cost_type);
                return FlowStatus.SUCCESS;
            }, 0x2538);

            flowFramework.Register("SET_SKILL_COST", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                short cost = (short)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 8));
                memory.Write<short>(targetPTR, cost);
                return FlowStatus.SUCCESS;
            }, 0x2539);

            flowFramework.Register("GET_SKILL_COST", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 8));
                short cost = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(cost);
                return FlowStatus.SUCCESS;
            }, 0x253A);

            flowFramework.Register("SET_SKILL_TARGETING_TYPE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte tar_type = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 12));
                memory.Write<byte>(targetPTR, tar_type);
                return FlowStatus.SUCCESS;
            }, 0x253B);

            flowFramework.Register("GET_SKILL_TARGETING_TYPE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 12));
                byte tar_type = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(tar_type);
                return FlowStatus.SUCCESS;
            }, 0x253C);

            flowFramework.Register("SET_SKILL_TARGETS", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte tar = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 13));
                memory.Write<byte>(targetPTR, tar);
                return FlowStatus.SUCCESS;
            }, 0x253D);

            flowFramework.Register("GET_SKILL_TARGETS", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 13));
                byte tar = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(tar);
                return FlowStatus.SUCCESS;
            }, 0x253E);

            flowFramework.Register("SET_SKILL_ACCURACY", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte acc = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 20));
                memory.Write<byte>(targetPTR, acc);
                return FlowStatus.SUCCESS;
            }, 0x253F);

            flowFramework.Register("GET_SKILL_ACCURACY", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 20));
                byte acc = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(acc);
                return FlowStatus.SUCCESS;
            }, 0x2540);

            flowFramework.Register("SET_SKILL_MIN_HITS", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte min_hits_amt = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 21));
                memory.Write<byte>(targetPTR, min_hits_amt);
                return FlowStatus.SUCCESS;
            }, 0x2541);

            flowFramework.Register("GET_SKILL_MIN_HITS", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 21));
                byte min_hits_amt = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(min_hits_amt);
                return FlowStatus.SUCCESS;
            }, 0x2542);

            flowFramework.Register("SET_SKILL_MAX_HITS", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte max_hits_amt = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 22));
                memory.Write<byte>(targetPTR, max_hits_amt);
                return FlowStatus.SUCCESS;
            }, 0x2543);

            flowFramework.Register("GET_SKILL_MAX_HITS", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 22));
                byte max_hits_amt = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(max_hits_amt);
                return FlowStatus.SUCCESS;
            }, 0x2544);

            flowFramework.Register("SET_SKILL_DAMAGE_HEALING_TYPE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte val = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 23));
                memory.Write<byte>(targetPTR, val);
                return FlowStatus.SUCCESS;
            }, 0x2545);

            flowFramework.Register("GET_SKILL_DAMAGE_HEALING_TYPE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 23));
                byte val = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(val);
                return FlowStatus.SUCCESS;
            }, 0x2546);

            flowFramework.Register("SET_SKILL_DAMAGE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                short dmg = (short)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 24));
                memory.Write<short>(targetPTR, dmg);
                return FlowStatus.SUCCESS;
            }, 0x2547);

            flowFramework.Register("GET_SKILL_DAMAGE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 24));
                short dmg = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(dmg);
                return FlowStatus.SUCCESS;
            }, 0x2548);

            flowFramework.Register("SET_SKILL_DEPLETE_SP", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte SP = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 26));
                memory.Write<byte>(targetPTR, SP);
                return FlowStatus.SUCCESS;
            }, 0x2549);

            flowFramework.Register("GET_SKILL_DEPLETE_SP", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 26));
                byte SP = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(SP);
                return FlowStatus.SUCCESS;
            }, 0x254A);

            flowFramework.Register("SET_SKILL_CRIT_CHANCE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte crit_chance = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 45));
                memory.Write<byte>(targetPTR, crit_chance);
                return FlowStatus.SUCCESS;
            }, 0x254B);

            flowFramework.Register("GET_SKILL_CRIT_CHANCE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 45));
                byte crit_chance = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(crit_chance);
                return FlowStatus.SUCCESS;
            }, 0x254C);


            // https://i.imgur.com/Cm13zBQ.gif my reaction to this function
            flowFramework.Register("UNIT_CLEAR_ANALYSIS", 1, () =>
            {
                int enemy_ID = flowApi.GetIntArg(0);

                for (int bitIndex = 0; bitIndex < 11; bitIndex++)
                {
                    WriteEnemyBit(ENEMY_ANALYSIS_DATA_PTR, enemy_ID, bitIndex, false);
                }

                return FlowStatus.SUCCESS;
            }, 0x254D);

            flowFramework.Register("SET_ENCOUNT_FLAG", 3, () =>
            {
                int encount_ID = flowApi.GetIntArg(0);
                int toggleFlag = flowApi.GetIntArg(1);
                bool onOff = flowApi.GetIntArg(2) > 0;

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                memory.Write<int>((nuint)(&currEnc->flags), ToggleBit((int)currEnc->flags, toggleFlag, onOff));
                return FlowStatus.SUCCESS;
            }, 0x254E);

            flowFramework.Register("GET_ENCOUNT_FLAG", 2, () =>
            {
                int encount_ID = flowApi.GetIntArg(0);
                int toggleFlag = flowApi.GetIntArg(1);

                encounterIDTBL* currEnc = _gameFunctions.GetEncounterTBLData(encount_ID);

                int flags = memory.Read<int>((nuint)(&currEnc->flags));

                flowApi.SetReturnValue(IsBitSet(flags, toggleFlag));
                return FlowStatus.SUCCESS;
            }, 0x254F);

            flowFramework.Register("SET_BATTLE_FLAG", 3, () =>
            {
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
                int skill_ID = flowApi.GetIntArg(0);
                byte eff_type = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 30));
                memory.Write<byte>(targetPTR, eff_type);
                return FlowStatus.SUCCESS;
            }, 0x2552);

            flowFramework.Register("GET_SKILL_EFFECT_TYPE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 30));
                byte eff_type = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(eff_type);
                return FlowStatus.SUCCESS;
            }, 0x2553);

            flowFramework.Register("SET_SKILL_EFFECT_CHANCE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte eff_chance = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 31));
                memory.Write<byte>(targetPTR, eff_chance);
                return FlowStatus.SUCCESS;
            }, 0x2554);

            flowFramework.Register("GET_SKILL_EFFECT_CHANCE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 31));
                byte eff_chance = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(eff_chance);
                return FlowStatus.SUCCESS;
            }, 0x2555);

            flowFramework.Register("SET_SKILL_EFFECT_EXTRA", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                byte eff_duration = (byte)flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 44));
                memory.Write<byte>(targetPTR, eff_duration);
                return FlowStatus.SUCCESS;
            }, 0x2556);

            flowFramework.Register("GET_SKILL_EFFECT_EXTRA", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 44));
                byte eff_duration = memory.Read<byte>(targetPTR);
                flowApi.SetReturnValue(eff_duration);
                return FlowStatus.SUCCESS;
            }, 0x2557);

            flowFramework.Register("SET_SKILL_EFFECT", 3, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                bool onOff = flowApi.GetIntArg(2) > 0;

                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 32));
                long eff_bitfield = memory.Read<long>(targetPTR);
                memory.Write<long>(targetPTR, ToggleBit(eff_bitfield, toggleFlag, onOff));
                return FlowStatus.SUCCESS;
            }, 0x2558);

            flowFramework.Register("GET_SKILL_EFFECT", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                var toggleFlag = flowApi.GetIntArg(1);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 32));
                long eff_bitfield = memory.Read<long>(targetPTR);
                flowApi.SetReturnValue(IsBitSet(eff_bitfield, toggleFlag));
                return FlowStatus.SUCCESS;
            }, 0x2559);

            flowFramework.Register("AI_SET_FLAG", 2, () =>
            {
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
            }, 0x255A);

            flowFramework.Register("AI_GET_FLAG", 1, () =>
            {
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
            }, 0x255B);

            flowFramework.Register("AI_SET_LV", 1, () =>
            {
                bool isValid = (nint)GetDatUnitFromCurrentAI() > 0;
                if (!isValid)
                {
                    return FlowStatus.SUCCESS;
                }
                ushort targetLV = (ushort)flowApi.GetIntArg(0);
                datUnit* currEnemy = GetDatUnitFromCurrentAI();

                memory.Write<ushort>((nuint)(&currEnemy->Joker_Lv), targetLV);

                return FlowStatus.SUCCESS;
            }, 0x255C);

            flowFramework.Register("AI_GET_LV", 0, () =>
            {
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
            }, 0x255D);

            flowFramework.Register("SET_SKILL_JYOKYO_MESSAGE", 2, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                short messID = (short)flowApi.GetIntArg(1);

                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 42)); // 0x2A

                memory.Write<short>(targetPTR, messID);
                return FlowStatus.SUCCESS;
            }, 0x255E);

            flowFramework.Register("GET_SKILL_JYOKYO_MESSAGE", 1, () =>
            {
                int skill_ID = flowApi.GetIntArg(0);
                nuint targetPTR = (nuint)(SKILL_TBL_Section1_PTR + (long)(skill_ID * SKILL_TBL_Section1_EntrySize + 42)); // 0x2A
                short messID = memory.Read<short>(targetPTR);
                flowApi.SetReturnValue(messID);
                return FlowStatus.SUCCESS;
            }, 0x255F);
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

        public static unsafe void CombineColorBytes(nint a1, SoundBank_Struct* a2, nint a3)
        {
            a2->Field000 = (int)ByteswapUlong((uint)CombineBytes(_configuration._031_JokerColor_R, _configuration._031_JokerColor_G, _configuration._031_JokerColor_B));

            _hookCombineColorBytes.OriginalFunction(a1, a2, a3);
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

            return (flags & (1 << n));
        }

        static long IsBitSet(long flags, int n)
        {
            if (n < 0 || n >= 64)
                throw new ArgumentOutOfRangeException(nameof(n), "Bit position must be between 0 and 63.");

            return flags & (1L << n);  // Returns 0 if not set, or 2^n if set
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
            LogNoPrefix($"datUnit PTR for Current enemy is 0x{(nint)(&currEnemy):X8}\nEnemy ID / Level -> {currEnemy->unitID} / {currEnemy->Joker_Lv} \nHP/SP -> {currEnemy->currentHP}/{currEnemy->currentSP} \ndatUnit Flags -> 0x{currEnemy->Flags:X8}");
            return currEnemy;
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
    }
}
