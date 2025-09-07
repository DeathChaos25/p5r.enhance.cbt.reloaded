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

namespace p5r.enhance.cbt.reloaded
{
    internal unsafe class GameFunctions
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

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate datUnit* getDatUnitFromPlayerIDDelegate(ushort id);
        public getDatUnitFromPlayerIDDelegate getDatUnitFromPlayerID;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate void playSingleWordCueDelegate(int id);
        public playSingleWordCueDelegate playSingleWordCue;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate void playSystemCueDelegate(int id);
        public playSystemCueDelegate playSystemCue;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate bool BitChkDelegate(nint id);
        public BitChkDelegate BIT_CHK;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate void BitSetDelegate(nint id, int status);
        public BitSetDelegate BIT_SET;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate PtrToFileHandle* file_open_simpleDelegate(char* a1);
        public file_open_simpleDelegate file_open_simple;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate int fs_syncDelegate(fileHandleStruct* a1);
        public fs_syncDelegate fsSync;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate void scrRunScriptDelegate(int a1, nint fileAddress, nint fileSize, int procIdx);
        public scrRunScriptDelegate scrRunScript;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate int CallNaviDialogueDelegate(int a1, nint a2);
        public CallNaviDialogueDelegate CallNaviDialogueImpl;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate int GetAssistBustupIDDelegate(int a1, nint a2);
        public GetAssistBustupIDDelegate GetAssistBustupID;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate encounterIDTBL* GetEncounterTBLEntryDelegate(int a1);
        public GetEncounterTBLEntryDelegate GetEncounterTBLData;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate UnitTBL_Segment0* GetUnitTBL_Segment0_Delegate(int a1);
        public GetUnitTBL_Segment0_Delegate GetUnitTBL_Segment0_Entry;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public delegate ushort GetUnitMaxBulletsDelegate(ushort itemID);
        public GetUnitMaxBulletsDelegate GetUnitMaxBullets;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public delegate nint somethingBossDDSFileOpenDelegate(nint a1, uint a2, uint a3);
        public somethingBossDDSFileOpenDelegate somethingBossDDSFileOpen;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public delegate ushort GetPlayerIDFromPartySlotDelegate(int a1);
        public GetPlayerIDFromPartySlotDelegate GetPlayerIDFromPartySlot;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public delegate void FreeSmartPointerDelegate(nint a1);
        public FreeSmartPointerDelegate FreeSmartPointer;

        /*[Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public unsafe delegate void LoadSoundByCueIDCombatVoiceDelegate(nint a1, nint a2, int CueID, byte a4);
        public LoadSoundByCueIDCombatVoiceDelegate LoadSoundByCueIDCombatVoice;*/

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public delegate void SetCurrentTotalDayDelegate(ushort day);
        public SetCurrentTotalDayDelegate SetCurrentTotalDay;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public delegate void SetCurrentTimeslotDelegate(ushort day);
        public SetCurrentTimeslotDelegate SetCurrentTimeslot;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        public delegate ushort GetTotalDayFromDateDelegate(ushort month, byte day);
        public GetTotalDayFromDateDelegate GetTotalDayFromDate;

        private nint GetDaysAddr = 0;
        private nint GetUserLangAddr = 0;

        Random random = new Random();

        internal GameFunctions(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            unsafe datUnit_Persona* GetPersonaAtIndex(datUnit* unit, int index)
            {
                if (index < 0 || index >= 12)
                    throw new ArgumentOutOfRangeException(nameof(index));

                // Calculate the address of the desired persona
                byte* basePtr = unit->StockPersona;
                byte* personaPtr = basePtr + (index * 0x30); // 0x30 is the size of datUnit_Persona

                // Cast the byte pointer to a datUnit_Persona pointer
                return (datUnit_Persona*)personaPtr;
            }


            // v1.0.0 = 0x14B94E3C0
            SigScan("0F B7 C1 48 8D 0D ?? ?? ?? ?? 48 69 C0 A0 02 00 00 48 01 C8", "get_btlunit_from_player_id_Sig", address =>
            {
                getDatUnitFromPlayerID = _hooks.CreateWrapper<getDatUnitFromPlayerIDDelegate>(address, out _);
            });

            // v1.0.0 = 0x14173bc60
            SigScan("40 53 48 83 EC 30 BA 02 00 00 00", "snd_man_play_single_word_Sig", address =>
            {
                playSingleWordCue = _hooks.CreateWrapper<playSingleWordCueDelegate>(address, out _);
            });

            // v1.0.0 = 0x1557a14b0
            SigScan("40 53 48 83 EC 30 48 8B 1D ?? ?? ?? ?? 48 85 DB 74 ?? 8B 53 ?? 41 89 C8", "snd_man_play_system_Sig", address =>
            {
                playSystemCue = _hooks.CreateWrapper<playSystemCueDelegate>(address, out _);
            });

            // v1.0.0 = 0x1406ee119
            SigScan("48 63 05 ?? ?? ?? ?? 83 F8 0A 0F 87 ?? ?? ?? ?? 48 8D 15 ?? ?? ?? ?? 8B 8C ?? ?? ?? ?? ?? 48 03 CA FF E1 48 8D 54 24 ??", "get_user_lang_Sig", address =>
            {
                GetUserLangAddr = address;
            });

            // v1.0.0 = 0x14b922400
            SigScan("48 89 5C 24 ?? 57 48 83 EC 20 0F B6 FA 89 CB 89 F9", "bit_set_Sig", address =>
            {
                BIT_SET = _hooks.CreateWrapper<BitSetDelegate>(address, out _);
            });

            // v1.0.0 = 0x140dd67c0
            SigScan("4C 8D 05 ?? ?? ?? ?? 33 C0 49 8B D0 0F 1F 40 00 39 0A 74 ?? FF C0 48 83 C2 08 83 F8 10 72 ?? 8B D1", "bit_chk_Sig", address =>
            {
                BIT_CHK = _hooks.CreateWrapper<BitChkDelegate>(address, out _);
            });

            // v1.0.0 = 0x1416664c0
            // v1.0.0 = 0x14155a490
            SigScan("48 89 5C 24 ?? 57 48 83 EC 30 BA 28 00 00 00 48 8B F9", "file_open_simple_Sig", address =>
            {
                file_open_simple = _hooks.CreateWrapper<file_open_simpleDelegate>(address, out _);
            });

            // v1.0.0 = 0x1416871c0
            // v1.0.4 = 0x14c6ba870
            SigScan("83 79 ?? 0C 75 ?? 80 B9 ?? ?? ?? ?? 00 B8 01 00 00 00 BA FF FF FF FF", "fs_sync_Sig", address =>
            {
                fsSync = _hooks.CreateWrapper<fs_syncDelegate>(address, out _);
            });

            // v1.0.0 = 0x1556f4700
            SigScan("48 89 6C 24 ?? 48 89 74 24 ?? 41 56 48 83 EC 20 41 8B F1", "scrRunScript_Sig", address =>
            {
                scrRunScript = _hooks.CreateWrapper<scrRunScriptDelegate>(address, out _);
            });

            // v1.0.0 = 0x1417e9ab0
            // v1.0.4 = 0x1416b5240
            SigScan("48 83 EC 38 45 33 C0 E8 ?? ?? ?? ??", "call_navi_dialogue_Sig", address =>
            {
                CallNaviDialogueImpl = _hooks.CreateWrapper<CallNaviDialogueDelegate>(address, out _);
            });

            // v1.0.0 = 0x1417e9bd0
            SigScan("4C 8B 05 ?? ?? ?? ?? 4D 85 C0 74 ?? 4C 63 1D ?? ?? ?? ??", "get_assist_bustup_id_Sig", address =>
            {
                GetAssistBustupID = _hooks.CreateWrapper<GetAssistBustupIDDelegate>(address, out _);
            });

            /*// v1.0.0 = 0x1417e9bd0
            SigScan("48 89 5C 24 ?? 48 89 74 24 ?? 55 57 41 54 41 56 41 57 48 8B EC 48 83 EC 40 0F B7 41 ??", "LoadSoundByCueIDCombatVoice_Sig", address =>
            {
                LoadSoundByCueIDCombatVoice = _hooks.CreateWrapper<LoadSoundByCueIDCombatVoiceDelegate>(address, out _);
            });*/

            // v1.0.0 = 0x1406eb08e
            SigScan("E8 ?? ?? ?? ?? F6 00 01 74 ?? 45 32 E4", "get_encounter_tbl_data_Sig", address =>
            {
                var funcAddress = GetGlobalAddress(address + 1);
                GetEncounterTBLData = _hooks.CreateWrapper<GetEncounterTBLEntryDelegate>((long)funcAddress, out _);
            });

            // v1.0.4 = 0x140d7bc90
            SigScan("0F B7 C1 48 8D 0D ?? ?? ?? ?? 48 6B C0 44", "GetUnitTBL_Segment0_Entry", address =>
            {
                GetUnitTBL_Segment0_Entry = _hooks.CreateWrapper<GetUnitTBL_Segment0_Delegate>(address, out _);
            });

            // v1.0.4 = 0x14157c69f
            SigScan("48 89 5C 24 ?? 56 48 83 EC 20 33 C9 E8 ?? ?? ?? ?? 0F B7 C8", "BULLET_RECOVERY", address =>
            {
                var funcAddress = GetGlobalAddress(address + 1);
                GetUnitMaxBullets = _hooks.CreateWrapper<GetUnitMaxBulletsDelegate>((long)funcAddress, out _);
            });

            SigScan("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 60 48 89 CD 89 D6", "somethingBossDDSFileOpen_Sig", address =>
            {
                somethingBossDDSFileOpen = _hooks.CreateWrapper<somethingBossDDSFileOpenDelegate>(address, out _);
            });

            // v1.0.4 = 0x140d656f0
            SigScan("E8 ?? ?? ?? ?? 66 85 C0 74 ?? 41 FF C4", "GetUnitIDFromPartySlot", address =>
            {
                var funcAddress = GetGlobalAddress(address + 1);
                GetPlayerIDFromPartySlot = _hooks.CreateWrapper<GetPlayerIDFromPartySlotDelegate>((long)funcAddress, out _);
            });

            // v1.0.4 = 0x140029d10
            SigScan("E8 ?? ?? ?? ?? B8 27 01 00 00", "Free SmartPointer", address =>
            {
                var funcAddress = GetGlobalAddress(address + 1);
                FreeSmartPointer = _hooks.CreateWrapper<FreeSmartPointerDelegate>((long)funcAddress, out _);
            });

            // v1.0.4 = 0x140b712a0
            SigScan("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 0F B7 D9 33 C9", "SetCurrentTotalDay", address =>
            {
                SetCurrentTotalDay = _hooks.CreateWrapper<SetCurrentTotalDayDelegate>(address, out _);
            });

            // v1.0.4 = 0x140b70690
            SigScan("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 0F B6 F9 33 C9", "SetCurrentTimeslot", address =>
            {
                SetCurrentTimeslot = _hooks.CreateWrapper<SetCurrentTimeslotDelegate>(address, out _);
            });

            // v1.0.4 = 0x14af74410
            SigScan("48 83 EC 08 31 C0 41 89 D3", "GetTotalDayFromDate", address =>
            {
                GetTotalDayFromDate = _hooks.CreateWrapper<GetTotalDayFromDateDelegate>(address, out _);
            });

        }

        public int RandomIntBetween(int min, int max)
        {
            return random.Next(min, max + 1);
        }

        public void SetDaysAddr(nint a1)
        {
            GetDaysAddr = a1;
        }

        public short GetTotalDays()
        {
            nint a1 = GetDaysAddr;

            int opd = *(int*)(a1 + 3);

            nint newAddress = a1 + (nint)opd + 7;

            short** days = (short**)newAddress;
            return **days;
        }

        public unsafe TotalDaysStruct* GetTotalDaysStruct()
        {
            nint a1 = GetDaysAddr;
            int opd = *(int*)(a1 + 3);
            nint newAddress = a1 + (nint)opd + 7;

            TotalDaysStruct** structPtr = (TotalDaysStruct**)newAddress;
            return *structPtr;
        }

        public short GetUserLang()
        {
            nint a1 = GetUserLangAddr;

            int opd = *(int*)(a1 + 3);

            nint newAddress = a1 + (nint)opd + 7;

            short* lang = (short*)newAddress;
            return *lang;
        }
    }
}
