using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using System.Diagnostics;
using Reloaded.Memory;
using Reloaded.Memory.Interfaces;
using Reloaded.Hooks.Definitions.X64;
using static p5r.enhance.cbt.reloaded.GameFunctions_Structs;
using static p5r.enhance.cbt.reloaded.Utils;
using System.Net;
using Reloaded.Hooks.Definitions;
using p5r.enhance.cbt.reloaded.Template;
using p5r.enhance.cbt.reloaded.Configuration;
using Reloaded.Memory;
using p5rpc.flowscriptframework.interfaces;
using System;
using System.Runtime.InteropServices;

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

            ////////////////////////////////
            ////////////////////////////////
            ////////////////////////////////
            // standalone flow functions

            if (_configuration._017_restoreDebugPrintingFunctions)
            {
                flowFramework.Register("PUT", 1, () =>
                {
                    var flowApi = flowFramework.GetFlowApi();
                    var message = flowApi.GetStringArg(0);
                    LogNoPrefix(message);
                    return FlowStatus.SUCCESS;
                }, 0x0002);

                flowFramework.Register("PUTS", 1, () =>
                {
                    var flowApi = flowFramework.GetFlowApi();
                    var message = flowApi.GetIntArg(0);
                    LogNoPrefix($"{message}");
                    return FlowStatus.SUCCESS;
                }, 0x0003);

                flowFramework.Register("PUTF", 1, () =>
                {
                    var flowApi = flowFramework.GetFlowApi();
                    var message = flowApi.GetFloatArg(0);
                    LogNoPrefix($"{message}");
                    return FlowStatus.SUCCESS;
                }, 0x0004);
            }

            flowFramework.Register("BTL_GET_COUNTER", 1, () =>
            {
                var flowApi = flowFramework.GetFlowApi();

                var COUNTER = flowApi.GetIntArg(0);

                flowApi.SetReturnValue(BTL_COUNTERS[COUNTER]);

                return FlowStatus.SUCCESS;
            }, 0x0015);

            flowFramework.Register("BTL_SET_COUNTER", 2, () =>
            {
                var flowApi = flowFramework.GetFlowApi();

                var COUNTER = flowApi.GetIntArg(0);
                var value = flowApi.GetIntArg(1);

                BTL_COUNTERS[COUNTER] = value;

                return FlowStatus.SUCCESS;
            }, 0x0016);

            flowFramework.Register("SET_HUMAN_LV", 2, () =>
            {
                var flowApi = flowFramework.GetFlowApi();

                var unitID = flowApi.GetIntArg(0);

                if (unitID != 1) return FlowStatus.SUCCESS;

                var targetLV = flowApi.GetIntArg(1);

                if (targetLV < 0 || targetLV > 100) return FlowStatus.SUCCESS;

                datUnit* Joker = _gameFunctions.getDatUnitFromPlayerID(1);

                Joker->Joker_Lv = (ushort)targetLV;
                Joker->Joker_EXP = JokerExpValues[targetLV];

                return FlowStatus.SUCCESS;
            }, 0x00D6);

            /*flowFramework.Register("AI_SET_TACTICS", 1, () =>
            {
                // ToDo

                return FlowStatus.SUCCESS;
            }, 0x010e);*/

            flowFramework.Register("FLAG_DATA_INPUT", 3, () =>
            {
                var flowApi = flowFramework.GetFlowApi();

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
                var flowApi = flowFramework.GetFlowApi();

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
                var flowApi = flowFramework.GetFlowApi();

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
    }
}
