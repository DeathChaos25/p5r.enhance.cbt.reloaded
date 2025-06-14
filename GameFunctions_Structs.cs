using System;
using System.Runtime.InteropServices;

namespace p5r.enhance.cbt.reloaded
{
    public class GameFunctions_Structs
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct FileAccessStruct
        {
            public unsafe int* ptr1;
            public unsafe int* ptr2;
            public unsafe byte* filepath; // char* in C++
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DebugFlagData
        {
            public unsafe fixed byte FlagStatus[12800]; // u8 FlagStatus[12800]
        }

        public static readonly ushort[,] CharModelReplacementTable = new ushort[11, 6]
        {
            { 51, 51, 51, 51, 51, 51 },
            { 51, 52, 103, 113, 119, 51 },
            { 51, 103, 106, 109, 51, 51 },
            { 51, 61, 101, 51, 51, 51 },
            { 51, 106, 107, 109, 51, 51 },
            { 51, 103, 106, 107, 51, 51 },
            { 51, 104, 107, 109, 51, 51 },
            { 51, 102, 103, 107, 51, 51 },
            { 51, 102, 106, 108, 51, 51 },
            { 51, 52, 102, 104, 105, 106 },
            { 51, 102, 106, 107, 51, 51 }
        };

        public static readonly uint[] JokerExpValues = new uint[]
        {
            0, 0, 0x15, 0x2F, 0x63, 0xB8, 0x138, 0x1E9, 0x2D6, 0x405, 0x580, 0x74F, 0x97B, 0xC0B, 0xF08, 0x127A,
            0x166B, 0x1AE2, 0x1FE7, 0x2584, 0x2BC0, 0x32AA, 0x3A40, 0x4293, 0x4B9E, 0x5578, 0x6023, 0x6BAE, 0x7816,
            0x856A, 0x93B2, 0xA2F5, 0xB33D, 0xC491, 0xD6FB, 0xEA83, 0xFF30, 0x1150C, 0x12C1E, 0x14470, 0x15E0A, 0x178F3,
            0x19535, 0x1B2D7, 0x1D1E3, 0x1F261, 0x21458, 0x237D2, 0x25CD6, 0x2836E, 0x2ABA2, 0x2D579, 0x300FD, 0x32E35,
            0x35D2B, 0x38DE7, 0x3C070, 0x3F4D0, 0x42B0E, 0x46334, 0x49D4A, 0x4D957, 0x51765, 0x5577B, 0x599A3, 0x5DDE5,
            0x62448, 0x66CD6, 0x6B796, 0x70492, 0x753D2, 0x7A55D, 0x7F93D, 0x84F79, 0x8A81B, 0x9032B, 0x960B0, 0x9C0B4,
            0xA233E, 0xA8858, 0xAF00A, 0xB5A5B, 0xBC755, 0xC36FF, 0xCA963, 0xD1E89, 0xD9678, 0xE113A, 0xE8ED6, 0xF0F56,
            0xF92C2, 0x101921, 0x10A27D, 0x112EDD, 0x11BE4B, 0x1250CF, 0x12E670, 0x137F38, 0x141B2E, 0x14BA5C
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SoundBank_Struct
        {
            public int Field000;
            public int Field004;
            public unsafe fixed byte acb_path[256]; // char[256]
            public unsafe fixed byte awb_path[256]; // char[256]
            public long Field208;
            public int Field210;
            public int Field214;
            public long Field218;
            public int Field220;
            public long Field224;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct colorStruct
        {
            public byte red;
            public byte green;
            public byte blue;
            public byte alpha;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SoundBank_GlobalVars
        {
            public byte Field0;
            public byte Field1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BuffStatus
        {
            private uint _bitfield;

            public bool BuffStatus_31 => (_bitfield & (1 << 31)) != 0;
            public bool BuffStatus_30 => (_bitfield & (1 << 30)) != 0;
            public bool BuffStatus_29 => (_bitfield & (1 << 29)) != 0;
            public bool BuffStatus_28 => (_bitfield & (1 << 28)) != 0;
            public bool BuffStatus_27 => (_bitfield & (1 << 27)) != 0;
            public bool BuffStatus_ResistInstaKill => (_bitfield & (1 << 26)) != 0;
            public bool BuffStatus_25 => (_bitfield & (1 << 25)) != 0;
            public bool BuffStatus_24 => (_bitfield & (1 << 24)) != 0;
            public bool BuffStatus_Concentrate => (_bitfield & (1 << 23)) != 0;
            public bool BuffStatus_Charge => (_bitfield & (1 << 22)) != 0;
            public bool BuffStatus_ResistMagic => (_bitfield & (1 << 21)) != 0;
            public bool BuffStatus_ResistPhys => (_bitfield & (1 << 20)) != 0;
            public bool BuffStatus_19 => (_bitfield & (1 << 19)) != 0;
            public bool BuffStatus_ResistPsy => (_bitfield & (1 << 18)) != 0;
            public bool BuffStatus_ResistNuke => (_bitfield & (1 << 17)) != 0;
            public bool BuffStatus_ResistWind => (_bitfield & (1 << 16)) != 0;
            public bool BuffStatus_ResistElec => (_bitfield & (1 << 15)) != 0;
            public bool BuffStatus_ResistIce => (_bitfield & (1 << 14)) != 0;
            public bool BuffStatus_ResistFire => (_bitfield & (1 << 13)) != 0;
            public bool BuffStatus_AffPsy => (_bitfield & (1 << 12)) != 0;
            public bool BuffStatus_AffNuke => (_bitfield & (1 << 11)) != 0;
            public bool BuffStatus_AffElec => (_bitfield & (1 << 10)) != 0;
            public bool BuffStatus_AffWind => (_bitfield & (1 << 9)) != 0;
            public bool BuffStatus_AffIce => (_bitfield & (1 << 8)) != 0;
            public bool BuffStatus_AffFire => (_bitfield & (1 << 7)) != 0;
            public bool BuffStatus_Susceptibility => (_bitfield & (1 << 6)) != 0;
            public bool BuffStatus_Critical2 => (_bitfield & (1 << 5)) != 0;
            public bool BuffStatus_Critical => (_bitfield & (1 << 4)) != 0;
            public bool BuffStatus_EVA => (_bitfield & (1 << 3)) != 0;
            public bool BuffStatus_DEF => (_bitfield & (1 << 2)) != 0;
            public bool BuffStatus_ACC => (_bitfield & (1 << 1)) != 0;
            public bool BuffStatus_ATK => (_bitfield & (1 << 0)) != 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BuffMeta
        {
            private ulong _bitfield1;
            private ulong _bitfield2;

            public uint BuffDir_ACC => (uint)(_bitfield1 & 0xF);
            public uint BuffDir_ATK => (uint)((_bitfield1 >> 4) & 0xF);
            public uint BuffDir_EVA => (uint)((_bitfield1 >> 8) & 0xF);
            public uint BuffDir_DEF => (uint)((_bitfield1 >> 12) & 0xF);
            public uint BuffDir_Crit => (uint)((_bitfield1 >> 16) & 0xF);
            public uint BuffDir_Crit2 => (uint)((_bitfield1 >> 20) & 0xF);
            public uint BuffDir_Sucep => (uint)((_bitfield1 >> 24) & 0xF);
            public uint BuffDir_AffFire => (uint)((_bitfield1 >> 28) & 0xF);
            public uint BuffDir_AffIce => (uint)((_bitfield1 >> 32) & 0xF);
            public uint BuffDir_AffWind => (uint)((_bitfield1 >> 36) & 0xF);
            public uint BuffDir_AffElec => (uint)((_bitfield1 >> 40) & 0xF);
            public uint BuffDir_AffNuke => (uint)((_bitfield1 >> 44) & 0xF);
            public uint BuffDir_AffPsy => (uint)((_bitfield1 >> 48) & 0xF);
            public uint BuffDir_ResistFire => (uint)((_bitfield1 >> 52) & 0xF);
            public uint BuffDir_ResistIce => (uint)((_bitfield1 >> 56) & 0xF);
            public uint BuffDir_ResistElec => (uint)((_bitfield1 >> 60) & 0xF);
            public uint BuffDir_ResistWind => (uint)(_bitfield2 & 0xF);
            public uint BuffDir_ResistNuke => (uint)((_bitfield2 >> 4) & 0xF);
            public uint BuffDir_ResistPsy => (uint)((_bitfield2 >> 8) & 0xF);
            public uint BuffDir_ => (uint)((_bitfield2 >> 12) & 0xF);
            public uint BuffDur_ACC => (uint)((_bitfield2 >> 16) & 0xF);
            public uint BuffDur_ATK => (uint)((_bitfield2 >> 20) & 0xF);
            public uint BuffDur_EVA => (uint)((_bitfield2 >> 24) & 0xF);
            public uint BuffDur_DEF => (uint)((_bitfield2 >> 28) & 0xF);
            public uint BuffDur_Crit => (uint)((_bitfield2 >> 32) & 0xF);
            public uint BuffDur_Crit2 => (uint)((_bitfield2 >> 36) & 0xF);
            public uint BuffDur_Sucep => (uint)((_bitfield2 >> 40) & 0xF);
            public uint BuffDur_AffFire => (uint)((_bitfield2 >> 44) & 0xF);
            public uint BuffDur_AffIce => (uint)((_bitfield2 >> 48) & 0xF);
            public uint BuffDur_AffWind => (uint)((_bitfield2 >> 52) & 0xF);
            public uint BuffDur_AffElec => (uint)((_bitfield2 >> 56) & 0xF);
            public uint BuffDur_AffNuke => (uint)((_bitfield2 >> 60) & 0xF);
            public uint BuffDur_AffPsy => (uint)(_bitfield2 >> 64) & 0xF;
            public uint BuffDur_ResistFire => (uint)((_bitfield2 >> 68) & 0xF);
            public uint BuffDur_ResistIce => (uint)((_bitfield2 >> 72) & 0xF);
            public uint BuffDur_ResistElec => (uint)((_bitfield2 >> 76) & 0xF);
            public uint BuffDur_ResistWind => (uint)((_bitfield2 >> 80) & 0xF);
            public uint BuffDur_ResistNuke => (uint)((_bitfield2 >> 84) & 0xF);
            public uint BuffDur_ResistPsy => (uint)((_bitfield2 >> 88) & 0xF);
            public uint BuffDur_ => (uint)((_bitfield2 >> 92) & 0xF);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct datUnit_Persona
        {
            public byte Flags;
            public byte isUnlocked;
            public ushort personaID;
            public byte personaLv;
            public byte _x5;
            public ushort trait;
            public uint personaExp;
            public unsafe fixed ushort SkillID[8]; // public ushort SkillID[8]
            public unsafe fixed byte Stats[5]; // u8 Stats[5]
            public unsafe fixed byte StatsEx[5]; // u8 StatsEx[5]
            public unsafe fixed byte StatsExTemp[5]; // u8 StatsExTemp[5]
            public byte _x2B;
            public uint _x2C;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct datUnit
        {
            public int Flags;
            public ushort unitType;
            public ushort Field06;
            public uint unitID;
            public uint currentHP;
            public uint currentSP;
            public uint StatusAilments;
            public ushort Joker_Lv;
            public ushort Field1A;
            public uint Joker_EXP;
            public uint PhaseID;
            public BuffStatus Buffs;
            public uint BuffStatus2;
            public BuffMeta BuffsDirDur;
            public ushort EquippedPersonaIndex;
            public ushort Field42;
            public unsafe fixed byte StockPersona[12 * 0x30]; // datUnit_Persona StockPersona[12]
            public ushort meleeID;
            public ushort protectorID;
            public ushort accessoryID;
            public ushort outfitID;
            public ushort rangedWeaponID;
            public ushort Field28E;
            public ushort Field290;
            public ushort TacticsState;
            public ushort numOfBullets;
            public uint Field296;
            public ushort Field29A;
            public ushort HPGainNextLv;
            public ushort SPGainNextLv;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EnemyPersonaFunctionStruct3
        {
            public nint ptr1;
            public nint ptr2;
            public unsafe datUnit* datUnitPtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StructD
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public unsafe EnemyPersonaFunctionStruct3* nextPTR;
            public nint field38;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct anim_func_a3
        {
            public nint Field00;
            public nint Field08;
            public nint Field10;
            public unsafe StructD* PtrToD;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct anim_func_a1
        {
            public nint Field00;
            public nint Field08;
            public nint Field10;
            public nint Field18;
            public unsafe EnemyPersonaFunctionStruct3* Field20;
            public nint Field28;
            public nint Field30;
            public nint Field38;
            public nint Field40;
            public nint Field48;
            public uint Field50;
            public uint CurrentAnim;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EnemyPersonaFunctionStruct2
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public unsafe EnemyPersonaFunctionStruct3* field30;
            public nint field38;
            public nint field40;
            public nint field48;
            public nint field50;
            public nint field58;
            public nint field60;
            public nint field68;
            public nint field70;
            public unsafe anim_func_a1* field78;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct subStruct1
        {
            public nint field00;
            public nint field04;
            public nint field08;
            public nint field0c;
            public nint field10;
            public nint field18;
            public nint field20;
            public unsafe StructD* field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public nint field48;
            public nint field50;
            public nint field58;
            public nint field60;
            public nint field68;
            public nint field70;
            public nint field78;
            public nint field80;
            public nint field88;
            public nint field90;
            public nint field98;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EnemyPersonaFunctionStruct1
        {
            public nint field00;
            public nint field04;
            public nint field08;
            public unsafe EnemyPersonaFunctionStruct2* field0c;
            public unsafe EnemyPersonaFunctionStruct2* field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public nint field48;
            public nint field50;
            public nint field58;
            public nint field60;
            public nint field68;
            public nint field70;
            public nint field78;
            public nint field80;
            public nint field88;
            public nint field90;
            public nint field98;
            public nint fielda0;
            public nint fielda8;
            public nint fieldb0;
            public nint fieldb8;
            public nint fieldc0;
            public nint fieldc8;
            public nint fieldd0;
            public nint fieldd8;
            public nint fielde0;
            public nint fielde8;
            public nint fieldf0;
            public nint fieldf8;
            public nint field100;
            public nint field108;
            public nint field110;
            public nint field118;
            public nint field120;
            public nint field128;
            public unsafe subStruct1* subPtr;
            public nint field138;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CueIDThingy
        {
            public nint Field00;
            public nint Field08;
            public nint Field10;
            public nint Field18;
            public unsafe EnemyPersonaFunctionStruct3* PointerToStruct;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct encounter_vtable_a1
        {
            public unsafe fixed byte unknown[0x5D]; // u8 unknown[0x5D]
            public byte someFlag;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LoadCombatVoiceStruct
        {
            public nint Field00;
            public nint Field08;
            public nint Field10;
            public unsafe EnemyPersonaFunctionStruct3* PointerToStruct;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SkillTBL_ActiveSkill
        {
            public unsafe fixed byte fill[0x2A]; // u8 fill[0x2A]
            public ushort datJyokyoHelpID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EnemyVoiceFuncReturn
        {
            public nint ptr1;
            public nint ptr2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CurrentAIStruct
        {
            public uint field00;
            public uint field04;
            public uint field08;
            public uint field0c;
            public uint field10;
            public uint field14;
            public uint field18;
            public uint field1c;
            public unsafe StructD* StructD_ptr;
            public uint field28;
            public uint field2c;
            public uint act_type;
            public uint field34;
            public uint field38;
            public uint field3c;
            public uint field40;
            public uint field44;
            public uint field48;
            public uint field4c;
            public uint field50;
            public uint field54;
            public uint skillID;
            public uint field5c;
            public uint field60;
            public uint field64;
            public uint field68;
            public uint field6c;
            public uint field70;
            public uint field74;
            public uint field78;
            public uint field7c;
            public nint field80;
            public nint field88;
            public nint field90;
            public unsafe fixed ushort arg_array[10]; // public ushort arg_array[10]
            public ushort arg_count;
            public ushort fieldae;
            public unsafe fixed uint arg_array2[10]; // public uint arg_array2[10]
            public unsafe fixed byte arg_array3[10]; // u8 arg_array3[10]
            public unsafe fixed ushort fill[10]; // public ushort fill[10]
            public ushort prev_skillID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct scrStruct
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public uint field28;
            public int stackValueCount;
            public unsafe fixed byte stackValueTypes[47]; // u8 stackValueTypes[47]
            public byte retValueType;
            public unsafe fixed ulong stackValues[47]; // u64 stackValues[47]
            public nint retValue;
            public nint field1e0;
            public nint field1e8;
            public nint field1f0;
            public nint field1f8;
            public nint field200;
            public nint field208;
            public nint field210;
            public nint field218;
            public nint field220;
            public nint field228;
            public nint field230;
            public nint field238;
            public nint field240;
            public nint field248;
            public nint field250;
            public nint field258;
            public unsafe CurrentAIStruct* currentAIStructPtr;
            public nint field268;
            public nint field270;
            public nint field278;
            public nint field280;
            public nint field288;
            public nint field290;
            public nint field298;
            public nint field2a0;
            public nint field2a8;
            public nint field2b0;
            public nint field2b8;
            public nint field2c0;
            public nint field2c8;
            public nint field2d0;
            public nint field2d8;
            public nint field2e0;
            public nint field2e8;
            public nint field2f0;
            public nint field2f8;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PtrHelper
        {
            public unsafe scrStruct* forwardPtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CalendarTransStruct
        {
            public unsafe fixed byte pad[0x459C]; // u8 pad[0x459C]
            public uint announceState;
            public unsafe fixed byte idk[0x53C3]; // u8 idk[0x53C3]
            public byte isCallingCard;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DaysStruct
        {
            public ushort TotalDays;
            public ushort Field02;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PtrToDays
        {
            public unsafe DaysStruct* ptrtoStruct;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct fileHandleStruct
        {
            public nint fileStatus;
            public unsafe fixed byte filename[128]; // char filename[128]
            public uint bufferSize;
            public uint unk1;
            public uint unk2;
            public uint unk3;
            public nint pointerToFile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PtrToFileHandle
        {
            public unsafe fileHandleStruct* ptrtoStruct;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UnitTBL_Segment0
        {
            public int Flags;
            public byte Arcana;
            public byte RESERVE;
            public ushort UnitLv;
            public int HP;
            public int SP;
            public unsafe fixed byte Stats[6]; // u8 Stats[6]
            public unsafe fixed ushort BattleSkills[8]; // public ushort BattleSkills[8]
            public ushort ExpReward;
            public ushort MoneyReward;
            public unsafe fixed uint ItemDrops[4]; // public uint ItemDrops[4]
            public unsafe fixed ushort EvtItemDrop[3]; // public ushort EvtItemDrop[3]
            public byte AtkAttribute;
            public byte AtkAccuracy;
            public ushort AtkDamage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UnitTBL_Segment3
        {
            public byte VoiceID;
            public byte TALK_PERSON;
            public byte VoicePackABC;
            public byte padding;
            public ushort TALK_MONEYMIN;
            public ushort TALK_MONEYMAX;
            public unsafe fixed ushort TALK_ITEM[4]; // public ushort TALK_ITEM[4]
            public unsafe fixed ushort TALK_ITEM_RARE[4]; // public ushort TALK_ITEM_RARE[4]
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ScriptInterpreter
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public nint field48;
            public nint field50;
            public nint field58;
            public nint field60;
            public nint field68;
            public nint field70;
            public nint field78;
            public nint field80;
            public nint field88;
            public nint field90;
            public nint field98;
            public nint fielda0;
            public nint fielda8;
            public nint fieldb0;
            public nint fieldb8;
            public nint fieldc0;
            public nint fieldc8;
            public nint fieldd0;
            public nint fieldd8;
            public nint fielde0;
            public nint fielde8;
            public nint fieldf0;
            public nint fieldf8;
            public nint field100;
            public nint field108;
            public nint field110;
            public nint field118;
            public nint field120;
            public nint field128;
            public nint field130;
            public nint field138;
            public nint field140;
            public nint field148;
            public nint field150;
            public nint field158;
            public nint field160;
            public nint field168;
            public nint field170;
            public nint field178;
            public nint field180;
            public nint field188;
            public nint field190;
            public nint field198;
            public nint field1a0;
            public nint field1a8;
            public nint field1b0;
            public nint field1b8;
            public nint field1c0;
            public nint field1c8;
            public nint field1d0;
            public nint field1d8;
            public nint field1e0;
            public nint field1e8;
            public nint field1f0;
            public nint field1f8;
            public nint field200;
            public nint field208;
            public nint field210;
            public nint field218;
            public nint field220;
            public nint field228;
            public nint field230;
            public nint field238;
            public nint field240;
            public nint field248;
            public nint field250;
            public nint field258;
            public unsafe CurrentAIStruct* CurrentRunningInst;
            public nint field268;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EncounterStartData
        {
            public uint field00;
            public uint field04;
            public uint field08;
            public uint field0c;
            public uint field10;
            public uint field14;
            public uint field18;
            public uint field1c;
            public uint field20;
            public uint field24;
            public uint field28;
            public uint field2c;
            public uint field30;
            public uint field34;
            public uint field38;
            public uint field3c;
            public uint field40;
            public uint field44;
            public uint field48;
            public uint field4c;
            public uint field50;
            public uint field54;
            public uint field58;
            public uint field5c;
            public uint field60;
            public uint field64;
            public uint field68;
            public uint field6c;
            public uint field70;
            public uint field74;
            public uint field78;
            public uint field7c;
            public uint field80;
            public uint field84;
            public uint field88;
            public uint field8c;
            public uint field90;
            public uint field94;
            public uint field98;
            public uint field9c;
            public uint fielda0;
            public uint fielda4;
            public uint fielda8;
            public uint fieldac;
            public uint fieldb0;
            public uint fieldb4;
            public uint fieldb8;
            public uint fieldbc;
            public uint fieldc0;
            public uint fieldc4;
            public uint fieldc8;
            public uint fieldcc;
            public uint fieldd0;
            public uint fieldd4;
            public uint fieldd8;
            public uint fielddc;
            public uint fielde0;
            public uint fielde4;
            public uint fielde8;
            public uint fieldec;
            public uint fieldf0;
            public uint fieldf4;
            public uint fieldf8;
            public uint fieldfc;
            public uint field100;
            public uint field104;
            public uint field108;
            public uint field10c;
            public uint field110;
            public uint field114;
            public uint field118;
            public uint field11c;
            public uint field120;
            public uint field124;
            public uint field128;
            public uint field12c;
            public uint field130;
            public uint field134;
            public uint field138;
            public uint field13c;
            public uint field140;
            public uint field144;
            public uint field148;
            public uint field14c;
            public uint field150;
            public uint field154;
            public uint field158;
            public uint field15c;
            public uint field160;
            public uint field164;
            public uint field168;
            public uint field16c;
            public uint field170;
            public uint field174;
            public uint field178;
            public uint field17c;
            public uint field180;
            public uint field184;
            public uint field188;
            public uint field18c;
            public uint field190;
            public uint field194;
            public uint field198;
            public uint field19c;
            public uint field1a0;
            public uint field1a4;
            public uint field1a8;
            public uint field1ac;
            public uint field1b0;
            public uint field1b4;
            public uint field1b8;
            public uint field1bc;
            public uint field1c0;
            public uint field1c4;
            public uint field1c8;
            public uint field1cc;
            public uint field1d0;
            public uint field1d4;
            public uint field1d8;
            public uint field1dc;
            public uint field1e0;
            public uint field1e4;
            public uint field1e8;
            public uint field1ec;
            public uint field1f0;
            public uint field1f4;
            public uint field1f8;
            public uint field1fc;
            public uint field200;
            public uint field204;
            public uint field208;
            public uint field20c;
            public uint field210;
            public uint field214;
            public uint field218;
            public uint field21c;
            public uint field220;
            public uint field224;
            public uint field228;
            public uint field22c;
            public uint field230;
            public uint field234;
            public uint field238;
            public uint field23c;
            public uint field240;
            public uint field244;
            public uint field248;
            public uint field24c;
            public uint field250;
            public uint field254;
            public uint field258;
            public uint field25c;
            public uint field260;
            public uint field264;
            public uint field268;
            public uint field26c;
            public uint field270;
            public uint field274;
            public uint encounterID;
            public uint battleFieldSetting;
            public ushort fieldMajor;
            public ushort fieldMinor;
            public ushort envMajor;
            public ushort envMinor;
            public ushort envSub;
            public ushort field28a;
            public uint ambushStatus;
            public uint field290;
            public uint field294;
            public uint field298;
            public uint field29c;
            public uint field2a0;
            public float deltaTime;
            public float encounterTime1;
            public float encounterTime2;
            public uint turnCount;
            public uint EnemyTurns1;
            public uint field2b8;
            public uint AllyTurns1;
            public uint enemyTurns2;
            public uint AllyTurns2;
            public uint field2c8;
            public uint field2cc;
            public uint field2d0;
            public uint field2d4;
            public uint field2d8;
            public uint field2dc;
            public uint field2e0;
            public uint field2e4;
            public uint field2e8;
            public uint field2ec;
            public uint field2f0;
            public uint field2f4;
            public uint field2f8;
            public uint field2fc;
            public uint field300;
            public uint field304;
            public uint field308;
            public uint field30c;
            public uint field310;
            public uint field314;
            public uint field318;
            public uint field31c;
            public uint field320;
            public uint field324;
            public uint field328;
            public uint field32c;
            public uint field330;
            public uint field334;
            public uint field338;
            public uint field33c;
            public uint field340;
            public uint field344;
            public uint field348;
            public uint field34c;
            public uint field350;
            public uint field354;
            public uint field358;
            public uint field35c;
            public uint field360;
            public uint field364;
            public uint field368;
            public uint field36c;
            public uint field370;
            public uint field374;
            public uint field378;
            public uint field37c;
            public uint field380;
            public uint field384;
            public uint field388;
            public uint field38c;
            public uint field390;
            public uint field394;
            public uint field398;
            public uint field39c;
            public uint field3a0;
            public uint field3a4;
            public uint field3a8;
            public uint field3ac;
            public uint field3b0;
            public uint field3b4;
            public uint field3b8;
            public uint field3bc;
            public uint field3c0;
            public uint field3c4;
            public uint field3c8;
            public uint field3cc;
            public uint field3d0;
            public uint field3d4;
            public uint field3d8;
            public uint field3dc;
            public uint field3e0;
            public uint field3e4;
            public uint field3e8;
            public uint field3ec;
            public uint field3f0;
            public uint field3f4;
            public uint field3f8;
            public uint field3fc;
            public uint field400;
            public uint field404;
            public uint field408;
            public uint field40c;
            public uint field410;
            public uint field414;
            public uint field418;
            public uint field41c;
            public uint field420;
            public uint field424;
            public uint field428;
            public uint field42c;
            public uint field430;
            public uint field434;
            public uint field438;
            public uint field43c;
            public uint field440;
            public uint field444;
            public uint field448;
            public uint field44c;
            public uint field450;
            public uint field454;
            public uint field458;
            public uint field45c;
            public uint field460;
            public uint field464;
            public uint field468;
            public uint field46c;
            public uint field470;
            public uint field474;
            public uint field478;
            public uint field47c;
            public uint field480;
            public uint field484;
            public uint field488;
            public uint field48c;
            public uint field490;
            public uint field494;
            public uint field498;
            public uint field49c;
            public uint field4a0;
            public uint field4a4;
            public uint field4a8;
            public uint field4ac;
            public uint field4b0;
            public uint field4b4;
            public uint field4b8;
            public uint field4bc;
            public uint field4c0;
            public uint field4c4;
            public uint field4c8;
            public uint field4cc;
            public uint field4d0;
            public uint field4d4;
            public uint field4d8;
            public uint field4dc;
            public uint field4e0;
            public uint field4e4;
            public uint field4e8;
            public uint field4ec;
            public uint field4f0;
            public uint field4f4;
            public uint field4f8;
            public uint field4fc;
            public uint field500;
            public uint field504;
            public uint field508;
            public uint field50c;
            public uint field510;
            public uint field514;
            public uint field518;
            public uint field51c;
            public uint field520;
            public uint field524;
            public uint field528;
            public uint field52c;
            public uint field530;
            public uint field534;
            public uint field538;
            public uint field53c;
            public uint field540;
            public uint field544;
            public uint field548;
            public uint field54c;
            public uint field550;
            public uint field554;
            public uint field558;
            public uint field55c;
            public uint field560;
            public uint field564;
            public uint field568;
            public uint field56c;
            public uint field570;
            public uint field574;
            public uint field578;
            public uint field57c;
            public uint field580;
            public uint field584;
            public uint field588;
            public uint field58c;
            public uint field590;
            public uint field594;
            public uint field598;
            public uint field59c;
            public uint field5a0;
            public uint field5a4;
            public uint field5a8;
            public uint field5ac;
            public uint field5b0;
            public uint field5b4;
            public uint field5b8;
            public uint field5bc;
            public uint field5c0;
            public uint field5c4;
            public uint field5c8;
            public uint field5cc;
            public uint field5d0;
            public uint field5d4;
            public uint field5d8;
            public uint field5dc;
            public uint field5e0;
            public uint field5e4;
            public uint field5e8;
            public uint field5ec;
            public uint field5f0;
            public uint field5f4;
            public uint field5f8;
            public uint field5fc;
            public uint field600;
            public uint field604;
            public uint field608;
            public uint field60c;
            public uint field610;
            public uint field614;
            public uint field618;
            public uint field61c;
            public uint field620;
            public uint field624;
            public uint field628;
            public uint field62c;
            public uint field630;
            public uint field634;
            public uint field638;
            public uint field63c;
            public uint field640;
            public uint field644;
            public uint field648;
            public uint field64c;
            public uint field650;
            public uint field654;
            public uint field658;
            public uint field65c;
            public uint field660;
            public uint field664;
            public uint field668;
            public uint field66c;
            public uint field670;
            public uint field674;
            public uint field678;
            public uint field67c;
            public uint field680;
            public uint field684;
            public uint field688;
            public uint field68c;
            public uint field690;
            public uint field694;
            public uint field698;
            public uint field69c;
            public uint field6a0;
            public uint field6a4;
            public uint field6a8;
            public uint field6ac;
            public uint field6b0;
            public uint field6b4;
            public uint field6b8;
            public uint field6bc;
            public uint field6c0;
            public uint field6c4;
            public uint field6c8;
            public uint field6cc;
            public uint field6d0;
            public uint field6d4;
            public uint field6d8;
            public uint field6dc;
            public uint field6e0;
            public uint field6e4;
            public uint field6e8;
            public uint field6ec;
            public uint field6f0;
            public uint field6f4;
            public uint field6f8;
            public uint field6fc;
            public uint field700;
            public uint field704;
            public uint field708;
            public uint field70c;
            public uint field710;
            public uint field714;
            public uint field718;
            public uint field71c;
            public uint field720;
            public uint field724;
            public uint field728;
            public uint field72c;
            public uint field730;
            public uint field734;
            public uint field738;
            public uint field73c;
            public uint field740;
            public uint field744;
            public uint field748;
            public uint field74c;
            public uint field750;
            public uint field754;
            public uint field758;
            public uint field75c;
            public uint field760;
            public uint field764;
            public uint field768;
            public uint field76c;
            public uint field770;
            public uint field774;
            public uint field778;
            public uint field77c;
            public uint field780;
            public uint field784;
            public uint field788;
            public uint field78c;
            public uint field790;
            public uint field794;
            public uint field798;
            public uint field79c;
            public uint field7a0;
            public uint field7a4;
            public uint field7a8;
            public uint field7ac;
            public uint field7b0;
            public uint field7b4;
            public uint field7b8;
            public uint field7bc;
            public uint field7c0;
            public uint field7c4;
            public uint field7c8;
            public uint field7cc;
            public uint field7d0;
            public uint field7d4;
            public uint field7d8;
            public uint field7dc;
            public uint field7e0;
            public uint field7e4;
            public uint field7e8;
            public uint field7ec;
            public uint field7f0;
            public uint field7f4;
            public uint field7f8;
            public uint field7fc;
            public uint field800;
            public uint field804;
            public uint field808;
            public uint field80c;
            public uint field810;
            public uint field814;
            public uint field818;
            public uint field81c;
            public uint field820;
            public uint field824;
            public uint field828;
            public uint field82c;
            public uint field830;
            public uint field834;
            public uint field838;
            public uint field83c;
            public uint field840;
            public uint field844;
            public uint field848;
            public uint field84c;
            public uint field850;
            public uint field854;
            public uint field858;
            public uint field85c;
            public uint field860;
            public uint field864;
            public uint field868;
            public uint field86c;
            public uint field870;
            public uint field874;
            public uint field878;
            public uint field87c;
            public uint field880;
            public uint field884;
            public uint field888;
            public uint field88c;
            public uint field890;
            public uint field894;
            public uint field898;
            public uint field89c;
            public uint field8a0;
            public uint field8a4;
            public uint field8a8;
            public uint field8ac;
            public uint field8b0;
            public uint field8b4;
            public uint field8b8;
            public uint field8bc;
            public uint field8c0;
            public uint field8c4;
            public uint field8c8;
            public uint field8cc;
            public uint field8d0;
            public uint field8d4;
            public uint field8d8;
            public uint field8dc;
            public uint field8e0;
            public uint field8e4;
            public uint field8e8;
            public uint field8ec;
            public uint field8f0;
            public uint field8f4;
            public uint field8f8;
            public uint field8fc;
            public uint field900;
            public uint field904;
            public uint field908;
            public uint field90c;
            public uint field910;
            public uint field914;
            public uint field918;
            public uint field91c;
            public uint field920;
            public uint field924;
            public uint field928;
            public uint field92c;
            public uint field930;
            public uint field934;
            public uint field938;
            public uint field93c;
            public uint field940;
            public uint field944;
            public uint field948;
            public uint field94c;
            public uint field950;
            public uint field954;
            public uint field958;
            public uint field95c;
            public uint field960;
            public uint field964;
            public uint field968;
            public uint field96c;
            public uint field970;
            public uint field974;
            public uint field978;
            public uint field97c;
            public uint field980;
            public uint field984;
            public uint field988;
            public uint field98c;
            public uint field990;
            public uint field994;
            public uint field998;
            public uint field99c;
            public uint field9a0;
            public uint field9a4;
            public uint field9a8;
            public uint musicID;
            public uint field9b0;
            public uint field9b4;
            public uint field9b8;
            public uint field9bc;
            public uint field9c0;
            public uint field9c4;
            public uint field9c8;
            public uint field9cc;
            public uint field9d0;
            public uint field9d4;
            public uint field9d8;
            public uint field9dc;
            public uint field9e0;
            public uint field9e4;
            public uint field9e8;
            public uint field9ec;
            public uint field9f0;
            public uint field9f4;
            public uint field9f8;
            public uint field9fc;
            public uint fielda00;
            public uint fielda04;
            public uint fielda08;
            public uint fielda0c;
            public uint fielda10;
            public uint fielda14;
            public uint fielda18;
            public uint fielda1c;
            public uint fielda20;
            public uint fielda24;
            public uint fielda28;
            public uint fielda2c;
            public uint fielda30;
            public uint fielda34;
            public uint fielda38;
            public uint fielda3c;
            public uint fielda40;
            public uint fielda44;
            public uint fielda48;
            public uint fielda4c;
            public uint fielda50;
            public uint fielda54;
            public uint fielda58;
            public uint fielda5c;
            public uint fielda60;
            public uint fielda64;
            public uint fielda68;
            public uint fielda6c;
            public uint fielda70;
            public uint fielda74;
            public uint fielda78;
            public uint fielda7c;
            public uint fielda80;
            public uint fielda84;
            public uint fielda88;
            public uint fielda8c;
            public uint fielda90;
            public uint fielda94;
            public uint fielda98;
            public uint fielda9c;
            public uint fieldaa0;
            public uint fieldaa4;
            public uint fieldaa8;
            public uint fieldaac;
            public uint fieldab0;
            public uint fieldab4;
            public uint fieldab8;
            public uint fieldabc;
            public uint fieldac0;
            public uint fieldac4;
            public uint fieldac8;
            public uint fieldacc;
            public uint fieldad0;
            public uint fieldad4;
            public uint fieldad8;
            public uint fieldadc;
            public uint fieldae0;
            public uint fieldae4;
            public uint fieldae8;
            public uint fieldaec;
            public uint fieldaf0;
            public uint fieldaf4;
            public uint fieldaf8;
            public uint fieldafc;
            public uint fieldb00;
            public uint fieldb04;
            public uint fieldb08;
            public uint fieldb0c;
            public uint fieldb10;
            public uint fieldb14;
            public uint fieldb18;
            public uint fieldb1c;
            public uint fieldb20;
            public uint fieldb24;
            public uint fieldb28;
            public uint fieldb2c;
            public uint fieldb30;
            public uint fieldb34;
            public uint fieldb38;
            public uint fieldb3c;
            public uint fieldb40;
            public uint fieldb44;
            public uint fieldb48;
            public uint fieldb4c;
            public uint fieldb50;
            public uint fieldb54;
            public uint fieldb58;
            public uint fieldb5c;
            public uint fieldb60;
            public uint fieldb64;
            public uint fieldb68;
            public uint fieldb6c;
            public uint fieldb70;
            public uint fieldb74;
            public uint fieldb78;
            public uint fieldb7c;
            public uint fieldb80;
            public uint fieldb84;
            public uint fieldb88;
            public uint fieldb8c;
            public uint fieldb90;
            public uint fieldb94;
            public uint fieldb98;
            public uint fieldb9c;
            public uint fieldba0;
            public uint fieldba4;
            public uint fieldba8;
            public uint fieldbac;
            public uint fieldbb0;
            public uint fieldbb4;
            public uint fieldbb8;
            public uint fieldbbc;
            public uint fieldbc0;
            public uint fieldbc4;
            public uint fieldbc8;
            public uint fieldbcc;
            public uint fieldbd0;
            public uint fieldbd4;
            public uint fieldbd8;
            public uint fieldbdc;
            public uint fieldbe0;
            public uint fieldbe4;
            public uint fieldbe8;
            public uint fieldbec;
            public uint fieldbf0;
            public uint fieldbf4;
            public uint fieldbf8;
            public uint fieldbfc;
            public uint fieldc00;
            public uint fieldc04;
            public uint fieldc08;
            public uint fieldc0c;
            public uint fieldc10;
            public uint fieldc14;
            public uint fieldc18;
            public uint fieldc1c;
            public uint fieldc20;
            public uint fieldc24;
            public uint fieldc28;
            public uint fieldc2c;
            public uint fieldc30;
            public uint fieldc34;
            public uint fieldc38;
            public uint fieldc3c;
            public uint fieldc40;
            public uint fieldc44;
            public uint fieldc48;
            public uint fieldc4c;
            public uint fieldc50;
            public uint fieldc54;
            public uint fieldc58;
            public uint fieldc5c;
            public uint fieldc60;
            public uint fieldc64;
            public uint fieldc68;
            public uint fieldc6c;
            public uint fieldc70;
            public uint fieldc74;
            public uint fieldc78;
            public uint fieldc7c;
            public uint fieldc80;
            public uint fieldc84;
            public uint fieldc88;
            public uint fieldc8c;
            public uint fieldc90;
            public uint fieldc94;
            public uint fieldc98;
            public uint fieldc9c;
            public uint fieldca0;
            public uint fieldca4;
            public uint fieldca8;
            public uint fieldcac;
            public uint fieldcb0;
            public uint fieldcb4;
            public uint fieldcb8;
            public uint fieldcbc;
            public uint fieldcc0;
            public uint fieldcc4;
            public uint fieldcc8;
            public uint fieldccc;
            public uint fieldcd0;
            public uint fieldcd4;
            public uint fieldcd8;
            public uint fieldcdc;
            public uint fieldce0;
            public uint fieldce4;
            public uint fieldce8;
            public uint fieldcec;
            public uint fieldcf0;
            public uint fieldcf4;
            public uint fieldcf8;
            public uint fieldcfc;
            public uint fieldd00;
            public uint fieldd04;
            public uint fieldd08;
            public uint fieldd0c;
            public uint fieldd10;
            public uint fieldd14;
            public uint fieldd18;
            public uint fieldd1c;
            public uint fieldd20;
            public uint fieldd24;
            public uint fieldd28;
            public uint fieldd2c;
            public uint fieldd30;
            public uint fieldd34;
            public uint fieldd38;
            public uint fieldd3c;
            public uint fieldd40;
            public uint fieldd44;
            public uint fieldd48;
            public uint fieldd4c;
            public uint fieldd50;
            public uint fieldd54;
            public uint fieldd58;
            public uint fieldd5c;
            public uint fieldd60;
            public uint fieldd64;
            public uint fieldd68;
            public uint fieldd6c;
            public uint fieldd70;
            public uint fieldd74;
            public uint fieldd78;
            public uint fieldd7c;
            public uint fieldd80;
            public uint fieldd84;
            public uint fieldd88;
            public uint fieldd8c;
            public uint fieldd90;
            public uint fieldd94;
            public uint fieldd98;
            public uint fieldd9c;
            public uint fieldda0;
            public uint fieldda4;
            public uint fieldda8;
            public uint fielddac;
            public uint fielddb0;
            public uint fielddb4;
            public uint fielddb8;
            public uint fielddbc;
            public uint fielddc0;
            public uint fielddc4;
            public uint fielddc8;
            public uint fielddcc;
            public uint fielddd0;
            public uint fielddd4;
            public uint fielddd8;
            public uint fieldddc;
            public uint fieldde0;
            public uint fieldde4;
            public uint fieldde8;
            public uint fielddec;
            public uint fielddf0;
            public uint fielddf4;
            public uint fielddf8;
            public uint fielddfc;
            public uint fielde00;
            public uint fielde04;
            public uint fielde08;
            public uint fielde0c;
            public uint fielde10;
            public uint fielde14;
            public uint fielde18;
            public uint fielde1c;
            public uint fielde20;
            public uint fielde24;
            public uint fielde28;
            public uint fielde2c;
            public uint fielde30;
            public uint fielde34;
            public uint fielde38;
            public uint fielde3c;
            public uint fielde40;
            public uint fielde44;
            public uint fielde48;
            public uint fielde4c;
            public uint fielde50;
            public uint fielde54;
            public uint fielde58;
            public uint fielde5c;
            public uint fielde60;
            public uint fielde64;
            public uint fielde68;
            public uint fielde6c;
            public uint fielde70;
            public uint fielde74;
            public uint fielde78;
            public uint fielde7c;
            public uint fielde80;
            public uint fielde84;
            public uint fielde88;
            public uint fielde8c;
            public uint fielde90;
            public uint fielde94;
            public uint fielde98;
            public uint fielde9c;
            public uint fieldea0;
            public uint fieldea4;
            public uint fieldea8;
            public uint fieldeac;
            public uint fieldeb0;
            public uint fieldeb4;
            public uint fieldeb8;
            public uint fieldebc;
            public uint fieldec0;
            public uint fieldec4;
            public uint fieldec8;
            public uint fieldecc;
            public uint fielded0;
            public uint fielded4;
            public uint fielded8;
            public uint fieldedc;
            public uint fieldee0;
            public uint fieldee4;
            public uint fieldee8;
            public uint fieldeec;
            public uint fieldef0;
            public uint fieldef4;
            public uint fieldef8;
            public uint fieldefc;
            public uint fieldf00;
            public uint fieldf04;
            public uint fieldf08;
            public uint fieldf0c;
            public uint fieldf10;
            public uint fieldf14;
            public uint fieldf18;
            public uint fieldf1c;
            public uint fieldf20;
            public uint fieldf24;
            public uint fieldf28;
            public uint fieldf2c;
            public uint fieldf30;
            public uint fieldf34;
            public uint fieldf38;
            public uint fieldf3c;
            public uint fieldf40;
            public uint fieldf44;
            public uint fieldf48;
            public uint fieldf4c;
            public uint fieldf50;
            public uint fieldf54;
            public uint fieldf58;
            public uint fieldf5c;
            public uint fieldf60;
            public uint fieldf64;
            public uint fieldf68;
            public uint fieldf6c;
            public uint fieldf70;
            public uint fieldf74;
            public uint fieldf78;
            public uint fieldf7c;
            public uint fieldf80;
            public uint fieldf84;
            public uint fieldf88;
            public uint fieldf8c;
            public uint fieldf90;
            public uint fieldf94;
            public uint fieldf98;
            public uint fieldf9c;
            public uint fieldfa0;
            public uint fieldfa4;
            public uint fieldfa8;
            public uint fieldfac;
            public uint fieldfb0;
            public uint fieldfb4;
            public uint fieldfb8;
            public uint fieldfbc;
            public uint fieldfc0;
            public uint fieldfc4;
            public uint fieldfc8;
            public uint fieldfcc;
            public uint fieldfd0;
            public uint fieldfd4;
            public uint fieldfd8;
            public uint fieldfdc;
            public uint fieldfe0;
            public uint fieldfe4;
            public uint fieldfe8;
            public uint fieldfec;
            public uint fieldff0;
            public uint fieldff4;
            public uint fieldff8;
            public uint fieldffc;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EncountPTR
        {
            public unsafe EncounterStartData* ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct encounterIDTBL
        {
            public int flags;
            public ushort Field04;
            public ushort Field06;
            public unsafe fixed ushort BattleUnitID[5]; // u16 BattleUnitID[5]
            public ushort FieldID;
            public ushort RoomID;
            public ushort BGMID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OutfitTBL
        {
            public ushort ModelID;
            public short BGMID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GAPTBL
        {
            public ushort replace_51;
            public ushort replace_52;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EncounterPreCheck
        {
            public unsafe fixed byte pad[0x28C]; // u8 pad[0x28C]
            public uint ambushStatus;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CombatVoiceVTable
        {
            public nint Constructor;         // 00
            public nint RegularAttack;       // 08
            public nint FireGun;             // 10
            public nint ShowtimeSuccess;     // 18
            public nint ShowtimeFail;        // 20
            public nint SingleAttack;        // 28
            public nint FollowUpAttack;      // 30
            public nint UseSkill;            // 38
            public nint UseSkill2;           // 40
            public nint DefeatedJoker;       // 48
            public nint NumberEnemiesLeft;   // 50
            public nint EnemyWontDie;        // 58
            public nint DoMidDamageToEnemy;  // 60
            public nint DoHighDamageToEnemy; // 68
            public nint MissAttack;          // 70
            public nint AttackNulled;        // 78
            public nint EscapeBattle;        // 80
            public nint EscapeBattle2;       // 88
            public nint field90;             // 90
            public nint TakeDamage;          // 98
            public nint TakeDamage2;         // A0
            public nint NullAttack;          // A8
            public nint DrainAttack;         // B0
            public nint TakeDamage3;         // B8
            public nint KnockedDown;         // C0
            public nint fieldc8;             // C8
            public nint DieParty;            // D0
            public nint DieEnemy;            // D8
            public nint DodgeAttack;         // E0
            public nint GetDebuffed;         // E8
            public nint ReactAilment;        // F0
            public nint GetUpFromKnockdown;  // F8
            public nint Revived;             // 100
            public nint EnemyAmbushPlayer;   // 108
            public nint PlayerAmbushEnemy;   // 110
            public nint FieldBattleIntro;    // 118
            public nint ReactAilments2;      // 120
            public nint SwitchPersona;       // 128
            public nint SayPersona;          // 130
            public nint GunAmbush;           // 138
            public nint AoAStart;            // 140
            public nint AoAFinisher;         // 148
            public nint FutabaAoAEnd;        // 150
            public nint UseItem;             // 158
            public nint BatonPassOut;        // 160
            public nint BatonPassIn;         // 168
            public nint field170;            // 170
            public nint SwapOut;             // 178
            public nint SwapIn;              // 180
            public nint RequestFollowUp;     // 188
            public nint PerformFollowUp;     // 190
            public nint RequestGunFollow;    // 198
            public nint PerformGunFollow;    // 1A0
            public nint PreventJokerFatal;   // 1A8
            public nint HarisenRecovery;     // 1B0
            public nint StartHoldUp;         // 1B8
            public nint field1c0;            // 1C0
            public nint WinBattle;           // 1C8
            public nint SayThanks;           // 1D0
            public nint SetTactics;          // 1D8
            public nint RequestSendOff;      // 1E0
            public nint RequestNoSendOff;    // 1E8
            public nint SentOff;             // 1F0
            public nint field1f8;            // 1F8
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct scrCommandTableData
        {
            public nint functionPtr;
            public nint arg_count;
            public unsafe byte* func_name; // char* func_name
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct scrCommandTable
        {
            // public unsafe fixed scrCommandTableData CommandTbl[0x1400]; // scrCommandTableData CommandTbl[0x1400]
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct scrTable
        {
            public unsafe scrCommandTable* COMMON_table;
            public nint COMMON_count;
            public unsafe scrCommandTable* FIELD_table;
            public nint FIELD_count;
            public unsafe scrCommandTable* AI_table;
            public nint AI_count;
            public unsafe scrCommandTable* SOCIAL_table;
            public nint SOCIAL_count;
            public unsafe scrCommandTable* FACILITY_table;
            public nint FACILITY_count;
            public unsafe scrCommandTable* NET_table;
            public nint NET_count;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SubStruct2
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public unsafe EnemyPersonaFunctionStruct3* PtrToSub3;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SubStruct1
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public unsafe SubStruct2* PtrToSub2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UnitScanStruct1
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public nint field48;
            public nint field50;
            public nint field58;
            public nint field60;
            public nint field68;
            public nint field70;
            public nint field78;
            public nint field80;
            public nint field88;
            public nint field90;
            public nint field98;
            public nint fielda0;
            public nint fielda8;
            public nint fieldb0;
            public nint fieldb8;
            public nint fieldc0;
            public nint fieldc8;
            public nint fieldd0;
            public nint fieldd8;
            public nint fielde0;
            public nint fielde8;
            public nint fieldf0;
            public nint fieldf8;
            public nint field100;
            public nint field108;
            public nint field110;
            public nint field118;
            public nint field120;
            public nint field128;
            public unsafe SubStruct1* PtrToSub1;
            public nint field138;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct navi_vtable_struct
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint JokerDie;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public nint field48;
            public nint PartyAilments;
            public nint field58;
            public nint EnemiesDefeated;
            public nint KnockedEnemiesDown;
            public nint field70;
            public nint field78;
            public nint field80;
            public nint field88;
            public nint field90;
            public nint field98;
            public nint fielda0;
            public nint fielda8;
            public nint fieldb0;
            public nint fieldb8;
            public nint fieldc0;
            public nint fieldc8;
            public nint fieldd0;
            public nint fieldd8;
            public nint fielde0;
            public nint fielde8;
            public nint fieldf0;
            public nint fieldf8;
            public nint field100;
            public nint field108;
            public nint field110;
            public nint field118;
            public nint field120;
            public nint field128;
            public nint field130;
            public nint field138;
            public nint field140;
            public nint field148;
            public nint field150;
            public nint field158;
            public nint field160;
            public nint field168;
            public nint field170;
            public nint FutabaUseBuffs;
            public nint field180;
            public nint field188;
            public nint field190;
            public nint field198;
            public nint field1a0;
            public nint field1a8;
            public nint field1b0;
            public nint field1b8;
            public nint field1c0;
            public nint field1c8;
            public nint field1d0;
            public nint field1d8;
            public nint field1e0;
            public nint field1e8;
            public nint field1f0;
            public nint field1f8;
            public nint field200;
            public nint field208;
            public nint field210;
            public nint field218;
            public nint field220;
            public nint field228;
            public nint field230;
            public nint field238;
            public nint field240;
            public nint field248;
            public nint field250;
            public nint field258;
            public nint field260;
            public nint field268;
            public nint field270;
            public nint field278;
            public nint field280;
            public nint field288;
            public nint field290;
            public nint field298;
            public nint field2a0;
            public nint field2a8;
            public nint field2b0;
            public nint field2b8;
            public nint field2c0;
            public nint field2c8;
            public nint field2d0;
            public nint field2d8;
            public nint field2e0;
            public nint field2e8;
            public nint field2f0;
            public nint field2f8;
            public nint field300;
            public nint field308;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct anim_vtable_struct
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public nint Idle_related;
            public nint field50;
            public nint field58;
            public nint IsDownedIdlePlaying;
            public nint Idle;
            public nint Is_Idle_Playing;
            public nint field78;
            public nint field80;
            public nint Run;
            public nint IsRunPlaying;
            public nint Run2;
            public nint IsRun2Playing;
            public nint Anim18;
            public nint ToIdle_MyTurn;
            public nint Is_ToIdle_MyTurn_Playing;
            public nint Idle_MyTurn;
            public nint Is_Idle_MyTurn_Playing;
            public nint Exit_Idle_MyTurn;
            public nint Attack;
            public nint IsAttackAnimPlaying;
            public nint fielde8;
            public nint fieldf0;
            public nint Damage;
            public nint IsDamagePlaying;
            public nint field108;
            public nint field110;
            public nint Dodge;
            public nint GetDodgeDistance;
            public nint CanDodge;
            public nint Anim16;
            public nint IsAnim16Playing;
            public nint field140;
            public nint field148;
            public nint field150;
            public nint field158;
            public nint HopBack;
            public nint field168;
            public nint field170;
            public nint Anim19;
            public nint Skill1;
            public nint IsSkill1Playing;
            public nint Skill2;
            public nint IsSkill2Playing;
            public nint field1a0;
            public nint Skill3;
            public nint IsSkill3Playing;
            public nint Skill4;
            public nint IsSkill4Playing;
            public nint Skill5;
            public nint ItemUse;
            public nint IsItemUsePlaying;
            public nint PersonaChange;
            public nint PersonaChangeAgain;
            public nint Idle2;
            public nint IsIdle2Playing;
            public nint field200;
            public nint PlayGuardAnim;
            public nint IsGuardAnimRepeat;
            public nint Die;
            public nint IsPlayingDeadAnim;
            public nint Revive;
            public nint HarisenRecovery;
            public nint Victory;
            public nint Victory2;
            public nint HoldUp_Talk;
            public nint field250;
            public nint HoldUpAnim;
            public nint GetUp;
            public nint IsGetUpPlaying;
            public nint KnockedDown;
            public nint IsKnockedDownPlaying;
            public nint GiveBaton;
            public nint GetBaton;
            public nint PlayJokerSaved;
            public nint PlayJokerSavedAnim_;
            public nint ExitItemMenu;
            public nint field2a8;
            public nint field2b0;
            public nint field2b8;
            public nint field2c0;
            public nint field2c8;
            public nint EnterTacticsMenu;
            public nint isEnterTacticsMenuPlaying;
            public nint TacticsMenuIdle;
            public nint isTacticsMenuIdlePlaying;
            public nint ExitTacticsMenu;
            public nint isExitTacticsMenuPlaying;
            public nint EnterItemsMenu;
            public nint isEnterItemsMenuPlaying;
            public nint ItemsMenuIdle;
            public nint isItemsMenuIdlePLaying;
            public nint CancelItemsMenu;
            public nint isCancelItemsMenuPlaying;
            public nint field330;
            public nint field338;
            public nint field340;
            public nint field348;
            public nint field350;
            public nint field358;
            public nint field360;
            public nint field368;
            public nint field370;
            public nint field378;
            public nint field380;
            public nint field388;
            public nint field390;
            public nint field398;
            public nint field3a0;
            public nint AoAJump;
            public nint AoAEnd;
            public nint field3b8;
            public nint field3c0;
            public nint field3c8;
            public nint field3d0;
            public nint field3d8;
            public nint field3e0;
            public nint field3e8;
            public nint field3f0;
            public nint field3f8;
            public nint field400;
            public nint field408;
            public nint field410;
            public nint field418;
            public nint field420;
            public nint field428;
            public nint field430;
            public nint field438;
            public nint field440;
            public nint CustomAnimModifier;
            public nint field450;
            public nint field458;
            public nint field460;
            public nint field468;
            public nint field470;
            public nint field478;
            public nint field480;
            public nint field488;
            public nint field490;
            public nint field498;
            public nint field4a0;
            public nint field4a8;
            public nint field4b0;
            public nint field4b8;
            public nint field4c0;
            public nint field4c8;
            public nint field4d0;
            public nint field4d8;
            public nint field4e0;
            public nint field4e8;
            public nint field4f0;
            public nint field4f8;
            public nint field500;
            public nint field508;
            public nint field510;
            public nint field518;
            public nint field520;
            public nint field528;
            public nint field530;
            public nint field538;
            public nint field540;
            public nint field548;
            public nint field550;
            public nint field558;
            public nint field560;
            public nint field568;
            public nint field570;
            public nint field578;
            public nint field580;
            public nint field588;
            public nint field590;
            public nint field598;
            public nint field5a0;
            public nint field5a8;
            public nint field5b0;
            public nint field5b8;
            public nint field5c0;
            public nint field5c8;
            public nint field5d0;
            public nint field5d8;
            public nint field5e0;
            public nint field5e8;
            public nint field5f0;
            public nint field5f8;
            public nint field600;
            public nint field608;
            public nint field610;
            public nint field618;
            public nint field620;
            public nint field628;
            public nint field630;
            public nint field638;
            public nint field640;
            public nint field648;
            public nint field650;
            public nint field658;
            public nint field660;
            public nint field668;
            public nint field670;
            public nint field678;
            public nint field680;
            public nint field688;
            public nint field690;
            public nint field698;
            public nint field6a0;
            public nint field6a8;
            public nint field6b0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EnemyVisualATBL
        {
            public uint field00;
            public float field04;
            public float field08;
            public float field0c;
            public float field10;
            public float field14;
            public float field18;
            public float field1c;
            public float field20;
            public float field24;
            public float field28;
            public float field2c;
            public float field30;
            public float field34;
            public float field38;
            public float field3c;
            public float field40;
            public float field44;
            public float field48;
            public float field4c;
            public float field50;
            public float field54;
            public float field58;
            public float field5c;
            public float field60;
            public float field64;
            public float field68;
            public float field6c;
            public float field70;
            public float field74;
            public float field78;
            public float field7c;
            public unsafe fixed short AttackHitRegFrames[8]; // s16 AttackHitRegFrames[8]
            public short AttackAnimSpeed;
            public short DistFromTarget;
            public unsafe fixed short UnkVal1[2]; // s16 UnkVal1[2]
            public unsafe fixed short MagicHitRegFrames[8]; // s16 MagicHitRegFrames[8]
            public short MagicAnimSpeed;
            public short MagicDistFromTarget;
            public unsafe fixed short UnkVal2[2]; // s16 UnkVal2[2]
            public unsafe fixed short UnkHitRegFrames[8]; // s16 UnkHitRegFrames[8]
            public short UnkAnimSpeed;
            public short UnkDistFromTarget;
            public unsafe fixed short UnkVal3[2]; // s16 UnkVal3[2]
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COUNTERS
        {
            public unsafe fixed int COUNT[500]; // int COUNT[500]
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FieldWorkData
        {
            public uint field00;
            public uint field04;
            public uint field08;
            public uint field0c;
            public uint field10;
            public uint field14;
            public uint field18;
            public uint field1c;
            public uint field20;
            public uint field24;
            public uint field28;
            public uint field2c;
            public uint field30;
            public uint field34;
            public uint field38;
            public uint field3c;
            public uint field40;
            public uint field44;
            public uint field48;
            public uint field4c;
            public uint field50;
            public uint field54;
            public uint field58;
            public uint field5c;
            public uint field60;
            public uint field64;
            public uint field68;
            public uint field6c;
            public uint field70;
            public uint field74;
            public uint field78;
            public uint field7c;
            public uint field80;
            public uint field84;
            public uint field88;
            public uint field8c;
            public uint field90;
            public uint field94;
            public uint field98;
            public uint field9c;
            public uint fielda0;
            public uint fielda4;
            public uint fielda8;
            public uint fieldac;
            public uint fieldb0;
            public uint fieldb4;
            public uint fieldb8;
            public uint fieldbc;
            public uint fieldc0;
            public uint fieldc4;
            public uint fieldc8;
            public uint fieldcc;
            public uint fieldd0;
            public uint fieldd4;
            public uint fieldd8;
            public uint fielddc;
            public uint fielde0;
            public uint fielde4;
            public uint fielde8;
            public uint fieldec;
            public uint fieldf0;
            public uint fieldf4;
            public uint fieldf8;
            public uint fieldfc;
            public uint field100;
            public uint field104;
            public uint field108;
            public uint field10c;
            public uint field110;
            public uint field114;
            public uint field118;
            public uint field11c;
            public uint field120;
            public uint field124;
            public uint field128;
            public uint field12c;
            public uint field130;
            public uint field134;
            public uint field138;
            public uint field13c;
            public uint field140;
            public uint field144;
            public uint field148;
            public uint field14c;
            public uint field150;
            public uint field154;
            public uint field158;
            public uint field15c;
            public uint field160;
            public uint field164;
            public uint field168;
            public uint field16c;
            public uint field170;
            public uint field174;
            public uint field178;
            public uint field17c;
            public uint field180;
            public uint field184;
            public uint field188;
            public uint field18c;
            public uint field190;
            public uint field194;
            public uint field198;
            public uint field19c;
            public uint field1a0;
            public uint field1a4;
            public uint field1a8;
            public uint field1ac;
            public uint field1b0;
            public uint field1b4;
            public uint field1b8;
            public uint field1bc;
            public uint field1c0;
            public uint field1c4;
            public uint field1c8;
            public uint field1cc;
            public uint field1d0;
            public uint field1d4;
            public uint field1d8;
            public uint field1dc;
            public uint field1e0;
            public uint field1e4;
            public uint field1e8;
            public uint field1ec;
            public uint field1f0;
            public uint field1f4;
            public uint field1f8;
            public uint field1fc;
            public uint field200;
            public uint field204;
            public uint field208;
            public uint field20c;
            public uint field210;
            public uint field214;
            public uint field218;
            public uint field21c;
            public uint field220;
            public uint field224;
            public uint field228;
            public uint field22c;
            public uint field230;
            public uint field234;
            public uint field238;
            public uint field23c;
            public uint field240;
            public uint field244;
            public uint field248;
            public uint field24c;
            public uint field250;
            public uint field254;
            public uint field258;
            public uint field25c;
            public uint field260;
            public uint field264;
            public uint field268;
            public uint field26c;
            public uint field270;
            public uint field274;
            public uint field278;
            public uint field27c;
            public uint field280;
            public uint field284;
            public uint field288;
            public uint field28c;
            public uint field290;
            public uint field294;
            public uint field298;
            public uint field29c;
            public uint field2a0;
            public uint field2a4;
            public uint field2a8;
            public uint field2ac;
            public uint field2b0;
            public uint field2b4;
            public uint field2b8;
            public uint field2bc;
            public uint field2c0;
            public uint field2c4;
            public uint field2c8;
            public uint field2cc;
            public uint field2d0;
            public uint field2d4;
            public uint field2d8;
            public uint field2dc;
            public uint field2e0;
            public uint field2e4;
            public uint field2e8;
            public uint field2ec;
            public uint field2f0;
            public uint field2f4;
            public uint field2f8;
            public uint field2fc;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FMWK_Pre1
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public unsafe FieldWorkData* FMWK_Ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TitleScreenSubStruct
        {
            public ushort Field00;
            public ushort StateID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TitleScreenStruct
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public unsafe TitleScreenSubStruct* PtrToSub1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct cutin_phase_sub_1
        {
            public int cutin_phase_current;
            public int cutin_type;
            public nint cutin_targetDDS;
            public nint cutin_gmd_string;
            public nint Field18;
            public nint Field20;
            public nint Field28;
            public ushort unit_type;
            public ushort unit_id;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct cutin_function_1
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public unsafe cutin_phase_sub_1* ptr1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sequence_struct_2
        {
            public uint field00;
            public uint sequenceID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sequence_struct_1
        {
            public nint field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public unsafe sequence_struct_2* ptr1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sequence_struct_0
        {
            public unsafe sequence_struct_1* preptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct encounter_vtable_struct
        {
            public nint field0_0x0;
            public nint Load;
            public nint IsReady;
            public nint Apply_Stage;
            public nint Initialize;
            public nint Starting_Turn;
            public nint Pursuit_BCD;
            public nint field7_0x38;
            public nint Finalize;
            public nint PhaseBeginning;
            public nint field10_0x50;
            public nint field11_0x58;
            public nint field12_0x60;
            public nint field13_0x68;
            public nint ActionStandby;
            public nint field15_0x78;
            public nint ActionAIStart;
            public nint field17_0x88;
            public nint field18_0x90;
            public nint CalcEfficacy;
            public nint field20_0xa0;
            public nint field21_0xa8;
            public nint ActionEndTurn;
            public nint field23_0xb8;
            public nint ActionDispatch;
            public nint ActionPrepare;
            public nint field26_0xd0;
            public nint field27_0xd8;
            public nint field28_0xe0;
            public nint field29_0xe8;
            public nint IsOccurrenceHoldUp;
            public nint field31_0xf8;
            public nint field32_0x100;
            public nint field33_0x108;
            public nint field34_0x110;
            public nint field35_0x118;
            public nint field36_0x120;
            public nint field37_0x128;
            public nint field38_0x130;
            public nint field39_0x138;
            public nint field40_0x140;
            public nint field41_0x148;
            public nint SkillLookTarget;
            public nint field43_0x158;
            public nint field44_0x160;
            public nint CreateSkillBEDEffectAndSEFilename;
            public nint IsLookingTargetOnCommandview;
            public nint Unit_Update_Control;
            public nint IsSwitchToChantSkillExtensionAnimation;
            public nint IsChangeSkillExtensionAnimation;
            public nint ChangeSkillExtensionAnimation;
            public nint IsShowChantEffect;
            public nint GetSkillShotTiming;
            public nint IsLoadExtension;
            public nint HoldUpResult;
            public nint field55_0x1b8;
            public nint field56_0x1c0;
            public nint field57_0x1c8;
            public nint CalculateCamera;
            public nint field59_0x1d8;
            public nint field60_0x1e0;
            public nint field61_0x1e8;
            public nint field62_0x1f0;
            public nint field63_0x1f8;
            public nint field64_0x200;
            public nint field65_0x208;
            public nint field66_0x210;
            public nint field67_0x218;
            public nint field68_0x220;
            public nint field69_0x228;
            public nint field70_0x230;
            public nint field71_0x238;
            public nint Summon;
            public nint field73_0x248;
            public nint field74_0x250;
            public nint field75_0x258;
            public nint field76_0x260;
            public nint field77_0x268;
            public nint field78_0x270;
            public nint field79_0x278;
            public nint Can_Use_Combat_Cutin;
            public nint Force_Combat_Cutin;
            public nint field82_0x290;
            public nint field83_0x298;
            public nint Persona_Summon;
            public nint field85_0x2a8;
            public nint field86_0x2b0;
            public nint field87_0x2b8;
            public nint field88_0x2c0;
            public nint field89_0x2c8;
            public nint field90_0x2d0;
            public nint field91_0x2d8;
            public nint field92_0x2e0;
            public nint field93_0x2e8;
            public nint field94_0x2f0;
            public nint field95_0x2f8;
            public nint field96_0x300;
            public nint field97_0x308;
            public nint field98_0x310;
            public nint field99_0x318;
            public nint field100_0x320;
            public nint field101_0x328;
            public nint field102_0x330;
            public nint field103_0x338;
            public nint Intro_BCD;
            public nint field105_0x348;
            public nint field106_0x350;
            public nint field107_0x358;
            public nint field108_0x360;
            public nint field109_0x368;
            public nint field110_0x370;
            public nint field111_0x378;
            public nint field112_0x380;
            public nint field113_0x388;
            public nint field114_0x390;
            public nint field115_0x398;
            public nint field116_0x3a0;
            public nint field117_0x3a8;
            public nint field118_0x3b0;
            public nint field119_0x3b8;
            public nint field120_0x3c0;
            public nint field121_0x3c8;
            public nint field122_0x3d0;
            public nint field123_0x3d8;
            public nint field124_0x3e0;
            public nint field125_0x3e8;
            public nint field126_0x3f0;
            public nint field127_0x3f8;
            public nint field128_0x400;
            public nint field129_0x408;
            public nint field130_0x410;
            public nint field131_0x418;
            public nint Order;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct encounter_vtable_memory
        {
            public unsafe encounter_vtable_struct* field00;
            public nint field08;
            public nint field10;
            public nint field18;
            public nint field20;
            public nint field28;
            public nint field30;
            public nint field38;
            public nint field40;
            public nint field48;
            public nint field50;
            public nint field58;
            public nint field60;
            public nint field68;
            public nint field70;
            public nint field78;
            public nint field80;
            public nint field88;
            public nint field90;
            public nint field98;
            public nint fielda0;
            public nint fielda8;
            public nint fieldb0;
            public nint fieldb8;
            public nint fieldc0;
            public nint fieldc8;
            public nint fieldd0;
            public nint fieldd8;
            public nint fielde0;
            public nint fielde8;
            public nint fieldf0;
            public nint fieldf8;
            public nint field100;
            public nint field108;
            public nint field110;
            public nint field118;
            public nint field120;
            public nint field128;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UnitAffinity
        {
            public unsafe fixed ushort Affinity[20]; // u16 Affinity[20]
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UnitTBLEnemyAffinity
        {
            // public unsafe fixed UnitAffinity EnemyAffinity[783]; // UnitAffinity EnemyAffinity[783]
        }
    }
}
